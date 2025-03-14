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
    public partial class Form20 : Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";

        private string email;
        private List<Tuple<string, int>> cartItems;
        private int customerId;

        public Form20(string email, List<Tuple<string, int>> cartItems)
        {
            InitializeComponent();
            this.email = email;
            this.cartItems = cartItems;
        }

        private void Form20_Load(object sender, EventArgs e)
        {
            getCustomerId();

            if (cartItems.Count == 0)
            {
                MessageBox.Show("Your cart is empty! Please add products to the cart.", "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Form18 productForm = new Form18(email);
                productForm.Show();
                this.Hide();
            }
            else
            {
                getProductDetails();
            }
        }

        private void getCustomerId()
        {
            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();

                    
                    string getLoginIdQuery = "SELECT Login_ID FROM LOGINS WHERE EmailAddress = @Email";
                    SqlCommand getLoginIdCommand = new SqlCommand(getLoginIdQuery, conn);
                    getLoginIdCommand.Parameters.AddWithValue("@Email", email);

                    int loginId = Convert.ToInt32(getLoginIdCommand.ExecuteScalar());

                    string getCustomerIdQuery = "SELECT CustomerID FROM CUSTOMER WHERE Login_ID = @Login_ID";
                    SqlCommand getCustomerIdCommand = new SqlCommand(getCustomerIdQuery, conn);
                    getCustomerIdCommand.Parameters.AddWithValue("@Login_ID", loginId);

                    customerId = Convert.ToInt32(getCustomerIdCommand.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching customer ID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void getProductDetails()
        {
            decimal total = 0;
            int yOffset = 10;

            
            foreach (var item in cartItems)
            {
                string itemNo = item.Item1;
                int quantity = item.Item2;

                string query = "SELECT Item_Name, Item_Price FROM ITEM WHERE Item_No = @itemNo";

                using (SqlConnection connection = new SqlConnection(FoodHubConnectionString))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            
                            cmd.Parameters.AddWithValue("@itemNo", itemNo);

                            
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string itemName = reader["Item_Name"].ToString();
                                    decimal itemPrice = Convert.ToDecimal(reader["Item_Price"]);

                                    
                                    decimal subtotal = itemPrice * quantity;
                                    total += subtotal;

                                    
                                    Label lblItemDetails = new Label
                                    {
                                        Text = $"{itemName} - ${itemPrice} x {quantity} = ${subtotal}",
                                        Location = new Point(10, yOffset),
                                        Size = new Size(280, 20)
                                    };

                                    
                                    cart.Controls.Add(lblItemDetails);

                                    
                                    yOffset += 30;
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

            Total.Text = $"Total: ${total}";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form18 afaet = new Form18(email);
            this.Hide();
            afaet.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Your cart is empty! Please add products to the cart.", "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Form18 productForm = new Form18(email);
                productForm.Show();
                this.Hide();
                return;
            }

            decimal orderTotal = 0;
            int numberOfItems = cartItems.Count;
            DateTime timestamp = DateTime.Now;
            string paymentMethod = "COD";
            string paymentStatus = "Pending";

            
            string insertOrderQuery = @"
        INSERT INTO [ORDER] (CustomerID, Employee_No, Order_Timestamp, Payment_method, Order_Status, Order_Total, No_of_Items, Payment_Status)
        VALUES (@CustomerID, NULL, @Order_Timestamp, @Payment_method, 'New', @Order_Total, @No_of_Items, @Payment_Status)";

            
            string selectOrderIDQuery = "SELECT Order_No FROM [ORDER] WHERE CustomerID = @CustomerID AND Order_Timestamp = @Order_Timestamp";

            
            string insertOrderItemQuery = "INSERT INTO ORDER_ITEM (Order_No, Item_No, Quantity) VALUES (@Order_No, @Item_No, @Quantity)";

            
            string updateOrderTotalQuery = "UPDATE [ORDER] SET Order_Total = @Order_Total WHERE Order_No = @Order_No";

            using (SqlConnection conn = new SqlConnection(FoodHubConnectionString))
            {
                try
                {
                    conn.Open();

                    
                    using (SqlCommand cmd = new SqlCommand(insertOrderQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerId);
                        cmd.Parameters.AddWithValue("@Order_Timestamp", timestamp);
                        cmd.Parameters.AddWithValue("@Payment_method", paymentMethod);
                        cmd.Parameters.AddWithValue("@Order_Total", orderTotal);
                        cmd.Parameters.AddWithValue("@No_of_Items", numberOfItems);
                        cmd.Parameters.AddWithValue("@Payment_Status", paymentStatus);

                        cmd.ExecuteNonQuery();
                    }

                    
                    int orderNo;
                    using (SqlCommand selectCmd = new SqlCommand(selectOrderIDQuery, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@CustomerID", customerId);
                        selectCmd.Parameters.AddWithValue("@Order_Timestamp", timestamp);

                        orderNo = Convert.ToInt32(selectCmd.ExecuteScalar());
                    }

                    
                    foreach (var item in cartItems)
                    {
                        string itemNo = item.Item1;
                        int quantity = item.Item2;

                        using (SqlCommand itemCmd = new SqlCommand(insertOrderItemQuery, conn))
                        {
                            itemCmd.Parameters.AddWithValue("@Order_No", orderNo);
                            itemCmd.Parameters.AddWithValue("@Item_No", itemNo);
                            itemCmd.Parameters.AddWithValue("@Quantity", quantity);

                            itemCmd.ExecuteNonQuery();
                        }

                        
                        decimal itemPrice = GetItemPrice(itemNo);
                        decimal subtotal = itemPrice * quantity;
                        orderTotal += subtotal;
                    }

                    
                    using (SqlCommand updateCmd = new SqlCommand(updateOrderTotalQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@Order_Total", orderTotal);
                        updateCmd.Parameters.AddWithValue("@Order_No", orderNo);

                        updateCmd.ExecuteNonQuery();
                    }

                    
                    MessageBox.Show($"Order #{orderNo} has been placed successfully! Total: ${orderTotal}", "Order Placed", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    
                    cartItems.Clear();
                    this.Hide();
                    Form18 productForm = new Form18(email);
                    productForm.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while placing the order: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        
        private decimal GetItemPrice(string itemNo)
        {
            
            string query = "SELECT Item_Price FROM ITEM WHERE Item_No = @Item_No";
            using (SqlConnection connection = new SqlConnection(FoodHubConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Item_No", itemNo);
                    return Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
        }

    }
}
