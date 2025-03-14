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
    public partial class Log_in: Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";
        public Log_in()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            label8.Visible = false;
            label9.Visible = false;
            //checked passwords and emails fro emplty
            string Email = email.Text;
            string password = passWord.Text;

            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter valid Email Address and its password to login to ths system", "Feilds required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label8.Visible = true;
                label9.Visible = true;
                return;
            }
            else
            {
                bool loginstatus = Login(Email, password);
                if (loginstatus)
                {
                    using (SqlConnection conne = new SqlConnection(FoodHubConnectionString))
                    {
                        conne.Open();
                        string query2 = "SELECT Account_Type FROM LOGINS WHERE EmailAddress = @EmailAddress";
                        SqlCommand cmd2 = new SqlCommand(query2, conne);
                        cmd2.Parameters.AddWithValue("@EmailAddress", Email);

                        string accountType = cmd2.ExecuteScalar().ToString();
                        if (accountType == "admin")
                        {
                            adminPanel admin = new adminPanel(Email);
                            admin.Show();
                            this.Hide();
                        }
                        else if (accountType == "user")
                        {
                            Dashboard userDash = new Dashboard(Email);
                            userDash.Show();
                            this.Hide();
                        }
                        else if (accountType == "rider")
                        {
                            riderBoard rider = new riderBoard(Email);
                            rider.Show();
                            this.Hide();
                        }
                    }
                }
            }
        }

        private bool Login(string email, string password)
        {
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                string query = "SELECT COUNT(1) FROM LOGINS WHERE EmailAddress = @email AND Password = @password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@password", password);

                conn.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count == 1)
                {
                    string query2 = "SELECT Account_Type FROM LOGINS WHERE EmailAddress = @EmailAddress";
                    SqlCommand cmd2 = new SqlCommand(query2, conn);
                    cmd2.Parameters.AddWithValue("@EmailAddress", email);

                    string accountType = cmd2.ExecuteScalar().ToString();
                    MessageBox.Show("Login successful. Account Type: " + accountType + " Click okay to go ahead.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                else
                {
                    MessageBox.Show("Login failed. Invalid email or password for " + email, "Authenication Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void register_Click(object sender, EventArgs e)
        {
            register reg = new register();
            this.Hide();
            reg.Show();
        }

        private void Log_in_Load(object sender, EventArgs e)
        {

        }
    }
}
