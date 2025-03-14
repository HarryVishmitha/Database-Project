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
    public partial class Form22: Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";

        private string email;
        private int orderNo;
        private int selectedEmployeeNo;
        public Form22(string email, int orderNo)
        {
            InitializeComponent();
            this.email = email;
            this.orderNo = orderNo;
        }

        private void Form22_Load(object sender, EventArgs e)
        {
            orderID.Text = orderNo.ToString();
            getFreeRiders();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckOrders sfsla = new CheckOrders(email);
            this.Hide();
            sfsla.Show();
        }

        private void getFreeRiders()
        {
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT e.Employee_No, e.First_Name
                                    FROM RIDER e
                                    INNER JOIN Bike_Assignment b ON e.Employee_No = b.Employee_No
                                    LEFT JOIN [ORDER] o ON e.Employee_No = o.Employee_No AND o.Order_Status = 'Rider Assigned'
                                    WHERE b.Start_Meter IS NOT NULL  -- Ensure Start_Meter is filled
                                    AND b.End_Meter IS NULL  -- Ensure End_Meter is NULL (indicating the rider is not done)
                                    AND o.Order_No IS NULL;  -- Ensure the rider is not currently assigned to an order";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Clear the ComboBox before adding items
                    comboBox2.Items.Clear();

                    while (reader.Read())
                    {
                        comboBox2.Items.Add(new Rider
                        {
                            EmployeeNo = reader.GetInt32(0),
                            EmployeeName = reader.GetString(1)
                        });
                    }

                    reader.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void assignRider()
        {
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Please select a rider.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {

                Rider selectedRider = (Rider)comboBox2.SelectedItem;
                selectedEmployeeNo = selectedRider.EmployeeNo;

                updateStatus();
            }
        }

        private void updateStatus()
        {
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();

                    string updateOrderStatusQuery = "UPDATE [ORDER] SET Order_Status = @Order_Status, Employee_No = @Employee_No WHERE Order_No = @Order_No";
                    SqlCommand cmdUpdateStatus = new SqlCommand(updateOrderStatusQuery, conn);
                    cmdUpdateStatus.Parameters.AddWithValue("@Order_Status", "Rider Assigned");
                    cmdUpdateStatus.Parameters.AddWithValue("@Employee_No", selectedEmployeeNo);
                    cmdUpdateStatus.Parameters.AddWithValue("@Order_No", orderNo);

                    int rowsAffectedOrder = cmdUpdateStatus.ExecuteNonQuery();
                    if (rowsAffectedOrder == 0)
                    {
                        MessageBox.Show("Error: Order not found or already assigned.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    MessageBox.Show("Rider assigned successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    
                    CheckOrders checkOrdersForm = new CheckOrders(email);
                    this.Hide();
                    checkOrdersForm.Show();
                }
                catch (Exception ex)
                {
                    
                    MessageBox.Show("Error assigning rider: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            assignRider();
        }

        public class Rider
        {
            public int EmployeeNo { get; set; }
            public string EmployeeName { get; set; }

            public override string ToString()
            {
                return EmployeeName;
            }
        }
    }
}
