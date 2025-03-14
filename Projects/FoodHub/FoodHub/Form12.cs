using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FoodHub
{
    public partial class Form12 : Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";
        private string email;

        private int ingredientComboBoxCount = 3;
        private const int minimumIngredients = 3;
        private List<int> selectedIngredients = new List<int>();

        public Form12(string email)
        {
            InitializeComponent();
            this.email = email;
        }

        private void Form12_Load(object sender, EventArgs e)
        {
            allFoodItems();
            for (int i = 0; i < 3; i++)
            {
                ComboBox comboBox = new ComboBox();
                comboBox.Name = "comboBoxIngredient" + i;
                comboBox.Width = 200;
                comboBox.Location = new Point(10, 40 * i);
                PopulateIngredientsComboBox(comboBox);
                comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
                panelIngredients.Controls.Add(comboBox);
            }
        }

        private void PopulateIngredientsComboBox(ComboBox comboBox)
        {
            string queryGetAllIngredients = "SELECT ING_ID, ING_Name FROM INGREDIENTS";
            List<KeyValuePair<int, string>> availableIngredients = new List<KeyValuePair<int, string>>();

            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(queryGetAllIngredients, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int ingredientId = reader.GetInt32(0);
                                string ingredientName = reader.GetString(1);

                                // Only add ingredients that haven't been selected yet
                                if (!selectedIngredients.Contains(ingredientId))
                                {
                                    availableIngredients.Add(new KeyValuePair<int, string>(ingredientId, ingredientName));
                                }
                            }
                        }
                    }


                    var previouslySelectedItem = comboBox.SelectedItem;

                    if (previouslySelectedItem == null)
                    {
                        comboBox.Items.Clear();
                    }
                    comboBox.Items.AddRange(availableIngredients.Cast<object>().ToArray());
                    comboBox.DisplayMember = "Value";
                    comboBox.ValueMember = "Key";
                    if (previouslySelectedItem != null && availableIngredients.Any(i => i.Key == ((KeyValuePair<int, string>)previouslySelectedItem).Key))
                    {
                        comboBox.SelectedItem = previouslySelectedItem;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }




        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedItem != null)
            {

                KeyValuePair<int, string> selectedIngredient = (KeyValuePair<int, string>)comboBox.SelectedItem;
                int ingredientId = selectedIngredient.Key;

                if (!selectedIngredients.Contains(ingredientId))
                {
                    selectedIngredients.Add(ingredientId);
                }

                // Refresh all ComboBoxes
                foreach (Control ctrl in panelIngredients.Controls)
                {
                    if (ctrl is ComboBox cb)
                    {
                        PopulateIngredientsComboBox(cb);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ingredientComboBoxCount < 20)
            {
                ComboBox newComboBox = new ComboBox();
                newComboBox.Name = "comboBoxIngredient" + ingredientComboBoxCount;
                newComboBox.Width = 200;
                newComboBox.Location = new Point(10, 40 * ingredientComboBoxCount);
                PopulateIngredientsComboBox(newComboBox);
                panelIngredients.Controls.Add(newComboBox);
                newComboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;

                ingredientComboBoxCount++;
            }
            else
            {
                MessageBox.Show("Maximum of 10 ingredients can be added.", "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Check if at least 3 ComboBoxes are selected
            List<int> selectedIngredientIds = new List<int>();
            foreach (Control ctrl in panelIngredients.Controls)
            {
                if (ctrl is ComboBox comboBox && comboBox.SelectedItem != null)
                {
                    KeyValuePair<int, string> selectedIngredient = (KeyValuePair<int, string>)comboBox.SelectedItem;
                    selectedIngredientIds.Add(selectedIngredient.Key);
                }
            }

            if (selectedIngredientIds.Count < minimumIngredients)
            {
                MessageBox.Show("Please select at least three ingredients.", "Input Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string foodName = fName.Text;
            string price = textBox1.Text;
            string category = textBox2.Text;
            if (EditFoodItem.SelectedItem != null)
            {
                KeyValuePair<int, string> selectedItem = (KeyValuePair<int, string>)EditFoodItem.SelectedItem;
                int foodID = selectedItem.Key;
                UpdateFoodItemInDatabase(foodName, price, category, selectedIngredientIds, foodID);
            }
            else
            {
                AddFoodItemToDatabase(foodName, price, category, selectedIngredientIds);
            }
        }

        private void AddFoodItemToDatabase(string foodName, string price, string category, List<int> ingredientIds)
        {
            string queryInsertFoodItem = "INSERT INTO ITEM (Item_Name, Item_Category, Item_Price) VALUES (@FoodName, @Category, @Price)";
            string querySelectFoodItemID = "SELECT Item_No FROM ITEM WHERE Item_Name = @FoodName";
            string queryInsertFoodIngredients = "INSERT INTO ITEMINGREDIENTS (ING_ID, Item_No) VALUES (@IngredientID, @FoodItemID)";

            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();

                    int foodItemID;
                    using (SqlCommand cmd = new SqlCommand(queryInsertFoodItem, conn))
                    {
                        cmd.Parameters.AddWithValue("@FoodName", foodName);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@Category", category);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand(querySelectFoodItemID, conn))
                    {
                        cmd.Parameters.AddWithValue("@FoodName", foodName);
                        foodItemID = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (foodItemID == 0)
                    {

                        MessageBox.Show("Failed to insert the food item.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    foreach (int ingredientID in ingredientIds)
                    {
                        using (SqlCommand cmd = new SqlCommand(queryInsertFoodIngredients, conn))
                        {
                            cmd.Parameters.AddWithValue("@FoodItemID", foodItemID);
                            cmd.Parameters.AddWithValue("@IngredientID", ingredientID);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Food item added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            adminPanel admin = new adminPanel(email);
            admin.Show();
            this.Hide();
        }

        private void EditFoodItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EditFoodItem.SelectedItem != null)
            {
                // Get the selected item (KeyValuePair<int, string>)
                var selectedItem = (KeyValuePair<int, string>)EditFoodItem.SelectedItem;
                int selectedItemId = selectedItem.Key;

                // Query the food item details based on the selected item ID
                string queryFoodDetails = "SELECT Item_Name, Item_Category, Item_Price FROM ITEM WHERE Item_No = @ItemID";
                string queryFoodIngredients = "SELECT ING_ID FROM ITEMINGREDIENTS WHERE Item_No = @ItemID"; // To get the associated ingredients

                using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
                {
                    try
                    {
                        conn.Open();

                        // Retrieve food item details
                        string foodName = "", foodCategory = "", foodPrice = "";
                        using (SqlCommand cmd = new SqlCommand(queryFoodDetails, conn))
                        {
                            cmd.Parameters.AddWithValue("@ItemID", selectedItemId);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    foodName = reader.GetString(0);
                                    foodCategory = reader.GetString(1);
                                    foodPrice = reader.GetDecimal(2).ToString();
                                }
                            }
                        }

                        // Populate the input fields
                        fName.Text = foodName; // Set the food name in the input field
                        textBox2.Text = foodCategory; // Set the food category
                        textBox1.Text = foodPrice; // Set the food price

                        // Clear the selected ingredients and populate the ingredient ComboBoxes
                        selectedIngredients.Clear();
                        foreach (Control ctrl in panelIngredients.Controls)
                        {
                            if (ctrl is ComboBox cb)
                            {
                                cb.SelectedItem = null; // Clear any previous selection
                                PopulateIngredientsComboBox(cb); // Re-populate the ComboBox
                            }
                        }

                        // Retrieve and display the ingredients for this food item
                        using (SqlCommand cmd = new SqlCommand(queryFoodIngredients, conn))
                        {
                            cmd.Parameters.AddWithValue("@ItemID", selectedItemId);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                int comboBoxIndex = 0; // Track the index of ComboBox to update
                                while (reader.Read() && comboBoxIndex < panelIngredients.Controls.Count)
                                {
                                    int ingredientId = reader.GetInt32(0); // Multiple ingredients can be returned here
                                    selectedIngredients.Add(ingredientId); // Add the ingredient ID to the list

                                    // Find the ComboBox to update and set the selected item
                                    foreach (Control ctrl in panelIngredients.Controls)
                                    {
                                        if (ctrl is ComboBox comboBox && comboBoxIndex == panelIngredients.Controls.GetChildIndex(ctrl))
                                        {
                                            // Set the selected item in the ComboBox
                                            foreach (KeyValuePair<int, string> item in comboBox.Items)
                                            {
                                                if (item.Key == ingredientId)
                                                {
                                                    comboBox.SelectedItem = item; // Set the ingredient
                                                    break;
                                                }
                                            }
                                            break;
                                        }
                                    }

                                    comboBoxIndex++; // Move to the next ComboBox
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void UpdateFoodItemInDatabase(string foodName, string price, string category, List<int> ingredientIds, int foodItemID)
        {
            string queryUpdateFoodItem = "UPDATE ITEM SET Item_Name = @FoodName, Item_Category = @Category, Item_Price = @Price WHERE Item_No = @FoodItemID";
            string queryDeleteFoodIngredients = "DELETE FROM ITEMINGREDIENTS WHERE Item_No = @FoodItemID";  // Remove previous ingredients
            string queryInsertFoodIngredients = "INSERT INTO ITEMINGREDIENTS (ING_ID, Item_No) VALUES (@IngredientID, @FoodItemID)";

            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();

                    // Update food item details
                    using (SqlCommand cmd = new SqlCommand(queryUpdateFoodItem, conn))
                    {
                        cmd.Parameters.AddWithValue("@FoodName", foodName);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@Category", category);
                        cmd.Parameters.AddWithValue("@FoodItemID", foodItemID);
                        cmd.ExecuteNonQuery();
                    }

                    // Remove existing ingredients for this food item
                    using (SqlCommand cmd = new SqlCommand(queryDeleteFoodIngredients, conn))
                    {
                        cmd.Parameters.AddWithValue("@FoodItemID", foodItemID);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert the new ingredients
                    foreach (int ingredientID in ingredientIds)
                    {
                        using (SqlCommand cmd = new SqlCommand(queryInsertFoodIngredients, conn))
                        {
                            cmd.Parameters.AddWithValue("@FoodItemID", foodItemID);
                            cmd.Parameters.AddWithValue("@IngredientID", ingredientID);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Food item updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void allFoodItems()
        {
            string queryGetAllFoodItems = "SELECT Item_No, Item_Name FROM ITEM";

            // Populate ComboBox with all food items
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(queryGetAllFoodItems, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int itemId = reader.GetInt32(0);
                                string itemName = reader.GetString(1);

                                // Add food items to the ComboBox
                                EditFoodItem.Items.Add(new KeyValuePair<int, string>(itemId, itemName));
                            }
                        }
                    }

                    // Set the ComboBox DisplayMember and ValueMember
                    EditFoodItem.DisplayMember = "Value";
                    EditFoodItem.ValueMember = "Key";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
