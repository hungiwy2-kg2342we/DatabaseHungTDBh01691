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
    public partial class WarehouseManagerForm : Form
    {
        int employeeId;
        string authorityLevel;
        public WarehouseManagerForm(string authorityLevel, int employeeId)
        {

            InitializeComponent();
            this.authorityLevel = authorityLevel;
            this.employeeId = employeeId;
        }

        private void WarehouseManagerForm_Load(object sender, EventArgs e)
        {
            SaleForm saleForm = new SaleForm(authorityLevel, employeeId);
            this.Hide();
            saleForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
