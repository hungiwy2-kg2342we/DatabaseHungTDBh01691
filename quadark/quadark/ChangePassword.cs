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
    public partial class ChangePassword : Form
    {
        private int EmployeeID;
        private string AuthorityLevel;

        public ChangePassword(string AuthorityLevel, int EmployeeID)
        {
            InitializeComponent();
            this.AuthorityLevel = AuthorityLevel;
            this.EmployeeID = EmployeeID;

        }
        private void ClearData()
        {
            txtNewPassword.Text = string.Empty;
            txtConfirmPassword.Text = string.Empty;
            txtNewPassword.Focus();
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void gbChangePassword_Enter(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearData();

        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update password in the database
            bool isPasswordChanged = ChangePasswordInDatabase(EmployeeID, newPassword);

            if (isPasswordChanged)
            {
                MessageBox.Show("Password changed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                NavigateToNextForm();
            }
            else
            {
                MessageBox.Show("Failed to change password. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool ChangePasswordInDatabase(int employeeId, string newPassword)
        {
            try
            {
                // Replace with your database connection logic
                SqlConnection connection = DataBaseConnection.GetConnection();
                if (connection != null)
                {
                    connection.Open();
                    string sql = "UPDATE Employee SET Password = @password WHERE EmployeeID = @employeeId";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("password", newPassword);
                    command.Parameters.AddWithValue("employeeId", employeeId);

                    int result = command.ExecuteNonQuery();
                    connection.Close();

                    return result > 0; // Returns true if the update was successful
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false; // Return false if there was an issue
        }

        private void NavigateToNextForm()
        {
            switch (AuthorityLevel)
            {
                case "Sale":
                    {
                        SaleForm saleForm = new SaleForm(AuthorityLevel, EmployeeID);
                        this.Hide();
                        saleForm.Show();
                        break;
                    }
                case "Warehouse Manager":
                    {
                        WarehouseManagerForm warehouseForm = new WarehouseManagerForm(AuthorityLevel, EmployeeID);
                        this.Hide();
                        warehouseForm.Show();
                        break;
                    }
                default:
                    {
                        MessageBox.Show("Invalid authority level.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
            }
        }

        private void ChangePassword_Load(object sender, EventArgs e)
        {

        }
    }
}
