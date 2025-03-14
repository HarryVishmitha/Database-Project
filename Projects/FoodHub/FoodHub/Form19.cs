using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FoodHub
{
    public partial class Form19: Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";

        private string email;
        private string itemNo;
        public Form19(string email, string itemNo)
        {
            InitializeComponent();
            this.email = email;
            this.itemNo = itemNo;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form18 jakae = new Form18(email);
            this.Hide();
            jakae.Show();
        }

        private void getitemDetails()
        {
            if (itemNo != null)
            {
                string query1 = "SELECT ITEM.Item_Name, ITEM.Item_Category, ITEM.Item_Price, " +
                                "INGREDIENTS.ING_Name, INGREDIENTS.ING_Description " +
                                "FROM ITEM " +
                                "INNER JOIN ITEMINGREDIENTS ON ITEM.Item_No = ITEMINGREDIENTS.Item_No " +
                                "INNER JOIN INGREDIENTS ON ITEMINGREDIENTS.ING_ID = INGREDIENTS.ING_ID " +
                                "WHERE ITEM.Item_No = @itemNo";

                using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query1, conn))
                        {
                            cmd.Parameters.AddWithValue("@itemNo", itemNo);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    string ingredientsList = "";

                                    // Loop through each row for ingredients
                                    while (reader.Read())
                                    {
                                        // Populate item details
                                        nameP.Text = reader["Item_Name"].ToString();
                                        priceP.Text = reader["Item_Price"].ToString();
                                        catP.Text = reader["Item_Category"].ToString();

                                        // Concatenate each ingredient
                                        string ingredientName = reader["ING_Name"].ToString();
                                        string ingredientDescription = reader["ING_Description"].ToString();

                                        ingredientsList += $"Ingredient: {ingredientName}\nDescription: {ingredientDescription}\n\n";
                                    }

                                    // Set the ingredients list to the label or TextBox
                                    IngPa.Text = ingredientsList;
                                }
                                else
                                {
                                    MessageBox.Show("No ingredients found for this product.");
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
            else
            {
                MessageBox.Show("Error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Form19_Load(object sender, EventArgs e)
        {
            getitemDetails();
        }
    }
}
