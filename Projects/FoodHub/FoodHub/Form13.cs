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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace FoodHub
{
    public partial class Form13: Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";
        private string email;
        public Form13(string email)
        {
            InitializeComponent();
            this.email = email;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            adminPanel admin = new adminPanel(email);
            admin.Show();
            this.Hide();
        }

        private void getFoodItems()
        {
            string queryGetAllFoodItems = "SELECT Item_No, Item_Name, Item_Category, Item_Price FROM ITEM";

            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(queryGetAllFoodItems, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader); // Load the result into a DataTable
                            dataGridView1.DataSource = dt; // Bind the DataTable to DataGridView
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void deleteFoodItem(int itemId)
        {
            string queryDeleteFoodItem = "DELETE FROM ITEM WHERE Item_No = @ItemID";
            string queryDeleteFoodIngredients = "DELETE FROM ITEMINGREDIENTS WHERE Item_No = @ItemID";  // Remove associated ingredients

            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();

                    // First, delete from ITEMINGREDIENTS to avoid foreign key constraint errors
                    using (SqlCommand cmd = new SqlCommand(queryDeleteFoodIngredients, conn))
                    {
                        cmd.Parameters.AddWithValue("@ItemID", itemId);
                        cmd.ExecuteNonQuery();
                    }

                    // Now delete from ITEM
                    using (SqlCommand cmd = new SqlCommand(queryDeleteFoodItem, conn))
                    {
                        cmd.Parameters.AddWithValue("@ItemID", itemId);
                        cmd.ExecuteNonQuery();
                    }

                    // Remove from DataGridView after deletion
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["Item_No"].Value != null && (int)row.Cells["Item_No"].Value == itemId)
                        {
                            dataGridView1.Rows.Remove(row);
                            break;
                        }
                    }

                    MessageBox.Show("Food item deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void setupDataGridView()
        {
            // Add a Delete button column to the DataGridView
            DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
            deleteButtonColumn.HeaderText = "Delete";
            deleteButtonColumn.Text = "Delete";
            deleteButtonColumn.Name = "btnDelete";
            deleteButtonColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(deleteButtonColumn);

            // Add event handler for the delete button click
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is in the Delete button column
            if (e.ColumnIndex == dataGridView1.Columns["btnDelete"].Index && e.RowIndex >= 0)
            {
                // Get the item ID of the selected food item
                int itemId = (int)dataGridView1.Rows[e.RowIndex].Cells["Item_No"].Value;
                // Call delete function
                deleteFoodItem(itemId);
            }
        }

        private void Form13_Load(object sender, EventArgs e)
        {
            getFoodItems();
            setupDataGridView();
        }
    }
}
