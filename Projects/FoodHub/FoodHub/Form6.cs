using System;
using System.Collections;
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

namespace FoodHub
{
    public partial class adminPanel: Form
    {
        string FoodHubConnectionString = "Data Source=DESKTOP-8PVTG63;Initial Catalog=FoodHub;Integrated Security=True;";
        private string email;
        public adminPanel(string email)
        {
            InitializeComponent();
            this.email = email;
        }

        private void adminPanel_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckOrders chk = new CheckOrders(email);
            chk.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            adminPanel adm = new adminPanel(email);
            adm.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            addnewUser add = new addnewUser(email);
            add.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            chkUsers chk = new chkUsers(email);
            this.Hide();
            chk.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form10 hgj = new Form10(email);
            this.Hide();
            hgj.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form11 sdgkj = new Form11(email);
            this.Hide();
            sdgkj.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Form12 ksjf = new Form12(email);
            this.Hide();
            ksjf.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Form13 kasdj = new Form13(email);
            this.Hide();
            kasdj.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form14 lakd = new Form14(email);
            this.Hide();
            lakd.Show();
        }
    }
}
