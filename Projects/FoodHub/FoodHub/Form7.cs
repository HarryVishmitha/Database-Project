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

namespace FoodHub
{
    public partial class CheckOrders: Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";

        private string email;
        public CheckOrders(string email)
        {
            InitializeComponent();
            this.email = email;
        }

        private void CheckOrders_Load(object sender, EventArgs e)
        {
            string query = @"SELECT Order_No, CustomerID, Order_Timestamp, Payment_method, Order_Status, Order_Total FROM [ORDER] WHERE Order_Status = 'New';";
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;
                    // Add "Assign Rider" and "Remove Order" buttons to the DataGridView
                    DataGridViewButtonColumn btnAssignRider = new DataGridViewButtonColumn();
                    btnAssignRider.Name = "btnAssignRider";
                    btnAssignRider.Text = "Assign Rider";
                    btnAssignRider.UseColumnTextForButtonValue = true;
                    dataGridView1.Columns.Add(btnAssignRider);

                    DataGridViewButtonColumn btnRemoveOrder = new DataGridViewButtonColumn();
                    btnRemoveOrder.Name = "btnRemoveOrder";
                    btnRemoveOrder.Text = "Remove Order";
                    btnRemoveOrder.UseColumnTextForButtonValue = true;
                    dataGridView1.Columns.Add(btnRemoveOrder);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void selectDataToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["btnAssignRider"].Index && e.RowIndex >= 0)
            {
                int orderNo = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Order_No"].Value);
                string orderStatus = dataGridView1.Rows[e.RowIndex].Cells["Order_Status"].Value.ToString();

               
                if (orderStatus == "New")
                {
                    Form22 jskds = new Form22(email, orderNo);
                    this.Hide();
                    jskds.Show();
                }
                else
                {
                    MessageBox.Show("This order has already been processed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            // Handle "Remove Order" button click
            if (e.ColumnIndex == dataGridView1.Columns["btnRemoveOrder"].Index && e.RowIndex >= 0)
            {
                int orderNo = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Order_No"].Value);
                string orderStatus = dataGridView1.Rows[e.RowIndex].Cells["Order_Status"].Value.ToString();

                // Confirm removal if order is still 'New'
                if (orderStatus == "New")
                {
                    var confirmResult = MessageBox.Show("Are you sure you want to remove this order?", "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirmResult == DialogResult.Yes)
                    {
                        RemoveOrder(orderNo);
                    }
                }
                else
                {
                    MessageBox.Show("This order cannot be removed because it's no longer 'New'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            adminPanel adm = new adminPanel(email);
            adm.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            adminPanel admin = new adminPanel(email);
            admin.Show();
            this.Hide();
        }

        private void RemoveOrder(int orderNo)
        {
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE [ORDER] SET Order_Status = @Order_Status WHERE Order_No = @Order_No";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Order_Status", "Rejected");
                    cmd.Parameters.AddWithValue("@Order_No", orderNo);
                    cmd.ExecuteNonQuery();

                    CheckOrders_Load(null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating order status: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



    }
}
