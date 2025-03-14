using System;
using System.Data.SqlClient;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace FoodHub
{
    public partial class Dashboard : Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";

        private string email;
        public string Last_Name { get; set; }

        public Dashboard(string email)
        {
            InitializeComponent();
            this.email = email;
        }

        
        private void Dashboard_Load(object sender, EventArgs e)
        {
            getUserDetails();
            userName.Text = Last_Name;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form18 jdfas = new Form18(email);
            this.Hide();
            jdfas.Show();
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
                    string getRiderDetailsQuery = @"SELECT * FROM CUSTOMER WHERE Login_ID = @Login_ID";

                    SqlCommand getRiderDetailsCommand = new SqlCommand(getRiderDetailsQuery, conn);
                    getRiderDetailsCommand.Parameters.AddWithValue("@Login_ID", loginId);

                    using (SqlDataReader reader = getRiderDetailsCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Last_Name = reader["Last_name"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form21 aljrsd = new Form21(email);
            this.Hide();
            aljrsd.Show();
        }
    }
}
