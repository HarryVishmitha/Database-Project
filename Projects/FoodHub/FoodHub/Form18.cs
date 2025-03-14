using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace FoodHub
{
    public partial class Form18: Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";

        private string email;
        private List<string[]> itemList = new List<string[]>();
        private List<Tuple<string, int>> cartItems = new List<Tuple<string, int>>();

        public string Last_Name { get; set; }

        public Form18(string email)
        {
            InitializeComponent();
            this.email = email;
            

        }

        private void Form18_Load(object sender, EventArgs e)
        {
            getUserDetails();
            userName.Text = Last_Name;
            getAllitems();
        }

        private void getUserDetails()
        {
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();

                    // Step 1: Get the Login_ID based on the email
                    string getLoginIdQuery = "SELECT Login_ID FROM LOGINS WHERE EmailAddress = @Email";
                    SqlCommand getLoginIdCommand = new SqlCommand(getLoginIdQuery, conn);
                    getLoginIdCommand.Parameters.AddWithValue("@Email", email);

                    int loginId = Convert.ToInt32(getLoginIdCommand.ExecuteScalar());

                    // Step 2: Fetch the rider details using the Login_ID
                    string getRiderDetailsQuery = @"SELECT * FROM CUSTOMER WHERE Login_ID = @Login_ID";

                    SqlCommand getRiderDetailsCommand = new SqlCommand(getRiderDetailsQuery, conn);
                    getRiderDetailsCommand.Parameters.AddWithValue("@Login_ID", loginId);

                    using (SqlDataReader reader = getRiderDetailsCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Last_Name = reader["Last_name"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dashboard djgh = new Dashboard(email);
            this.Hide();
            djgh.Show();
        }

        public void getAllitems()
        {
            string query = "SELECT * FROM ITEM";
            try
            {
                using (SqlConnection connection = new SqlConnection(FoodHubConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Initialize itemDetails array with 4 elements to store the necessary fields
                                string[] itemDetails = new string[4];  // We need 4 items in the array
                                itemDetails[0] = reader["Item_Name"].ToString();
                                itemDetails[1] = reader["Item_Category"].ToString();
                                itemDetails[2] = reader["Item_Price"].ToString();
                                itemDetails[3] = reader["Item_No"].ToString();  // Storing Item_No in index 3

                                itemList.Add(itemDetails);  // Add the itemDetails to the itemList
                            }
                        }
                    }
                }

                int yOffset = 10; // Initial Y offset for the first item panel

                // Loop through each item in the list and create dynamic controls
                foreach (var item in itemList)
                {
                    // Create a new Panel to hold this product's details
                    Panel productPanel = new Panel
                    {
                        Size = new Size(300, 180), // Set panel size
                        Location = new Point(10, yOffset), // Position the panel on the form
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    // Add Label for Item Name
                    Label lblItemName = new Label
                    {
                        Text = "Name: " + item[0], // Use item[0] for item name
                        Location = new Point(10, 10),
                        Size = new Size(280, 20)
                    };
                    productPanel.Controls.Add(lblItemName);

                    // Add Label for Item Category
                    Label lblItemCategory = new Label
                    {
                        Text = "Category: " + item[1], // Use item[1] for item category
                        Location = new Point(10, 40),
                        Size = new Size(280, 20)
                    };
                    productPanel.Controls.Add(lblItemCategory);

                    // Add Label for Item Price
                    Label lblItemPrice = new Label
                    {
                        Text = "Price: $" + item[2], // Use item[2] for item price
                        Location = new Point(10, 70),
                        Size = new Size(280, 20)
                    };
                    productPanel.Controls.Add(lblItemPrice);

                    // Add TextBox for Quantity
                    TextBox txtQuantity = new TextBox
                    {
                        Location = new Point(10, 100),
                        Size = new Size(100, 20)
                    };
                    productPanel.Controls.Add(txtQuantity);

                    // Add Button for Check Ingredients
                    Button btnCheckIngredients = new Button
                    {
                        Text = "Check Ingredients",
                        Location = new Point(120, 100),
                        Size = new Size(120, 30)
                    };
                    btnCheckIngredients.Click += (s, e) => CheckIngredients(item[3]); // Pass Item_No (item[3])
                    productPanel.Controls.Add(btnCheckIngredients);

                    // Add Button for Add to Cart
                    Button btnAddToCart = new Button
                    {
                        Text = "Add to Cart",
                        Location = new Point(120, 130),
                        Size = new Size(120, 30)
                    };
                    btnAddToCart.Click += (s, e) => AddToCart(item[3], txtQuantity.Text); // Pass Item_No and Quantity
                    productPanel.Controls.Add(btnAddToCart);

                    // Add the product panel to the products panel
                    products.Controls.Add(productPanel);

                    // Adjust the Y offset for the next product panel
                    yOffset += 190; // Increase the Y offset to avoid overlapping
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void CheckIngredients(string itemNo)
        {
            Form19 aras = new Form19(email, itemNo);
            this.Hide();
            aras.Show();
        }

        private void AddToCart(string itemNo, string quantityText)
        {
            if (int.TryParse(quantityText, out int quantity) && quantity > 0)
            {
                // Assuming you have a list to store cart items
                cartItems.Add(new Tuple<string, int>(itemNo, quantity)); // Add Item_No and Quantity to the cart
                MessageBox.Show($"Item {itemNo} with Quantity {quantity} added to cart!");
            }
            else
            {
                MessageBox.Show("Please enter a valid quantity.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form20 alfke = new Form20(email, cartItems);
            this.Hide();
            alfke.Show();
        }
    }
}
