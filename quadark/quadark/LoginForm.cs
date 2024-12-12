using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using quadark;


namespace quadark
{

    public partial class LoginForm : Form

    {
        public void InitializeCombobox()
        {
            cbRole.Items.Add("Admin");
            cbRole.Items.Add("Warehouse Manager");
            cbRole.Items.Add("Sale");
            cbRole.SelectedIndex = 0;
        }
        public LoginForm()
        {

            InitializeComponent();
            InitializeCombobox();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Do you really want to exit the program?", "Warning", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
            {
                e.Cancel = true;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private bool ValidateData(string username, string password, string role)
        {
            bool IsValid = true;
            if (username == null || username == string.Empty)
            {
                MessageBox.Show(
                    "Username cannot be blank",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
                IsValid = false;
                txtUsername.Focus();
            }
            else if (password == null || password == string.Empty)
            {
                MessageBox.Show(

                    "Username cannot be blank",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
                IsValid = false;
                txtPassword.Focus();

            }
            else if (role == null || password == string.Empty)
            {
                MessageBox.Show(

                    "Username cannot be blank",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
                IsValid = false;
                cbRole.Focus();

            }
            return IsValid;
        }

        private void RedirectPage(string selectedRole, int employeeID, bool IsChangePassword)
        {
            if (IsChangePassword)
            {
                if (selectedRole != null)
                {
                    if (selectedRole == "Admin")
                    {
                        AdminForm admin = new AdminForm(selectedRole, employeeID);
                        this.Hide();
                        admin.Show();
                    }
                    else if (selectedRole == "Warehouse Manager")
                    {
                        WarehouseManagerForm warehouseManagerForm = new WarehouseManagerForm(selectedRole, employeeID);
                        this.Hide();
                        warehouseManagerForm.Show();
                    }
                    else if (selectedRole == "Sale")
                    {
                        SaleForm saleForm = new SaleForm(selectedRole, employeeID);
                        this.Hide();
                        saleForm.Show();
                    }

                }
            }
            else
            {
                ChangePassword changePassword = new ChangePassword(selectedRole, employeeID);
                changePassword.Show();
                this.Hide();

            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Get data from user input
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string role = cbRole.SelectedItem.ToString();

            // Valid data
            bool isValid = ValidateData(username, password, role);
            if (isValid)
            {
                // Open connection by calling the GetConnection function in DatabaseConnection class
                SqlConnection connection = DataBaseConnection.GetConnection();

                // Check if the connection is null
                if (connection == null)
                {
                    MessageBox.Show(
                        "Unable to connect to the database.",
                        "Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Define query to execute
                string query = "SELECT EmployeeId, PasswordChanged FROM Employee WHERE Username = @username " +
                               "AND Password = @password AND AuthorityLevel = @role";

                try
                {
                    connection.Open();

                    // Initialize SqlCommand to execute query
                    SqlCommand command = new SqlCommand(query, connection);

                    // Add parameters to query
                    command.Parameters.AddWithValue("username", username);
                    command.Parameters.AddWithValue("password", password);
                    command.Parameters.AddWithValue("role", role);

                    // Retrieve data from the database
                    SqlDataReader reader = command.ExecuteReader();

                    int employeeId = 0;
                    bool passwordChanged = false;

                    // Read data
                    while (reader.Read())
                    {
                        employeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId"));
                        passwordChanged = reader.GetBoolean(reader.GetOrdinal("PasswordChanged"));
                    }

                    // Check if a valid employee was found
                    if (employeeId > 0)
                    {
                        MessageBox.Show(
                            "Login successful",
                            "Information",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        // Redirect to appropriate page based on role and password status
                        RedirectPage(role, employeeId, passwordChanged);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Invalid login credentials",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions, such as connection errors or SQL errors
                    MessageBox.Show(
                        "An error occurred: " + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    // Close the connection to release resources
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show(
                    "Please enter valid data.",
                    "Input Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void cboRole_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}