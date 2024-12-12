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
    public partial class AdminForm : Form
    {
        int employeeId;
        string authorityLevel;
        public AdminForm(string authorityLevel, int employeeId)
        {
            InitializeComponent();
            this.authorityLevel = authorityLevel;
            this.employeeId = employeeId;
        }

        private void btnManageEmployee_Click(object sender, EventArgs e)
        {
            MangeEmployee manageEmployee = new MangeEmployee(authorityLevel);
            this.Hide();
            manageEmployee.Show();
        }

        private void btnManageProduct_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(authorityLevel) || employeeId <= 0)
            {
                MessageBox.Show("Thông tin không hợp lệ!");
                return;
            }

            ManageProduct manageProduct = new ManageProduct(authorityLevel, employeeId);
            this.Hide();
            manageProduct.Show();
        }

        private void btnManageCategory_Click(object sender, EventArgs e)
        {
           
        }

        private void btnManageOrder_Click(object sender, EventArgs e)
        {
            OrderHistory orderhistory = new OrderHistory(authorityLevel, employeeId);
            this.Hide();
            orderhistory.Show();
        }

        private void btnManageImport_Click(object sender, EventArgs e)
        {

        }

        private void btnViewStatistic_Click(object sender, EventArgs e)
        {

        }
    }
}
