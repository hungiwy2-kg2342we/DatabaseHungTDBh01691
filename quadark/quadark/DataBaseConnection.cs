using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace quadark
{
    internal class DataBaseConnection
    {
        private static string _connectionString = "Data Source=THELEGIONCHAMP\\SQLEXPRESS;Initial Catalog=ASM_DataBase_SE07203;Integrated Security=True;";
        public static SqlConnection GetConnection()
        {
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(_connectionString);
            }
            catch
            {
                MessageBox.Show(
                    "error while connecting to the database", "warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
            }
            return connection;
        }
    }
}
