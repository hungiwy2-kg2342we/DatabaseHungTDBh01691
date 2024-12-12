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
using System.Windows.Input;

namespace quadark
{
    public partial class ManageCustomer : Form
    {
        private int customerId;
        private int employeeId;
        private string authorityLevel;
        private int userId;
        public ManageCustomer(string authorityLevel, int employeeId)
        {
            this.employeeId = employeeId;
            this.authorityLevel = authorityLevel;
            InitializeComponent();
        }
        
        private bool ValidateData(string customerCode, string customerName, string customerAddress, string phonenumber)
        {
            bool isValid = true;
            if (customerCode == null || customerCode == string.Empty)
            {
                MessageBox.Show("Customer Code cannot be blank",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error); 
                isValid = false;
                txtCustomerCode.Focus();
            }
            else if (customerName == null || customerName == string.Empty)
            {
                MessageBox.Show("Customer Name cannot be blank", "Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                isValid = false ;
                txtCustomerName.Focus();
            }
            else if (customerAddress == null || customerAddress == string.Empty)
            {
                MessageBox.Show("Customer Address cannot be blank","Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                isValid = false;
                txtCustomerAddress.Focus();
            }
            else if (phonenumber == null || phonenumber == string.Empty)
            {
                MessageBox.Show("Phonenumber cannot be blank", "Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error); 
                isValid = false;
                txtPhonenumber.Focus();
            }return isValid;
        }
        private void FlushCustomerId()
        {
            this.customerId = 0;
            
        }
        private void LoadCustomerData()
        {
            // class
            SqlConnection connection = DataBaseConnection.GetConnection();
            // Check connection
            if (connection != null)
            {
                // Open the connection
                connection.Open();
                // Declare query to the database
                string query = "SELECT * FROM Customer";
                // Initialize SqlDataAdapter to translate query result to a data table
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                // Initialize data table
                DataTable table = new DataTable();
                // Fill the data set by data querried from the database
                adapter.Fill(table);
                // Set the datasource of data gridview by the dataset
                dtgCustomer.DataSource = table;
                // close the connection
                connection.Close();
            }
        }
        private bool CheckUserExistence(int customerId)
        {
            bool isExist = false;
            SqlConnection connection = DataBaseConnection.GetConnection();
            if (connection != null)
            {
                connection.Open();
                string checkCustomerQuery = "SELECT * FROM Customer WHERE CustomerID = @customerId";
                // Declare SqlCommand variable to add parameters to query and execute it
                SqlCommand command = new SqlCommand(checkCustomerQuery, connection);
                // Add parameters
                command.Parameters.AddWithValue("customerId", customerId);
                // Declare SqlDataReader variable to read retrieved data
                SqlDataReader reader = command.ExecuteReader();
                // Check if reader has row (query success and return one row show user information)
                isExist = reader.HasRows;
                // close the connection
                connection.Close();
            }
            return isExist;
        }
        private void AddCustomer(string customerCode, string customerName, string customerAddress, string phoneNumber)
        {
            SqlConnection connection = DataBaseConnection.GetConnection();
            
            if (connection != null)
            {
                connection.Open();
                string query = "INSERT INTO Customer (CustomerCode, CustomerName, Phonenumber, Address) " +
                "VALUES (@customerCode, @customerName, @phoneNumber, @customerAddress)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("customerCode", customerCode);
                command.Parameters.AddWithValue("customerName", customerName);
                command.Parameters.AddWithValue("phoneNumber", phoneNumber);
                command.Parameters.AddWithValue("customerAddress", customerAddress);
                int result = command.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Successfully add new customer", "Success",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }else
                {
                    MessageBox.Show("An error occur while adding customer","Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
                connection.Close();              
                // Reload the data gridview
                LoadCustomerData();
            }
            
        }
        private void updateCustomer(int customerId, string customerCode, string customerName, string customerAddress, string phoneNumber)
        {
            SqlConnection connection = DataBaseConnection.GetConnection();
            // Check the connection
            if (connection != null)
            {
                connection.Open();
                string query = "UPDATE Customer SET " +
                "CustomerCode = @customerCode,"+
                "CustomerName = @customerName,"+
                "Address = @customerAddress,"+
                "Phonenumber = @phoneNumber," +
                "WHERE CustomerID = @customerId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("customerCode", customerCode);
                command.Parameters.AddWithValue("customerName", customerName);
                command.Parameters.AddWithValue("customerAddress", customerAddress);
                command.Parameters.AddWithValue("phoneNumber", phoneNumber);
                command.Parameters.AddWithValue("customerId", customerId);
                int result = command.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Successfully update customer", "Success",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }else
                {
                    MessageBox.Show("An error occur while updating customer","Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);

                    connection.Close();
                    // Reload the data gridview
                    LoadCustomerData();
                }
            }
        }
        private void DeleteCustomer(int customerId)
        {
            SqlConnection connection = DataBaseConnection.GetConnection();
            // Check the connection
            if (connection != null)
            {
                connection.Open();
                string deleteOrderDetailQuery = "DELETE OrderDetail WHERE OrderDetailID IN " +
                "(SELECT OrderID FROM Orders WHERE CustomerID = @customerId)";
                // declare SqlCommand to add params and execute query
                SqlCommand command = new SqlCommand(deleteOrderDetailQuery, connection);
                // add parameters
                command.Parameters.AddWithValue("customerId", customerId);
                // execute query (We do not need to know the result because this step is used to ensure no execption occur)
                command.ExecuteNonQuery();
                // Declare query to delete Orders records
                string deleteOrderQuery = "DELETE Orders WHERE CustomerID = @customerId";
                // re-declare SqlCommand with different query
                command = new SqlCommand(deleteOrderQuery, connection);
                // add parameters
                command.Parameters.AddWithValue("customerId", customerId);
                // execute query (We do not need to know the result because this step is used to ensure no execption occur)
                command.ExecuteNonQuery();
                // Declare query to delete Customer records (Now we can delete Customer record because it is not refered by other records in Order table)
                string deleteCustomerQuery = "DELETE Customer WHERE CustomerID = @customerId";
                // re-declare SqlCommand with different query
                command = new SqlCommand(deleteCustomerQuery, connection);
                // add parameters
                command.Parameters.AddWithValue("customerId", customerId);
                // execute query
                int deleteCustomerResult = command.ExecuteNonQuery();
                if (deleteCustomerResult > 0)
                {
                    MessageBox.Show("Successfully delete customer", "Success",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                connection.Close();
                LoadCustomerData();
            }
        }
        private void SearchCustomer(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                LoadCustomerData();
            }else
            {
                // class
                SqlConnection connection = DataBaseConnection.GetConnection();
                // Check connection
                if (connection != null)
                {
                    connection.Open();
                    // Declare query to the database
                    string query = "SELECT * FROM Customer WHERE CustomerCode LIKE @search OR CustomerName LIKE @search OR Phonenumber LIKE @search OR Address LIKE @search";
                    // Initialize SqlDataAdapter to translate query result to a data table
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("search", "%" + search + "%");
                    // Initialize data table
                    DataTable table = new DataTable();
                    // Fill the data set by data querried from the database
                    adapter.Fill(table);
                    // Set the datasource of data gridview by the dataset
                    dtgCustomer.DataSource = table;
                    // close the connection
                    connection.Close();
                }
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void ManageCustomer_Load(object sender, EventArgs e)
        {
            LoadCustomerData(); 
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string customerCode = txtCustomerCode.Text;
            string customerName = txtCustomerName.Text;
            string customerAddress = txtCustomerAddress.Text;
            string phoneNumber = txtPhonenumber.Text;
            // Validate data
            bool isValid = ValidateData(customerCode, customerName, customerAddress, phoneNumber);
            if (isValid)
            {

            }

            AddCustomer(customerCode, customerName, customerAddress, phoneNumber);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (customerId > 0)
            {
                bool isUserExist = CheckUserExistence(customerId);
                if (isUserExist)
                {
                    string customerCode = txtCustomerCode.Text;
                    string customerName = txtCustomerName.Text;
                    string customerAddress = txtCustomerAddress.Text;
                    string phoneNumber = txtPhonenumber.Text;
                    // Validate data
                    bool isValid = ValidateData(customerCode, customerName, customerAddress, phoneNumber);
                    if (isValid)
                    {
                        updateCustomer(customerId, customerCode, customerName, customerAddress, phoneNumber);
                    }

                    else
                    {
                        MessageBox.Show("No customer found", "Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }

                        
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(customerId > 0)
            {
                DialogResult result = MessageBox.Show("Do you want to delete this customer with all related data?","Warning",
                                                        MessageBoxButtons.OKCancel,
                                                        MessageBoxIcon.Question);
                if (result == DialogResult.OK);
                {
                    bool isUserExist = CheckUserExistence(customerId);
                    if (isUserExist)
                    {
                        DeleteCustomer(customerId);
                    }
                    else
                    {
                        MessageBox. Show("No customer found","Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            switch (authorityLevel)
            {
                case "Admin":
                    {
                        AdminForm adminForm = new AdminForm(this.authorityLevel, this.employeeId);
                        this.Hide();
                        adminForm.Show();
                        break;
                    }
                case "Warehouse Manager":
                    {
                        WarehouseManagerForm warehouseManagerForm = new WarehouseManagerForm(this.authorityLevel, this.employeeId);
                        this.Hide();
                        warehouseManagerForm.Show();
                        break;
                    }
                case "Sale":
                    {
                        SaleForm saleForm = new SaleForm(this.authorityLevel, this.employeeId);
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

        private void dtgCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = dtgCustomer.CurrentCell.RowIndex;
            if (index > -1)
            {
                customerId = (int)dtgCustomer.Rows[index].Cells[0].Value;
                string customerName = dtgCustomer.Rows[index].Cells[2].Value.ToString();
               
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            string search = txtSearch.Text;
            SearchCustomer(search);
        }

        private void ManageCustomer_MouseClick(object sender, MouseEventArgs e)
        {
             
        }
    }
}
