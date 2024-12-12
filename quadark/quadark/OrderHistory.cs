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

namespace quadark
{
    public partial class OrderHistory : Form
    {
        string authorityLevel;
        int employeeID;
        public OrderHistory(string authorityLevel, int employeeID)
        {

            InitializeComponent();
            this.authorityLevel = authorityLevel;
            this.employeeID = employeeID;
        }
        private void LoadOrderHistory()
        {
            SqlConnection connection = DataBaseConnection.GetConnection();
            if (connection != null)
            {
                connection.Open();
                string query = "SELECT o.OrderDate, " +
                               "e.EmployeeName, " +
                               "c.CustomerName " +  // Thêm dấu cách ở đây
                               "FROM Orders o " +
                               "INNER JOIN Employee e ON o.EmployeeID = e.EmployeeID " + // Thêm dấu cách
                               "INNER JOIN Customer c ON o.CustomerID = c.CustomerID " + // Thêm dấu cách
                               "WHERE o.EmployeeID = @employeeID " + // Thêm dấu cách
                               "GROUP BY o.OrderDate, e.EmployeeName, c.CustomerName"; // Sửa cột GROUP BY

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@employeeID", employeeID); // Thêm dấu @ vào tham số
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dtgOrderHistory.DataSource = dataTable;

                connection.Close(); // Đảm bảo đóng kết nối
            }
        }
        private void OrderHistory_Load(object sender, EventArgs e)
        {
            LoadOrderHistory();
        }
        private void RedirectPage()
        {
            switch (this.authorityLevel)
            {
                case "Admin":
                    {
                        AdminForm adminForm = new AdminForm(authorityLevel, employeeID);
                        this.Hide();
                        adminForm.Show();
                        break;
                    }
                case "Warehouse Manager":
                    {
                        WarehouseManagerForm warehouseManagerForm = new WarehouseManagerForm(authorityLevel, employeeID);
                        this.Hide();
                        warehouseManagerForm.Show();
                        break;
                    }
                case "Sale":
                    {
                        ManageCustomer saleForm = new ManageCustomer(authorityLevel, employeeID);
                        this.Hide();
                        saleForm.Show();
                        break;
                    }
                default:
                    {
                        break;
                    }

            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            RedirectPage();
        }
    }
}
