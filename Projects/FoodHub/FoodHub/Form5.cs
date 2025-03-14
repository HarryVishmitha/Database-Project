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
    public partial class riderBoard: Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";

        private string email;
        public string Last_Name { get; set; }
        public riderBoard(string email)
        {
            InitializeComponent();
            this.email = email;
        }

        private void riderBoard_Load(object sender, EventArgs e)
        {
            getUserDetails();
            userName.Text = Last_Name;
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
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form16 jdfka = new Form16(email);
            this.Hide();
            jdfka.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form23 adk = new Form23(email);
            this.Hide();
            adk.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form16 jdfka = new Form16(email);
            this.Hide();
            jdfka.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form17 jsf = new Form17(email);
            this.Hide();
            jsf.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form17 jsf = new Form17(email);
            this.Hide(); 
            jsf.Show();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void userName_Click(object sender, EventArgs e)
        {

        }
    }
}
