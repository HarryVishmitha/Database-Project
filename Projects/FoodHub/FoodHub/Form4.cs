using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace FoodHub
{
    public partial class regP2: Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";
        private string Email;
        private string FirstName;
        private string LastName;
        public regP2(string email, string fName, string lName)
        {
            InitializeComponent();
            Email = email;
            FirstName = fName;
            LastName = lName;
        }

        private void login_Click(object sender, EventArgs e)
        {
            string DOB = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string NIC = nic.Text;
            string HouseNo = houseNo.Text;
            string Addressl1 = addressl1.Text;
            string City = city.Text;
            string Province = province.SelectedItem.ToString();
            string PhoneNo = phoneNu.Text;

            if (valueChecker())
            {
                string queryI2 = "INSERT INTO CUSTOMER (First_name, Last_name, C_DOB, Phone_No, C_NIC, HouseNO, Address_Line1, City, Province, Login_ID)" +
                    "VALUES (@firstName, @lastName, @DOB, @phoneno, @nic, @houseNo, @addressl1, @city, @province, @loginId)";
                string querys3 = "SELECT Login_ID FROM LOGINS WHERE EmailAddress = @email";

                using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
                {
                    try
                    {
                        conn.Open();
                        // getting login id
                        using (SqlCommand cmdSelect = new SqlCommand(querys3, conn))
                        {
                            cmdSelect.Parameters.AddWithValue("@email", Email);
                            var result = cmdSelect.ExecuteScalar();
                            if (result != null)
                            {
                                string loginIdFromDb = result.ToString();
                                using (SqlCommand cmdInsert = new SqlCommand(queryI2, conn))
                                {
                                    cmdInsert.Parameters.AddWithValue("@FirstName", FirstName);  // Use the passed FirstName value
                                    cmdInsert.Parameters.AddWithValue("@LastName", LastName);  // Use the passed LastName value
                                    cmdInsert.Parameters.AddWithValue("@DOB", DOB);
                                    cmdInsert.Parameters.AddWithValue("@PhoneNo", PhoneNo);
                                    cmdInsert.Parameters.AddWithValue("@NIC", NIC);
                                    cmdInsert.Parameters.AddWithValue("@HouseNo", HouseNo);
                                    cmdInsert.Parameters.AddWithValue("@Addressl1", Addressl1);
                                    cmdInsert.Parameters.AddWithValue("@City", City);
                                    cmdInsert.Parameters.AddWithValue("@Province", Province);
                                    cmdInsert.Parameters.AddWithValue("@LoginId", loginIdFromDb);

                                    int rowsAffected = cmdInsert.ExecuteNonQuery();
                                    if (rowsAffected > 0)
                                    {
                                        MessageBox.Show("A new user added successfully!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        Log_in lg = new Log_in();
                                        this.Hide();
                                        lg.Show();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Error saving user information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("No login ID found for the provided email address. Contact admin!", "Unknown Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle any errors that may occur
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }

                }

            }

        }

        private bool valueChecker()
        {
            string DOB = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string NIC = nic.Text;
            string HouseNo = houseNo.Text;
            string Addressl1 = addressl1.Text;
            string City = city.Text;
            string Province = province.SelectedItem.ToString();
            string PhoneNo = phoneNu.Text;
            if (string.IsNullOrEmpty(DOB) || string.IsNullOrEmpty(NIC) || string.IsNullOrEmpty(HouseNo) || string.IsNullOrEmpty(City) || string.IsNullOrEmpty(Province) || string.IsNullOrEmpty(PhoneNo))
            {
                MessageBox.Show("Please fill all required feilds!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label8.Visible = true;
                label2.Visible = true;
                label7.Visible = true;
                label14.Visible = true;
                label12.Visible = true;
                label10.Visible = true;
                return false;
            }

            return true;
        }

        private void regP2_Load(object sender, EventArgs e)
        {

        }
    }
}
