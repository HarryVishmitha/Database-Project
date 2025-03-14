using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FoodHub
{
    public partial class Form23 : Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";
        private string email;
        public string Last_Name { get; set; }
        public string Empno { get; set; }
        private int orderNo; // Added to store the order number

        public Form23(string email)
        {
            InitializeComponent();
            this.email = email;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form17 jsf = new Form17(email);
            this.Hide();
            jsf.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            riderBoard jasfl = new riderBoard(email);
            this.Hide();
            jasfl.Show();
        }

        private void Form23_Load(object sender, EventArgs e)
        {
            getUserDetails();
            userName.Text = Last_Name;
            getAssignedOrder();
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
                    string getRiderDetailsQuery = @"SELECT Employee_No, First_Name, Middle_Name, Last_Name, 
                                                    Employee_NIC, Employee_DOB, Employee_Age, 
                                                    HouseNO, Address_Line1, City, Province, 
                                                    Employee_LicenseNo, Employee_PhoneNo 
                                                FROM RIDER WHERE Login_ID = @Login_ID";

                    SqlCommand getRiderDetailsCommand = new SqlCommand(getRiderDetailsQuery, conn);
                    getRiderDetailsCommand.Parameters.AddWithValue("@Login_ID", loginId);

                    using (SqlDataReader reader = getRiderDetailsCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Last_Name = reader["Last_Name"].ToString();
                            Empno = reader["Employee_No"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private void getAssignedOrder()
        {
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();

                    // SQL Query to get the assigned orders along with rider and customer details
                    string query = @"
                    SELECT 
                        r.Employee_No, 
                        r.First_Name + ' ' + r.Last_Name AS Employee_Name,
                        o.Order_No, 
                        c.First_name + ' ' + c.Last_name AS Customer_Name,
                        c.Phone_No, 
                        c.HouseNO + ' ' + c.Address_Line1 + ', ' + c.City + ', ' + c.Province AS Customer_Address,
                        o.Payment_method
                    FROM [ORDER] o
                    INNER JOIN RIDER r ON o.Employee_No = r.Employee_No
                    INNER JOIN CUSTOMER c ON o.CustomerID = c.CustomerID
                    WHERE o.Order_Status = 'Rider Assigned';";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    // Check if any data was returned
                    if (reader.HasRows)
                    {
                        // Process the results and populate the relevant controls
                        while (reader.Read())
                        {
                            orderNo = reader.GetInt32(2); // Store the Order_No for later use
                            string employeeName = reader.GetString(1);
                            string customerName = reader.GetString(3);
                            string customerPhone = reader.GetString(4);
                            string customerAddress = reader.GetString(5);
                            string paymentMethod = reader.GetString(6);

                            label10.Text = customerName;
                            label9.Text = customerPhone;
                            label8.Text = customerAddress;
                            label7.Text = paymentMethod;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No assigned orders found.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error retrieving assigned orders: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Prompt for confirmation before updating the order status
            var confirmResult = MessageBox.Show("Are you sure you want to mark this order as delivered?", "Confirm Delivery", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmResult == DialogResult.Yes)
            {
                // Call the method to update the order status to 'Delivered'
                updateOrderStatusToDelivered();
            }
        }

        private void updateOrderStatusToDelivered()
        {
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();

                    // SQL query to update the order status to 'Delivered'
                    string updateOrderStatusQuery = "UPDATE [ORDER] SET Order_Status = @Order_Status WHERE Order_No = @Order_No";
                    SqlCommand cmdUpdateStatus = new SqlCommand(updateOrderStatusQuery, conn);
                    cmdUpdateStatus.Parameters.AddWithValue("@Order_Status", "Delivered");
                    cmdUpdateStatus.Parameters.AddWithValue("@Order_No", orderNo);  // Use the stored orderNo value

                    int rowsAffected = cmdUpdateStatus.ExecuteNonQuery();  // Execute the command

                    // Check if the update was successful
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Order marked as Delivered successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Error: Order not found or status already updated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    // Show error message if something goes wrong
                    MessageBox.Show("Error updating order status: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
