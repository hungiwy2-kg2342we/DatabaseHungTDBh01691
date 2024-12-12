using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace quadark
{
    public partial class ManageProduct : Form
    {
        private int productId;
        private string authorityLevel;
        private int userId;
        public ManageProduct(string authorityLevel, int userId)

        {
            this.authorityLevel = authorityLevel;
            this.userId = userId;
            productId = 0;
            InitializeComponent();
        }
        private void LoadCategoryCombobox()
        {
            SqlConnection connection = DataBaseConnection.GetConnection();
            if (connection != null)
            {
                connection.Open();
                string query = "SELECT CategoryID, CategoryName FROM Category";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                cbCategory.DataSource = dataTable;
                cbCategory.DisplayMember = "CategoryName";
                cbCategory.ValueMember = "CategoryID";
            }
        }
        private bool ValidateData(String productCode,
                                String productName,
                                String productPrice,
                                String productQuantity)
        {
            double temp;
            int temp2;
            if (String.IsNullOrEmpty(productName)) { return false; }
            if (String.IsNullOrEmpty(productPrice)) { return false; }
            if (!double.TryParse(productPrice, out temp)) { return false; }
            if (String.IsNullOrEmpty(productQuantity)) { return false; }
            return int.TryParse(productQuantity, out temp2);
        }
        private void UploadFile(String filter, String path)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = filter;
            openFileDialog.Title = "Select a file to upload";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sourceFilePath = openFileDialog.FileName;
                string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Upload");
                string targetFilePath = Path.Combine(targetDirectory, Path.GetFileName(sourceFilePath));

                try
                {
                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    File.Copy(sourceFilePath, targetFilePath, overwrite: true);
                    txtProductImg.Text = targetFilePath;
                    MessageBox.Show("File uploaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error uploading file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void LoadProductData()
        {
            SqlConnection connection = DataBaseConnection.GetConnection();
            if (connection != null)
            {
                connection.Open();
                string querry = "SELECT * FROM Product";
                SqlDataAdapter adapter = new SqlDataAdapter(querry, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dtgProduct.DataSource = dataTable;
                connection.Close();
            }
        }
        private void ClearData()
        {

            txtProductCode.Text = string.Empty;
            txtProductName.Text = string.Empty;
            txtProductImg.Text = string.Empty;
            txtProductPrice.Text = string.Empty;
            txtProductQuantity.Text = string.Empty;
            txtSearch.Text = string.Empty;
        }

        private void AddProduct()
        {
            SqlConnection connection = DataBaseConnection.GetConnection();
            if (connection != null)
            {
                connection.Open();
                // Lấy dữ liệu từ giao diện
                string productCode = txtProductCode.Text;
                string productName = txtProductName.Text;
                string productImg = txtProductImg.Text;
                string price = txtProductPrice.Text;
                string quantity = txtProductQuantity.Text;
                int categoryId = Convert.ToInt32(cbCategory.SelectedValue);

                // Kiểm tra dữ liệu hợp lệ
                if (ValidateData(productCode, productName, price, quantity))
                {
                    string sql;
                    if (this.productId == 0) // Nếu sản phẩm mới
                    {
                        sql = "INSERT INTO Product (ProductCode, ProductName, Price, InventoryQuantity, ProductImage, CategoryID) " +
                              "VALUES (@productCode, @productName, @productPrice, @productQuantity, @productImg, @categoryId)";
                    }
                    else // Nếu sản phẩm đã tồn tại, thực hiện cập nhật
                    {
                        sql = "UPDATE Product SET ProductCode = @productCode, " +
                              "ProductName = @productName, " +
                              "Price = @productPrice, " +
                              "InventoryQuantity = @productQuantity, " +
                              "ProductImage = @productImg, " +
                              "CategoryID = @categoryId " +
                              "WHERE ProductID = @productId";
                    }

                    SqlCommand command = new SqlCommand(sql, connection);
                    // Gắn tham số
                    command.Parameters.AddWithValue("productCode", productCode);
                    command.Parameters.AddWithValue("productName", productName);
                    command.Parameters.AddWithValue("productPrice", Convert.ToDouble(price));
                    command.Parameters.AddWithValue("productQuantity", Convert.ToInt32(quantity));
                    command.Parameters.AddWithValue("productImg", productImg);
                    command.Parameters.AddWithValue("categoryId", categoryId);
                    if (this.productId != 0) // Nếu là cập nhật
                    {
                        command.Parameters.AddWithValue("productId", this.productId);
                    }

                    // Thực thi truy vấn
                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show(this.productId == 0
                            ? "Successfully added a new product!"
                            : "Successfully updated the product!",
                            "Information",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        ClearData();
                        LoadProductData();
                    }
                    else
                    {
                        MessageBox.Show("Operation failed. Please try again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    connection.Close();
                }
            }
        }

        private bool IsProductInOrder(int productID)
        {
            SqlConnection connection = DataBaseConnection.GetConnection();
            if (connection != null)
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM OrderDetail WHERE productID = @productID";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("productID", productID);
                int result = (int)command.ExecuteScalar();
                connection.Close();
                return result > 0;


            }
            return false;
        }
        private void DeleteProduct()
        {
            DialogResult dialogResult = MessageBox.Show("Do you want to delete the product", "Warning",
                                                MessageBoxButtons.OKCancel,
                                                MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                if (!IsProductInOrder(this.productId))
                {
                    SqlConnection connection = DataBaseConnection.GetConnection();
                    // Check connection
                    if (connection != null)
                    {
                        connection.Open();
                        string sql = "DELETE Product WHERE ProductID = @productId";
                        SqlCommand command = new SqlCommand(sql, connection);
                        // add params
                        command.Parameters.AddWithValue("productId", this.productId);
                        // execute query and get the result
                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            MessageBox.Show("Successfully delete product", "Information",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);
                            ClearData();
                            LoadProductData();
                        }
                        else
                        {
                            MessageBox.Show("Cannot delete product", "Error",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);
                        }
                        connection.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Product is in another order\nCannot delete", "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
        }
        private void SearchProduct(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                LoadProductData();
            }
            else
            {
                SqlConnection connection = DataBaseConnection.GetConnection();
                // Check connection
                if (connection != null)
                {
                    connection.Open();
                    string sql = "SELECT p.ProductID, p.ProductCode, p.ProductName, p.Price, " +
                                        "p. InventoryQuantity, p.ProductImage, c.CategoryName" +
                                        "FROM Product p " +
                                        "INNER JOIN Category c " +
                                        "ON p.CategoryID = c. CategoryID " +
                                        "WHERE p.ProductCode LIKE @search" +
                                        "OR p.ProductName LIKE @search " +
                                        "OR c.CategoryName LIKE @search";
                    // Initialize SqlDataAdapter to translate query result to a data table
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                    // Add params to query
                    adapter.SelectCommand.Parameters.AddWithValue("search", "%" + search + "%");
                    // Initialize data table
                    DataTable data = new DataTable();
                    // Fill datatable with data queried from database
                    adapter.Fill(data);
                    // Set the data source for data table
                    dtgProduct.DataSource = data;
                    // Close the connection
                    connection.Close();
                }
            }
        }
        private void UpdateProduct()
        {
            SqlConnection connection = DataBaseConnection.GetConnection();
            if (connection != null)
            {
                connection.Open();
                string productCode = txtProductCode.Text;
                string productName = txtProductName.Text;
                string productImg = txtProductImg.Text;
                string price = txtProductPrice.Text;
                string quantity = txtProductQuantity.Text;
                int categoryID = Convert.ToInt32(cbCategory.SelectedValue);

                if (ValidateData(productCode, productName, price, quantity))
                {
                    string sql = "UPDATE Product SET productCode = @productCode" +
                        "productName = @productName," +
                        "price = @productPrice," +
                        "stock_quantity = @productQuantity, " +
                        "image_url = @productImg, " +
                        "categoryID = @categoryID " +
                        "WHERE ProductID = @productID";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("productCode", productCode);
                    command.Parameters.AddWithValue("productName", productName);
                    command.Parameters.AddWithValue("productPrice", Convert.ToDouble(price));
                    command.Parameters.AddWithValue("productQuantity", Convert.ToInt32(quantity));
                    command.Parameters.AddWithValue("productImg", productImg);
                    command.Parameters.AddWithValue("categoryID", categoryID);
                    command.Parameters.AddWithValue("productID", this.productId);
                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show(
                            "Successfully add new product",
                            "Information",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        ClearData();
                        LoadProductData();

                    }
                    else
                    {
                        MessageBox.Show(
                            "Cannot add new product",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                    }
                    connection.Close();
                }
            }
        }



        private void lblProductCode_Click(object sender, EventArgs e)
        {

        }

        private void ManageProduct_Load(object sender, EventArgs e)
        {
            LoadProductData();
            // binding data to combobox
            LoadCategoryCombobox();
            // set status for button to ensure the UX of user
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            this.productId = 0; // Đặt productId về 0 để đảm bảo đây là sản phẩm mới
            AddProduct();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteProduct();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (this.productId != 0) // Đảm bảo sản phẩm đã được chọn
            {
                AddProduct(); // Sử dụng lại hàm AddProduct để xử lý cập nhật
            }
            else
            {
                MessageBox.Show("Please select a product to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            switch (authorityLevel)
            {
                case "Admin":
                    {
                        AdminForm adminForm = new AdminForm(this.authorityLevel, this.userId);
                        this.Hide();
                        adminForm.Show();
                        break;
                    }
                case "Warehouse Manager":
                    {
                        WarehouseManagerForm warehouseManagerForm = new WarehouseManagerForm(this.authorityLevel, this.userId);
                        this.Hide();
                        warehouseManagerForm.Show();
                        break;
                    }
                case "Sale":
                    {
                        SaleForm saleForm = new SaleForm(this.authorityLevel, this.userId);
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


        private void dtgProduct_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = dtgProduct.CurrentCell.RowIndex;
            if (index != -1 && !dtgProduct.Rows[index].IsNewRow)
            {
                this.productId = Convert.ToInt32(dtgProduct.Rows[index].Cells[0].Value);
                txtProductCode.Text = dtgProduct.Rows[index].Cells[1].Value.ToString();
                txtProductName.Text = dtgProduct.Rows[index].Cells[2].Value.ToString();
                txtProductPrice.Text = dtgProduct.Rows[index].Cells[3].Value.ToString();
                txtProductQuantity.Text = dtgProduct.Rows[index].Cells[4].Value.ToString();
                txtProductImg.Text = dtgProduct.Rows[index].Cells[5].Value.ToString();
                cbCategory.SelectedValue = dtgProduct.Rows[index].Cells[6].Value;
            }
        }


        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void btnUpload_Click(object sender, EventArgs e)
        {

        }

        private void gbProductInformation_Enter(object sender, EventArgs e)
        {

        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {




        }
    }
}