using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FoodHub
{
    public partial class chkUsers : Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";
        private string email;

        public chkUsers(string email)
        {
            InitializeComponent();
            this.email = email;
        }

        private void Form9_Load(object sender, EventArgs e)
        {
            LoadAllUsers();
        }

        private void LoadAllUsers()
        {
            string customerQuery = "SELECT L.Login_ID, L.EmailAddress, L.Account_Type, C.First_name, C.Last_name, C.C_NIC FROM LOGINS L INNER JOIN CUSTOMER C ON L.Login_ID = C.Login_ID";
            string riderQuery = "SELECT L.Login_ID, L.EmailAddress, L.Account_Type, R.First_Name, R.Last_Name, R.Employee_NIC FROM LOGINS L INNER JOIN RIDER R ON L.Login_ID = R.Login_ID";
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();
                    // customers
                    DataTable customerTable = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(customerQuery, conn))
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(customerTable);
                    }

                    // riders
                    DataTable riderTable = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(riderQuery, conn))
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(riderTable);
                    }

                    // Merge data into one table
                    DataTable mergedTable = new DataTable();
                    mergedTable.Columns.Add("Login_ID", typeof(int));
                    mergedTable.Columns.Add("Email", typeof(string));
                    mergedTable.Columns.Add("Account_Type", typeof(string));
                    mergedTable.Columns.Add("FirstName", typeof(string));
                    mergedTable.Columns.Add("LastName", typeof(string));
                    mergedTable.Columns.Add("NIC", typeof(string));

                    foreach (DataRow row in customerTable.Rows)
                    {
                        mergedTable.Rows.Add(
                            row["Login_ID"],
                            row["EmailAddress"],
                            row["Account_Type"],
                            row["First_name"],
                            row["Last_name"],
                            row["C_NIC"]
                        );
                    }

                    foreach (DataRow row in riderTable.Rows)
                    {
                        mergedTable.Rows.Add(
                            row["Login_ID"],
                            row["EmailAddress"],
                            row["Account_Type"],
                            row["First_Name"],
                            row["Last_Name"],
                            row["Employee_NIC"]
                        );
                    }

                    // Bind data to DataGridView
                    dataGridView1.DataSource = mergedTable;

                    // Add Delete Button Column if not already added
                    if (dataGridView1.Columns["Delete"] == null)
                    {
                        DataGridViewButtonColumn deleteColumn = new DataGridViewButtonColumn();
                        deleteColumn.Name = "Delete";
                        deleteColumn.HeaderText = "Delete";
                        deleteColumn.Text = "Delete";
                        deleteColumn.UseColumnTextForButtonValue = true;
                        dataGridView1.Columns.Add(deleteColumn);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Handle delete button click in DataGridView
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the "Delete" button was clicked
            if (e.ColumnIndex == dataGridView1.Columns["Delete"].Index && e.RowIndex >= 0)
            {
                int loginId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Login_ID"].Value);
                string accountType = dataGridView1.Rows[e.RowIndex].Cells["Account_Type"].Value.ToString();

                DialogResult confirm = MessageBox.Show("Are you sure you want to delete this user?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    // Call method to delete user from LOGINS table
                    DeleteUser(loginId, accountType);
                }
            }
        }

        // Delete user from LOGINS table based on Login_ID and Account_Type (Customer/Rider)
        private void DeleteUser(int loginId, string accountType)
        {
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();

                    // Step 1: Define the SQL queries for deletion
                    string deleteQuery = "";

                    // Check if the account type is "Customer" or "Rider"
                    if (accountType == "user")
                    {
                        // Delete from CUSTOMER table
                        deleteQuery = "DELETE FROM CUSTOMER WHERE Login_ID = @LoginID";
                    }
                    else if (accountType == "rider")
                    {
                        // Delete from RIDER table
                        deleteQuery = "DELETE FROM RIDER WHERE Login_ID = @LoginID";
                    }
                    else
                    {
                        // If the account type is not valid, show an error and return
                        MessageBox.Show("Invalid account type. Cannot delete user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Step 2: Execute the query for the corresponding table
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoginID", loginId);
                        cmd.ExecuteNonQuery();  // Execute the delete query
                    }

                    // Step 3: Delete from the LOGINS table
                    string deleteLoginQuery = "DELETE FROM LOGINS WHERE Login_ID = @LoginID";
                    using (SqlCommand cmd = new SqlCommand(deleteLoginQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoginID", loginId);
                        cmd.ExecuteNonQuery();  // Execute the delete query for LOGINS
                    }

                    // Refresh the user list after deletion
                    LoadAllUsers();

                    MessageBox.Show("User deleted successfully!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // Handle any errors that occur during the delete process
                    MessageBox.Show("Error: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }




        private void button1_Click(object sender, EventArgs e)
        {
            adminPanel ad = new adminPanel(email);
            this.Hide();
            ad.Show();
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the "Delete" button was clicked
            if (e.ColumnIndex == dataGridView1.Columns["Delete"].Index && e.RowIndex >= 0)
            {
                int loginId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Login_ID"].Value);
                string accountType = dataGridView1.Rows[e.RowIndex].Cells["Account_Type"].Value.ToString();

                DialogResult confirm = MessageBox.Show("Are you sure you want to delete this user?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    // Call method to delete user from LOGINS table
                    DeleteUser(loginId, accountType);
                }
            }
        }
    }
}
