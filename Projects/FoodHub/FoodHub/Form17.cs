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
    public partial class Form17: Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";

        private string email;
        public string Last_Name { get; set; }
        public string Empno { get; set; }
        public Form17(string email)
        {
            InitializeComponent();
            this.email = email;
        }

        private void label21_Click(object sender, EventArgs e)
        {

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
                            label12.Text = reader["First_Name"].ToString();
                            label16.Text = reader["Middle_Name"].ToString();
                            label20.Text = reader["Last_Name"].ToString();
                            label21.Text = reader["Employee_NIC"].ToString();
                            label22.Text = email.ToString();
                            houseNo.Text = reader["HouseNO"].ToString();
                            addressl1.Text = reader["Address_Line1"].ToString();
                            city.Text = reader["City"].ToString();
                            province.Text = reader["Province"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private void Form17_Load(object sender, EventArgs e)
        {
            getUserDetails();
            getDependent();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form17 jsf = new Form17(email);
            this.Hide();
            jsf.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string pwd = password.Text;
            if (!string.IsNullOrEmpty(pwd))
            {
                using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
                {
                    try
                    {

                        // Step 2: Update the password for the corresponding Login_ID
                        string updatePasswordQuery = "UPDATE LOGINS SET Password = @Password WHERE EmailAddress = @Login_ID";
                        SqlCommand updatePasswordCommand = new SqlCommand(updatePasswordQuery, conn);
                        updatePasswordCommand.Parameters.AddWithValue("@Password", pwd);
                        updatePasswordCommand.Parameters.AddWithValue("@Login_ID", email);

                        int rowsAffected = updatePasswordCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Password updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Error updating password. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            string hnO = houseNo.Text;
            string ad1 = addressl1.Text;
            string ci = city.Text;
            string pro = province.SelectedItem.ToString();

            if (string.IsNullOrEmpty(hnO) || string.IsNullOrEmpty(ci) || string.IsNullOrEmpty(pro))
            {
                MessageBox.Show("Please fill all inputs", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                UpdateAddressInDatabase(hnO, ad1, ci, pro);
            }
        }

        private void UpdateAddressInDatabase(string houseNo, string addressLine1, string city, string province)
        {
            
            string employeeNo = Empno;

            string updateQuery = @"UPDATE RIDER
                                   SET HouseNO = @HouseNo, Address_Line1 = @AddressLine1, City = @City, Province = @Province
                                   WHERE Employee_No = @EmployeeNo";

            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        // Adding parameters to prevent SQL injection
                        cmd.Parameters.AddWithValue("@HouseNo", houseNo);
                        cmd.Parameters.AddWithValue("@AddressLine1", addressLine1);
                        cmd.Parameters.AddWithValue("@City", city);
                        cmd.Parameters.AddWithValue("@Province", province);
                        cmd.Parameters.AddWithValue("@EmployeeNo", employeeNo);

                        // Execute the query
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Address updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No record found to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating address: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string dependentName = textBox1.Text;
            string relationship = textBox2.Text;
            string dob = dateTimePicker1.Value.ToString("yyyy-MM-dd");

            if (string.IsNullOrEmpty(dependentName) || string.IsNullOrEmpty(relationship) || string.IsNullOrEmpty(dob))
            {
                MessageBox.Show("Please fill in all fields", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                string query = @"SELECT COUNT(*) FROM DEPENDENTS WHERE Employee_No = @empNoa";
                using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
                {
                    try
                    {
                        conn.Open();

                        // Check if dependent already exists
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@empNoa", Empno);
                            int count = (int)cmd.ExecuteScalar();

                            if (count > 0)
                            {
                                // Update dependent
                                string queryup = @"UPDATE DEPENDENTS SET Dependent_Name = @DependentName, Relationship = @Relationship, Dependent_DOB = @DOB WHERE Employee_No = @empNoa";
                                using (SqlCommand cmd2 = new SqlCommand(queryup, conn))
                                {
                                    cmd2.Parameters.AddWithValue("@Relationship", relationship);
                                    cmd2.Parameters.AddWithValue("@DOB", dob);
                                    cmd2.Parameters.AddWithValue("@empNoa", Empno);
                                    cmd2.Parameters.AddWithValue("@DependentName", dependentName);

                                    int rowsAffected = cmd2.ExecuteNonQuery();
                                    if (rowsAffected > 0)
                                    {
                                        MessageBox.Show("Dependent updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Failed to update dependent.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            else
                            {
                                // Add new dependent
                                string queryIN = @"INSERT INTO DEPENDENTS (Dependent_Name, Relationship, Dependent_DOB, Employee_No) 
                                           VALUES (@DependentName, @Relationship, @DOB, @empnoa)";
                                using (SqlCommand cmd3 = new SqlCommand(queryIN, conn))
                                {
                                    cmd3.Parameters.AddWithValue("@DependentName", dependentName);
                                    cmd3.Parameters.AddWithValue("@Relationship", relationship);
                                    cmd3.Parameters.AddWithValue("@DOB", dob);
                                    cmd3.Parameters.AddWithValue("@empNoa", Empno);

                                    int rowsAffected = cmd3.ExecuteNonQuery();
                                    if (rowsAffected > 0)
                                    {
                                        MessageBox.Show("Dependent added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Failed to add dependent.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error handling dependent: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void getDependent()
        {
            string query = @"SELECT Dependent_Name, Relationship, Dependent_DOB 
                     FROM DEPENDENTS 
                     WHERE Employee_No = @empNoa";
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@empNoa", Empno);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Assuming you have labels or textboxes to display the dependent details
                                textBox1.Text = reader["Dependent_Name"].ToString();
                                textBox2.Text = reader["Relationship"].ToString();
                                dateTimePicker1.Text = Convert.ToDateTime(reader["Dependent_DOB"]).ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                MessageBox.Show("No dependent found for this employee.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching dependent details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}
