using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace quadark
{
    public partial class SaleForm : Form
    {
        private int employeeId;
        private string authorityLevel;
        public SaleForm(string authorityLevel, int employeeId)
        {
          
        InitializeComponent();
        this.employeeId = employeeId;
        this.authorityLevel = authorityLevel;
        }

        private void SaleForm_Load(object sender, EventArgs e)
        {

        }

        private void btnManageCustomer_Click(object sender, EventArgs e)
        {
            ManageCustomer manageCustomer = new ManageCustomer(authorityLevel, employeeId);
            this.Hide();
            manageCustomer.Show();
        }

        private void btnManageOrder_Click(object sender, EventArgs e)
        {
            OrderHistory orderHistory = new OrderHistory(this.authorityLevel, this.employeeId);
            this.Hide();
            orderHistory.Show();
        }
    }
}   
