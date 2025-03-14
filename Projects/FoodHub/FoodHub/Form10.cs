using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FoodHub
{
    public partial class Form10 : Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";
        private string email;
        public Form10(string email)
        {
            InitializeComponent();
            this.email = email;
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null)
            {
                editIngredient();
            } 
            else
            {
                AddnewIngredient();
            }

        }

        private void AddnewIngredient()
        {
            string Iname = fName.Text;
            string description = textBox1.Text;
            if (!string.IsNullOrEmpty(Iname))
            {
                string queryInI = "INSERT INTO INGREDIENTS (ING_Name, ING_Description) VALUES (@Iname, @description)";
                using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd23 = new SqlCommand(queryInI, conn))
                        {
                            cmd23.Parameters.AddWithValue("@Iname", Iname);
                            cmd23.Parameters.AddWithValue("@description", description);

                            // Executing the query
                            int result = cmd23.ExecuteNonQuery();

                            // Checking if rows were affected
                            if (result > 0)
                            {
                                MessageBox.Show("Ingredient added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Error adding ingredient.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                label12.Visible = true;
                MessageBox.Show("Ingredient Name is required", "Input Validation error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void editIngredient()
        {
            KeyValuePair<int, string> selectedIngredient = (KeyValuePair<int, string>)comboBox2.SelectedItem;
            int selectedIng = selectedIngredient.Key;
            string Iname = fName.Text;
            string description = textBox1.Text;
            if (!string.IsNullOrEmpty(Iname))
            {
                string queryUpdate = "UPDATE INGREDIENTS SET ING_Name = @Iname, ING_Description = @description WHERE ING_ID = @selectedIng";
                using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
                {
                    try
                    {
                        conn.Open();

                        // Create the SQL command
                        using (SqlCommand cmd = new SqlCommand(queryUpdate, conn))
                        {
                            // Add parameters to prevent SQL injection
                            cmd.Parameters.AddWithValue("@Iname", Iname);
                            cmd.Parameters.AddWithValue("@description", description);
                            cmd.Parameters.AddWithValue("@selectedIng", selectedIng);  // Use the selected ingredient ID to identify the record

                            // Execute the command to update the ingredient
                            int result = cmd.ExecuteNonQuery();

                            // Check if the update was successful
                            if (result > 0)
                            {
                                MessageBox.Show("Ingredient updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Error updating ingredient or ingredient not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Display an error message if something goes wrong
                        MessageBox.Show("Error: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                label12.Visible = true;
                MessageBox.Show("Ingredient Name is required", "Input Validation error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void getIngredients()
        {
            string queryGetAllIngredients = "SELECT ING_ID, ING_Name FROM INGREDIENTS";
            comboBox2.Items.Clear();
            comboBox2.SelectedIndex = -1;
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
                                comboBox2.Items.Add(new KeyValuePair<int, string>(ingredientId, ingredientName));
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

        private void Form10_Load(object sender, EventArgs e)
        {
            getIngredients();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            KeyValuePair<int, string> selectedIngredient = (KeyValuePair<int, string>)comboBox2.SelectedItem;
            int selectedId = selectedIngredient.Key;
            getSelectedIng(selectedId);
        }

        private void getSelectedIng(int selected)
        {
            string queryGetDetails = "SELECT ING_Name, ING_Description FROM INGREDIENTS WHERE ING_ID = @ingredientId";
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(queryGetDetails, conn))
                    {
                        cmd.Parameters.AddWithValue("@ingredientId", selected);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())  // Check if any result was returned
                            {
                                fName.Text = reader.GetString(0);  // ING_Name
                                textBox1.Text = reader.GetString(1);
                            }
                            else
                            {
                                // If no ingredient was found for the given ID, show an error or message
                                MessageBox.Show("Ingredient not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void button1_Click(object sender, EventArgs e)
        {
            adminPanel admin = new adminPanel(email);
            admin.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form10 hgjsd = new Form10(email);
            this.Hide();
            hgjsd.Show();
        }

        private void label30_Click(object sender, EventArgs e)
        {

        }
    }
}
