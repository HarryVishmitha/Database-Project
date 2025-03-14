using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FoodHub
{
    public partial class Form15 : Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";
        private string email;

        public Form15(string email)
        {
            InitializeComponent();
            this.email = email;
        }

        private void Form15_Load(object sender, EventArgs e)
        {
            setupDataGridView();  // Setup the DataGridView
            getBikes();  // Fetch bikes
        }

        private void getBikes()
        {
            // SQL query to fetch bikes
            string queryGetAllBikes = "SELECT VReg_No, Brand, Model, Engine_No, VRDate, Color_theme FROM BIKE";

            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(queryGetAllBikes, conn))
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

        private void deleteBike(int bikeId)
        {
            // SQL query to delete a bike record
            string queryDeleteBike = "DELETE FROM BIKE WHERE VReg_No = @BikeID";

            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();

                    // Now delete the bike from the BIKE table
                    using (SqlCommand cmd = new SqlCommand(queryDeleteBike, conn))
                    {
                        cmd.Parameters.AddWithValue("@BikeID", bikeId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // After deletion, update the DataGridView
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                if (row.Cells["VReg_No"].Value != null && (int)row.Cells["VReg_No"].Value == bikeId)
                                {
                                    dataGridView1.Rows.Remove(row);
                                    break;
                                }
                            }

                            MessageBox.Show("Bike deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No bike found with the specified ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
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
                // Get the bike ID of the selected bike
                if (dataGridView1.Rows[e.RowIndex].Cells["VReg_No"].Value != null && dataGridView1.Rows[e.RowIndex].Cells["VReg_No"].Value is int bikeId)
                {
                    // Call delete function
                    deleteBike(bikeId);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            adminPanel admin = new adminPanel(email);
            admin.Show();
            this.Hide();
        }
    }
}
