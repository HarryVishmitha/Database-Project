using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FoodHub
{
    public partial class Form14: Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";
        private string email;
        public Form14(string email)
        {
            InitializeComponent();
            this.email = email;
        }

        private void Form14_Load(object sender, EventArgs e)
        {
            allBikes();
        }

        private void allBikes()
        {
            Editbikes.SelectedIndex = -1;
            // Get all bikes from the database and show them in Editbikes (ComboBox)
            // By default, there is no selected bike in ComboBox

            // Create a connection to your database
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    // Open the database connection
                    conn.Open();

                    // SQL query to retrieve all bike registrations and brands
                    string query = "SELECT VReg_No, Brand FROM BIKE";

                    // Create a data adapter
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Bind the data to the ComboBox (Editbikes)
                    Editbikes.DataSource = dt;
                    Editbikes.DisplayMember = "Brand - VReg_No"; // Field to display in ComboBox
                    Editbikes.ValueMember = "VReg_No"; // Field to store in ComboBox
                    Editbikes.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching bike data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void addnewBike()
        {
            string Vreg = fName.Text;
            string brand = textBox1.Text;
            string model = textBox2.Text;
            string enumber = textBox3.Text;
            string regdate = textBox5.Text;
            string color = textBox4.Text;

            // Create a new bike entry in the database
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();

                    // SQL query to insert a new bike
                    string query = "INSERT INTO BIKE (VReg_No, Brand, Model, Engine_No, VRDate, Color_theme) VALUES (@Vreg, @Brand, @Model, @ENumber, @RegDate, @Color)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Vreg", Vreg);
                    cmd.Parameters.AddWithValue("@Brand", brand);
                    cmd.Parameters.AddWithValue("@Model", model);
                    cmd.Parameters.AddWithValue("@ENumber", enumber);
                    cmd.Parameters.AddWithValue("@RegDate", regdate);
                    cmd.Parameters.AddWithValue("@Color", color);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("New bike added successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        allBikes(); // Refresh the bike list in the ComboBox
                    }
                    else
                    {
                        MessageBox.Show("Failed to add new bike", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding new bike: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void updateBike()
        {
            string Vreg = fName.Text;
            string brand = textBox1.Text;
            string model = textBox2.Text;
            string enumber = textBox3.Text;
            string regdate = textBox5.Text;
            string color = textBox4.Text;
            int vRegNo = (int)Editbikes.SelectedValue;

            // Update the bike details in the database
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();

                    // SQL query to update the bike
                    string query = "UPDATE BIKE SET Brand = @Brand, Model = @Model, Engine_No = @ENumber, VRDate = @RegDate, Color_theme = @Color WHERE VReg_No = @Vreg";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Vreg", vRegNo);
                    cmd.Parameters.AddWithValue("@Brand", brand);
                    cmd.Parameters.AddWithValue("@Model", model);
                    cmd.Parameters.AddWithValue("@ENumber", enumber);
                    cmd.Parameters.AddWithValue("@RegDate", regdate);
                    cmd.Parameters.AddWithValue("@Color", color);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Bike details updated successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to update bike details", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating bike details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (validateInputs())
            {
                if (Editbikes.SelectedItem != null)
                {
                    updateBike();
                }
                else
                {
                    addnewBike();
                }
            }
            else
            {
                label12.Visible = true;
                label3.Visible = true;
                label5.Visible = true;
                label7.Visible = true;
                label11.Visible = true;
                label9.Visible = true;
                MessageBox.Show("Please fill all required fields", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool validateInputs()
        {
            string Vreg = fName.Text;
            string brand = textBox1.Text;
            string model = textBox2.Text;
            string enumber = textBox3.Text;
            string regdate = textBox5.Text;
            string color = textBox4.Text;

            // Check if any of the input fields are empty
            if (string.IsNullOrWhiteSpace(Vreg) ||
                string.IsNullOrWhiteSpace(brand) ||
                string.IsNullOrWhiteSpace(model) ||
                string.IsNullOrWhiteSpace(enumber) ||
                string.IsNullOrWhiteSpace(regdate) ||
                string.IsNullOrWhiteSpace(color))
            {
                return false;
            }

            return true;
        }

        private void Editbikes_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ensure the ComboBox has a selected item before trying to access its value
            if (Editbikes.SelectedValue != null)
            {
                int selectedVRegNo;
                // Try to parse the selected value to an int
                if (int.TryParse(Editbikes.SelectedValue.ToString(), out selectedVRegNo))
                {
                    // Get the selected bike's details and populate the input fields
                    using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
                    {
                        try
                        {
                            conn.Open();

                            // SQL query to fetch bike details
                            string query = "SELECT * FROM BIKE WHERE VReg_No = @VRegNo";
                            SqlCommand cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@VRegNo", selectedVRegNo);

                            SqlDataReader reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                // Populate the input fields with the bike's details
                                fName.Text = reader["VReg_No"].ToString();
                                textBox1.Text = reader["Brand"].ToString();
                                textBox2.Text = reader["Model"].ToString();
                                textBox3.Text = reader["Engine_No"].ToString();
                                textBox5.Text = reader["VRDate"].ToString();
                                textBox4.Text = reader["Color_theme"].ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error fetching bike details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    //MessageBox.Show("Invalid bike selection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            adminPanel admin = new adminPanel(email);
            admin.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form15 dfmas = new Form15(email);
            this.Hide();
            dfmas.Show();
        }
    }
}
