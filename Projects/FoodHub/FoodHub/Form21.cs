using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace FoodHub
{
    public partial class Form21 : Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";
        private string email;
        private int customerId;

        public string Last_Name { get; set; }

        public Form21(string email)
        {
            InitializeComponent();
            this.email = email;
        }

        private void Form21_Load(object sender, EventArgs e)
        {
            getUserDetails();
            userName.Text = Last_Name;
            LoadUserOrders();
        }

        private void getUserDetails()
        {
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();

                    string getLoginIdQuery = "SELECT Login_ID FROM LOGINS WHERE EmailAddress = @Email";
                    SqlCommand getLoginIdCommand = new SqlCommand(getLoginIdQuery, conn);
                    getLoginIdCommand.Parameters.AddWithValue("@Email", email);

                    int loginId = Convert.ToInt32(getLoginIdCommand.ExecuteScalar());
 
                    string getCustomerIdQuery = "SELECT CustomerID FROM CUSTOMER WHERE Login_ID = @Login_ID";
                    SqlCommand getCustomerIdCommand = new SqlCommand(getCustomerIdQuery, conn);
                    getCustomerIdCommand.Parameters.AddWithValue("@Login_ID", loginId);

                    customerId = Convert.ToInt32(getCustomerIdCommand.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching customer ID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadUserOrders()
        {
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();
                    string getOrdersQuery = @"SELECT Order_No, Order_Timestamp, Order_Total, Order_Status 
                                              FROM [ORDER] 
                                              WHERE CustomerID = @CustomerID AND Order_Status = 'New'";

                    SqlCommand getOrdersCmd = new SqlCommand(getOrdersQuery, conn);
                    getOrdersCmd.Parameters.AddWithValue("@CustomerID", customerId);

                    using (SqlDataReader reader = getOrdersCmd.ExecuteReader())
                    {
                        DataTable ordersTable = new DataTable();
                        ordersTable.Load(reader);
                        dataGridView1.DataSource = ordersTable;

                        DataGridViewButtonColumn discardButtonColumn = new DataGridViewButtonColumn();
                        discardButtonColumn.HeaderText = "Discard Order";
                        discardButtonColumn.Text = "Discard";
                        discardButtonColumn.UseColumnTextForButtonValue = true;
                        dataGridView1.Columns.Add(discardButtonColumn);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == dataGridView1.Columns.Count - 1 && e.RowIndex >= 0)
            {
                int orderNo = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Order_No"].Value);
                string orderStatus = dataGridView1.Rows[e.RowIndex].Cells["Order_Status"].Value.ToString();

                if (orderStatus == "New")
                {
                    
                    var confirmResult = MessageBox.Show("Are you sure you want to discard this order?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirmResult == DialogResult.Yes)
                    {
                        DeleteOrder(orderNo);
                        MessageBox.Show("Order discarded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUserOrders();
                    }
                }
                else
                {
                    MessageBox.Show("You can only discard orders with status 'New'.", "Cannot Discard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void DeleteOrder(int orderNo)
        {
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();

                    SqlTransaction transaction = conn.BeginTransaction();

                    string deleteOrderItemQuery = "DELETE FROM ORDER_ITEM WHERE Order_No = @Order_No";
                    using (SqlCommand deleteItemCmd = new SqlCommand(deleteOrderItemQuery, conn, transaction))
                    {
                        deleteItemCmd.Parameters.AddWithValue("@Order_No", orderNo);
                        deleteItemCmd.ExecuteNonQuery();
                    }

                    string deleteOrderQuery = "DELETE FROM [ORDER] WHERE Order_No = @Order_No";
                    using (SqlCommand deleteOrderCmd = new SqlCommand(deleteOrderQuery, conn, transaction))
                    {
                        deleteOrderCmd.Parameters.AddWithValue("@Order_No", orderNo);
                        deleteOrderCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form18 productForm = new Form18(email);
            this.Hide();
            productForm.Show();
        }
    }
}
