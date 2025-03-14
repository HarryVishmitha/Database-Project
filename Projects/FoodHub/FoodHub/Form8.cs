using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace FoodHub
{
    public partial class addnewUser: Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";

        private string emaila;
        private string selectedItem;
        public addnewUser(string emaila)
        {
            InitializeComponent();
            this.emaila = emaila;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.selectedItem = comboBox1.SelectedItem.ToString();
            if (selectedItem != null)
            {
                calluserSpecs(selectedItem);
                LoadAllUsers(selectedItem);
            }
            else
            {
                MessageBox.Show("Select the user type to add new user", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void calluserSpecs(string selectedItem)
        {
            if (selectedItem == "Rider")
            {
                fName.Visible = true;
                lName.Visible = true;
                textBox2.Visible = true;
                nic.Visible = true;
                email.Visible = true;
                houseNo.Visible = true;
                addressl1.Visible = true;
                password.Visible = true;
                city.Visible = true;
                province.Visible = true;
                textBox1.Visible = true;
                phoneNu.Visible = true;
                dateTimePicker1.Visible = true;
            }
            else if (selectedItem == "Customer")
            {
                fName.Visible = true;
                lName.Visible = false;
                textBox2.Visible = true;
                nic.Visible = true;
                email.Visible = true;
                houseNo.Visible = true;
                addressl1.Visible = true;
                password.Visible = true;
                city.Visible = true;
                province.Visible = true;
                textBox1.Visible = false;
                phoneNu.Visible = true;
                dateTimePicker1.Visible = true;
            }
            else
            {
                fName.Visible = false;
                lName.Visible = false;
                textBox2.Visible = false;
                nic.Visible = false;
                email.Visible = true;
                houseNo.Visible = false;
                addressl1.Visible = false;
                password.Visible = false;
                city.Visible = false;
                province.Visible = false;
                textBox1.Visible = false;
                phoneNu.Visible = false;
                dateTimePicker1.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            adminPanel admin = new adminPanel(emaila);
            admin.Show();
            this.Hide();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null)
            {
                // Get the selected user's full name
                string selectedUser = comboBox2.SelectedItem.ToString();

                // Fetch and display user details based on the selected name
                GetUserDetails(selectedUser);
            }
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            LoadAllUsers(selectedItem);
        }
        private void LoadAllUsers(string selectedItem)
        {
            string customerQuery = "SELECT CustomerID, First_name, Last_name FROM CUSTOMER";
            string riderQuery = "SELECT Employee_No, First_name, Last_name FROM RIDER";
            List<string> allUsers = new List<string>();

            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();
                    string query = "";

                    if (selectedItem == "Rider")
                    {
                        query = riderQuery;
                    }
                    else if (selectedItem == "Customer")
                    {
                        query = customerQuery;
                    }
                    else
                    {
                        return; // Exit if no valid selection
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string fullName = reader["First_name"].ToString() + " " + reader["Last_name"].ToString();
                                allUsers.Add(fullName);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            comboBox2.DataSource = allUsers;
            comboBox2.SelectedIndex = -1;
        }

        //if one of users selected in combobox2 perform updating it
        public void updateUser(string actype, string username)
        {
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();

                    // Split the full name to first and last names
                    string[] nameParts = username.Split(' '); // Assuming the name format is "FirstName LastName"

                    string queryUpdateCustomer = "";
                    string queryUpdateRider = "";

                    if (actype == "Customer")
                    {
                        // Update query for Customer
                        queryUpdateCustomer = "UPDATE CUSTOMER SET First_name = @firstName, Last_name = @lastName, " +
                                              "C_NIC = @nic, Phone_No = @phoneno, HouseNO = @houseno, Address_Line1 = @addressl1, " +
                                              "City = @city, Province = @province WHERE First_name = @oldFirstName AND Last_name = @oldLastName";
                    }
                    else if (actype == "Rider")
                    {
                        // Update query for Rider
                        queryUpdateRider = "UPDATE RIDER SET First_Name = @firstName, Middle_Name = @mname, Last_Name = @lastName, " +
                                           "Employee_NIC = @empnic, Employee_PhoneNo = @empphone, Employee_LicenseNo = @emplicense, " +
                                           "Employee_DOB = @empdob, Employee_Age = @empage, HouseNO = @emphouse, Address_Line1 = @empaddress, " +
                                           "City = @empcity, Province = @empprovince WHERE First_Name = @oldFirstName AND Last_Name = @oldLastName";
                    }

                    // Update the Customer Table
                    if (actype == "Customer")
                    {
                        using (SqlCommand cmd = new SqlCommand(queryUpdateCustomer, conn))
                        {
                            cmd.Parameters.AddWithValue("@firstName", fName.Text);
                            cmd.Parameters.AddWithValue("@lastName", textBox2.Text);
                            cmd.Parameters.AddWithValue("@nic", nic.Text);
                            cmd.Parameters.AddWithValue("@phoneno", phoneNu.Text);
                            cmd.Parameters.AddWithValue("@houseno", houseNo.Text);
                            cmd.Parameters.AddWithValue("@addressl1", addressl1.Text);
                            cmd.Parameters.AddWithValue("@city", city.Text);
                            cmd.Parameters.AddWithValue("@province", province.Text);
                            cmd.Parameters.AddWithValue("@oldFirstName", nameParts[0]);
                            cmd.Parameters.AddWithValue("@oldLastName", nameParts[1]);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Customer details updated successfully!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Error saving customer details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }

                    // Update the Rider Table
                    if (actype == "Rider")
                    {
                        using (SqlCommand cmd = new SqlCommand(queryUpdateRider, conn))
                        {
                            cmd.Parameters.AddWithValue("@firstName", fName.Text);
                            cmd.Parameters.AddWithValue("@mname", lName.Text);  // Middle Name
                            cmd.Parameters.AddWithValue("@lastName", textBox2.Text);
                            cmd.Parameters.AddWithValue("@empnic", nic.Text);
                            cmd.Parameters.AddWithValue("@empphone", phoneNu.Text);
                            cmd.Parameters.AddWithValue("@emplicense", textBox1.Text); // License Number
                            cmd.Parameters.AddWithValue("@empdob", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("@empage", DateTime.Now.Year - dateTimePicker1.Value.Year);
                            cmd.Parameters.AddWithValue("@emphouse", houseNo.Text);
                            cmd.Parameters.AddWithValue("@empaddress", addressl1.Text);
                            cmd.Parameters.AddWithValue("@empcity", city.Text);
                            cmd.Parameters.AddWithValue("@empprovince", province.Text);
                            cmd.Parameters.AddWithValue("@oldFirstName", nameParts[0]);
                            cmd.Parameters.AddWithValue("@oldLastName", nameParts[1]);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Rider details updated successfully!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Error saving rider details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        //if no user selected in combobox 2 perform insert new user to database
        public void regUser(string actype)
        {
            string fName1 = fName.Text;
            string lName1 = lName.Text;
            string textBox21 = textBox2.Text;
            string nic1 = nic.Text;
            string email1 = email.Text;
            string houseNo1 = houseNo.Text;
            string addressl11 = addressl1.Text;
            string password1 = password.Text;
            string city1 = city.Text;
            string province1 = province.Text;
            string textBox11 = textBox1.Text;
            string phoneNu1 = phoneNu.Text;
            string dateTimePicker11 = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();
                    int age = DateTime.Now.Year - dateTimePicker1.Value.Year;
                    if (DateTime.Now < dateTimePicker1.Value.AddYears(age)) // Adjust if birthday hasn't occurred yet
                    {
                        age--;
                    }

                    // Step 2: Validate Age (Rider must be at least 18)
                    if (age < 18 && selectedItem == "Rider")
                    {
                        MessageBox.Show("Rider must be at least 18 years old.", "Age Restriction", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // Exit function without inserting
                    }

                    string queryIL1 = "INSERT INTO LOGINS (EmailAddress, Password, Account_Type) VALUES (@email, @password, @actype)";
                    string queryIL2 = "INSERT INTO CUSTOMER (First_name, Last_name, C_DOB, Phone_No, C_NIC, HouseNO, Address_Line1, City, Province, Login_ID)" +
                        "VALUES (@firstName, @lastName, @DOB, @phoneno, @nic, @houseno, @addressl1, @city, @province, @loginId)";
                    string queryIL3 = "SELECT Login_ID FROM LOGINS WHERE EmailAddress = @emailas";
                    string queryIL4 = "INSERT INTO RIDER (First_Name, Middle_Name, Last_Name, Employee_NIC, Employee_DOB, Employee_Age, Login_ID, HouseNO, Address_Line1, City, Province, Employee_LicenseNo, Employee_PhoneNo)" +
                        "VALUES(@faname, @mname, @liname, @empnic, @empdob, @empage, @emploginId, @emphouse, @empaddress, @empcity, @empprovince, @emplicense, @empphone)";
                    
                    using(SqlCommand cmd = new SqlCommand(queryIL1, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email1);
                        cmd.Parameters.AddWithValue("@password", password1);
                        if (actype == "Customer")
                        {
                            string actype12 = "user";
                            cmd.Parameters.AddWithValue("@actype", actype12);
                        }
                        if (actype == "Rider")
                        {
                            string actype12 = "rider";
                            cmd.Parameters.AddWithValue("@actype", actype12);
                        }
                        cmd.ExecuteNonQuery();

                        int loginId;
                        using(SqlCommand cmd2 = new SqlCommand(queryIL3, conn))
                        {
                            cmd2.Parameters.AddWithValue("@emailas", email1);
                            loginId = (int)cmd2.ExecuteScalar();
                        }

                        if (actype == "Customer")
                        {
                            using (SqlCommand cmd2 = new SqlCommand(queryIL2, conn))
                            {
                                string actype12 = "user";
                                        
                                cmd2.Parameters.AddWithValue("@firstName", fName1);
                                cmd2.Parameters.AddWithValue("@lastName", textBox21);
                                cmd2.Parameters.AddWithValue("@DOB", dateTimePicker11);
                                cmd2.Parameters.AddWithValue("@phoneno", phoneNu1);
                                cmd2.Parameters.AddWithValue("@nic", nic1);
                                cmd2.Parameters.AddWithValue("@houseno", houseNo1);
                                cmd2.Parameters.AddWithValue("@addressl1", addressl11);
                                cmd2.Parameters.AddWithValue("@city", city1);
                                cmd2.Parameters.AddWithValue("@province", province1);
                                cmd2.Parameters.AddWithValue("@loginId", loginId);
                                cmd2.Parameters.AddWithValue("@actype", actype12);

                                int rowsAffected = cmd2.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("A new user added successfully!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Error saving user information. Maybe user partially added to the system. Please check manually.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else if (actype == "Rider")
                        {
                            using (SqlCommand cmd2 = new SqlCommand(queryIL4, conn))
                            {
                                string actype12 = "rider";

                                cmd2.Parameters.AddWithValue("@faname", fName1);
                                cmd2.Parameters.AddWithValue("@mname", lName1);
                                cmd2.Parameters.AddWithValue("@liname", textBox21);
                                cmd2.Parameters.AddWithValue("@empnic", nic1);
                                cmd2.Parameters.AddWithValue("@empdob", dateTimePicker11);
                                cmd2.Parameters.AddWithValue("@empage", age);
                                cmd2.Parameters.AddWithValue("@emploginId", loginId);
                                cmd2.Parameters.AddWithValue("@emphouse", houseNo1);
                                cmd2.Parameters.AddWithValue("@empaddress", addressl11);
                                cmd2.Parameters.AddWithValue("@empcity", city1);
                                cmd2.Parameters.AddWithValue("@empprovince", province1);
                                cmd2.Parameters.AddWithValue("@emplicense", textBox11);
                                cmd2.Parameters.AddWithValue("@empphone", phoneNu1);

                                cmd2.Parameters.AddWithValue("@actype", actype12);
                                int rowsAffected = cmd2.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("A new user added successfully!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Error saving user information. Maybe user partially added to the system. Please check manually.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
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

        private void button5_Click(object sender, EventArgs e)
        {
            string cmb1 = comboBox1.SelectedItem.ToString();
            string cmb2 = comboBox2.SelectedItem?.ToString();
            if (cmb1 != null && cmb2 == null)
            {
                regUser(cmb1);
            }
            else if (cmb1 != null && cmb2 != null)
            {
                updateUser(cmb1, cmb2);
            }
            else
            {
                MessageBox.Show("Error: Undefined! Contact developers!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GetUserDetails(string selectedUser)
        {
            if (comboBox2.SelectedItem != null && !string.IsNullOrEmpty(selectedUser))
            {
                using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "";
                        string[] nameParts = selectedUser.Split(' ');

                        if (selectedItem == "Rider")
                        {
                            query = @"SELECT R.*, L.EmailAddress, L.Password FROM RIDER R JOIN LOGINS L ON R.Login_ID = L.Login_ID WHERE R.First_Name = @firstName AND R.Last_Name = @lastName";
                        }
                        else if (selectedItem == "Customer")
                        {
                            query = @"SELECT C.*, L.EmailAddress, L.Password FROM CUSTOMER C JOIN LOGINS L ON C.Login_ID = L.Login_ID WHERE C.First_name = @firstName AND C.Last_name = @lastName";
                        }

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@firstName", nameParts[0]);
                            cmd.Parameters.AddWithValue("@lastName", nameParts[1]);

                            SqlDataReader reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                if (selectedItem == "Rider")
                                {
                                    // Rider-specific fields
                                    lName.Text = reader["Middle_Name"].ToString();  // Middle Name for Rider
                                    textBox1.Text = reader["Employee_LicenseNo"].ToString(); // License No for Rider
                                    phoneNu.Text = reader["Employee_PhoneNo"].ToString();
                                    nic.Text = reader["Employee_NIC"].ToString();
                                    dateTimePicker1.Value = Convert.ToDateTime(reader["Employee_DOB"]);
                                }
                                else
                                {
                                    nic.Text = reader["C_NIC"].ToString();
                                    phoneNu.Text = reader["Phone_No"].ToString();
                                    dateTimePicker1.Value = Convert.ToDateTime(reader["C_DOB"]);
                                }
                                // Populate the form fields with current user data
                                fName.Text = reader["First_name"].ToString();
                                textBox2.Text = reader["Last_name"].ToString();
                                email.Text = reader["EmailAddress"].ToString();
                                password.Text = reader["Password"].ToString(); // Fetch password
                                houseNo.Text = reader["HouseNO"].ToString();
                                addressl1.Text = reader["Address_Line1"].ToString();
                                city.Text = reader["City"].ToString();
                                province.Text = reader["Province"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }



    }
}
