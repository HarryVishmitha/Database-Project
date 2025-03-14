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
    public partial class Form16 : Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";

        private string email;
        public string Last_Name { get; set; }
        public string Empno { get; set; }
        public Form16(string email)
        {
            InitializeComponent();
            this.email = email;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            riderBoard jasfl = new riderBoard(email);
            this.Hide();
            jasfl.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedValue != null && int.TryParse(comboBox2.SelectedValue.ToString(), out int selectedVRegNo))
            {
                // Check if the bike is already assigned to another rider
                string checkAssignmentQuery = @"SELECT COUNT(*) 
                                                FROM BIKE_ASSIGNMENT 
                                                WHERE VReg_No = @VRegNo AND End_meter IS NULL";

                using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand checkCmd = new SqlCommand(checkAssignmentQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@VRegNo", selectedVRegNo);

                            int assignmentCount = (int)checkCmd.ExecuteScalar();

                            if (assignmentCount > 0)
                            {
                                MessageBox.Show("This bike is already assigned to another rider. Please ensure the previous rider ends their day.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                // Get the start meter reading from the fName TextBox
                                if (int.TryParse(fName.Text, out int startMeterReading))
                                {
                                    // SQL query to insert a new record into the BIKE_ASSIGNMENT table
                                    string query = @"INSERT INTO BIKE_ASSIGNMENT (Employee_No, VReg_No, Start_meter, Assignment_Date) 
                                                    VALUES (@AssignedTo, @VRegNo, @StartMeter, @StartTime)";

                                    using (SqlCommand cmd = new SqlCommand(query, conn))
                                    {
                                        cmd.Parameters.AddWithValue("@VRegNo", selectedVRegNo);
                                        cmd.Parameters.AddWithValue("@StartMeter", startMeterReading);
                                        cmd.Parameters.AddWithValue("@StartTime", DateTime.Now);
                                        cmd.Parameters.AddWithValue("@AssignedTo", Empno);

                                        cmd.ExecuteNonQuery();

                                        MessageBox.Show("Day started successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Please enter a valid start meter reading.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error starting the day: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a bike.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedValue != null && int.TryParse(comboBox2.SelectedValue.ToString(), out int selectedVRegNo))
            {
                // Get the end meter reading from the textBox1
                if (int.TryParse(textBox1.Text, out int endMeterReading))
                {
                    // SQL query to check if the bike is assigned to the current rider and no end meter is entered
                    string checkAssignmentQuery = @"SELECT COUNT(*) 
                                                    FROM BIKE_ASSIGNMENT 
                                                    WHERE VReg_No = @VRegNo AND Employee_No = @AssignedTo AND End_meter IS NULL";

                    using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
                    {
                        try
                        {
                            conn.Open();
                            using (SqlCommand checkCmd = new SqlCommand(checkAssignmentQuery, conn))
                            {
                                checkCmd.Parameters.AddWithValue("@VRegNo", selectedVRegNo);
                                checkCmd.Parameters.AddWithValue("@AssignedTo", Empno);

                                int assignmentCount = (int)checkCmd.ExecuteScalar();

                                if (assignmentCount > 0)
                                {
                                    // SQL query to update the existing record in the BIKE_ASSIGNMENT table
                                    string query = @"UPDATE BIKE_ASSIGNMENT 
                                                    SET End_meter = @EndMeter
                                                    WHERE VReg_No = @VRegNo AND Employee_No = @AssignedTo AND End_meter IS NULL";

                                    using (SqlCommand cmd = new SqlCommand(query, conn))
                                    {
                                        cmd.Parameters.AddWithValue("@VRegNo", selectedVRegNo);
                                        cmd.Parameters.AddWithValue("@EndMeter", endMeterReading);
                                        cmd.Parameters.AddWithValue("@AssignedTo", Empno);

                                        int rowsAffected = cmd.ExecuteNonQuery();

                                        if (rowsAffected > 0)
                                        {
                                            MessageBox.Show("Day ended successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                        else
                                        {
                                            MessageBox.Show("No active assignment found for the selected bike.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("This bike is not currently assigned to you or the day has already been ended.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error ending the day: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid end meter reading.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a bike.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void editBikes()
        {
            ColoR.Text = "";
            LicenseStatus.Text = "";
            Prider.Text = "";
            braNd.Text = "";
            modEl.Text = "";
        }

        private void Form16_Load(object sender, EventArgs e)
        {
            getAllbikes();
            getUserDetails();
            userName.Text = Last_Name;
        }

        private void getAllbikes()
        {
            // get all unassigned bikes and show it in comboBox2
            string query = "SELECT VReg_No, Brand, Model, Color_theme FROM BIKE";
            try
            {
                using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader); // Load the result into a DataTable
                            comboBox2.DataSource = dt; // Bind the DataTable to ComboBox
                            comboBox2.DisplayMember = "VReg_No"; // Field to display in ComboBox
                            comboBox2.ValueMember = "VReg_No"; // Field to store in ComboBox
                            comboBox2.SelectedIndex = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedValue != null && int.TryParse(comboBox2.SelectedValue.ToString(), out int selectedVRegNo))
            {
                // SQL query to fetch bike details based on VReg_No
                string query = "SELECT Brand, Model, Color_theme, VRDate FROM BIKE WHERE VReg_No = @VRegNo";

                using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@VRegNo", selectedVRegNo);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Calculate the license status based on VRDate
                                    DateTime vrDate = Convert.ToDateTime(reader["VRDate"]);
                                    DateTime currentDate = DateTime.Now;
                                    string statusLic = (currentDate <= vrDate.AddYears(1)) ? "Valid" : "Expired";

                                    // Populate the input fields with the bike's details
                                    braNd.Text = reader["Brand"].ToString();
                                    LicenseStatus.Text = statusLic;
                                    modEl.Text = reader["Model"].ToString();
                                    ColoR.Text = reader["Color_theme"].ToString();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error fetching bike details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form17 jsf = new Form17(email);
            this.Hide();
            jsf.Show();
        }
    }
}
