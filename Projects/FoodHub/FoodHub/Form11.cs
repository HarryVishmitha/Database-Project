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
    public partial class Form11: Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";
        private string email;
        public Form11(string email)
        {
            InitializeComponent();
            this.email = email;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form11 hgjsd = new Form11(email);
            this.Hide();
            hgjsd.Show();
        }
        private void getIngredients()
        {
            string queryGetAllIngredients = "SELECT ING_ID, ING_Name, ING_Description FROM INGREDIENTS";
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            // Add columns to the DataGridView if not added already
            if (dataGridView1.Columns.Count == 0)
            {
                dataGridView1.Columns.Add("ING_ID", "ID");
                dataGridView1.Columns.Add("ING_Name", "Name");
                dataGridView1.Columns.Add("ING_Description", "Description");
                DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
                deleteButtonColumn.Name = "Delete";
                deleteButtonColumn.Text = "Delete";
                deleteButtonColumn.UseColumnTextForButtonValue = true;
                dataGridView1.Columns.Add(deleteButtonColumn);
            }

            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(queryGetAllIngredients, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            int rowCount = 0; // Add a counter for rows to check if any rows are being added

                            while (reader.Read())
                            {
                                // Add a new row to the DataGridView
                                dataGridView1.Rows.Add(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), "Delete");
                                rowCount++;
                            }

                            // Debug: Print the number of rows added
                            Console.WriteLine("Rows added: " + rowCount);

                            // If no rows were added, show a message
                            if (rowCount == 0)
                            {
                                MessageBox.Show("No ingredients found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
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


        //private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.ColumnIndex == dataGridView1.Columns["Delete"].Index && e.RowIndex >= 0)
        //    {
        //        int selectedIngId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);

        //        var confirmResult = MessageBox.Show("Are you sure you want to delete this ingredient?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        //        if (confirmResult == DialogResult.Yes)
        //        {
        //            deleteIngredientFromDatabase(selectedIngId);
        //            dataGridView1.Rows.RemoveAt(e.RowIndex);
        //        }
        //    }
        //}
        private void deleteIngredientFromDatabase(int ingredientId)
        {
            string queryDelete = "DELETE FROM INGREDIENTS WHERE ING_ID = @ingredientId";

            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(queryDelete, conn))
                    {
                        cmd.Parameters.AddWithValue("@ingredientId", ingredientId);

                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            MessageBox.Show("Ingredient deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Error deleting ingredient.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void addDeleteButtonColumn()
        {
            DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
            deleteButtonColumn.Name = "Delete";
            deleteButtonColumn.Text = "Delete";
            deleteButtonColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(deleteButtonColumn);
        }

        private void Form11_Load(object sender, EventArgs e)
        {
            addDeleteButtonColumn();
            getIngredients();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            adminPanel admin = new adminPanel(email);
            admin.Show();
            this.Hide();
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Delete"].Index && e.RowIndex >= 0)
            {
                int selectedIngId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);

                var confirmResult = MessageBox.Show("Are you sure you want to delete this ingredient?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirmResult == DialogResult.Yes)
                {
                    deleteIngredientFromDatabase(selectedIngId);
                    dataGridView1.Rows.RemoveAt(e.RowIndex);
                }
            }
        }
    }
}
