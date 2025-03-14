using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FoodHub
{
    public partial class register : Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";
        public register()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            //Log_in log_In = new Log_in();
            //this.Hide();
            //log_In.Show();
        }

        private void login_Click(object sender, EventArgs e)
        {
            label8.Visible = false;
            label2.Visible = false;
            label4.Visible = false;
            label7.Visible = false;
            label10.Visible = false;

            String firstName = fName.Text;
            String lastName = lName.Text;
            String Email = email.Text;
            string Password = password.Text;
            string Cpassword = confirmPassword.Text;

            if (valueChecker())
            {
                using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
                {
                    string queryI1 = "INSERT INTO LOGINS (EmailAddress, Password, Account_Type) VALUES (@Email, @Password, 'user')";
                    using (SqlCommand cmd = new SqlCommand(queryI1, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", Email);
                        cmd.Parameters.AddWithValue("@Password", Password);

                        conn.Open();

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("User inserted successfully.");
                            regP2 regnext = new regP2(Email, firstName, lastName);
                            this.Hide();
                            regnext.Show();
                        }
                        else
                        {
                            MessageBox.Show("Something went wrong! Please contact developers!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                }
            }
        }

        private bool valueChecker()
        {
            String firstName = fName.Text;
            String lastName = lName.Text;
            String Email = email.Text;
            string Password = password.Text;
            string Cpassword = confirmPassword.Text;

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Cpassword))
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label8.Visible = true;
                label2.Visible = true;
                label4.Visible = true;
                label7.Visible = true;
                label10.Visible = true;
                return false;
            }

            if (Password != Cpassword)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label10.Visible = true;
                label10.Text = "Password don't match!";
                return false;
            }
            if (!IsValidEmail(Email))
            {
                MessageBox.Show("Invalid email format.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label4.Text = "Invalid Email format!";
                label4.Visible = true;
                return false;
            }
            if (EmailExistant(Email))
            {
                MessageBox.Show("Sorry! Email already exists on our database!", "User Exist!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                label4.Text = "Email already used!";
                label4.Visible = true;
                return false;
            }



            return true;
        }
        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        private bool EmailExistant(string email)
        {
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                string query = "SELECT COUNT(1) FROM LOGINS WHERE EmailAddress = @email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@email", email);
                conn.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count > 0)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
