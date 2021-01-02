using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LicenseApp
{
    public partial class MainWindow : Window
    {
        private string connectionString;
        SqlDataAdapter adapter;
        DataTable licensesTable;
        public MainWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["LicenseApp.Properties.Settings.licensesConnectionString"].ConnectionString;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT * FROM licenses_in_use";   
            licensesTable = new DataTable();
            SqlConnection connection = null;
            try
            {
                //setting up a new connection to the DataBase 
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);

                adapter = new SqlDataAdapter(command);
                //inserting a stored procedure so adding and updating a new user could be possible
                adapter.InsertCommand = new SqlCommand("sp_InsertLicenseUser", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@first_name", SqlDbType.NVarChar, 50, "first_name"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@last_name", SqlDbType.NVarChar, 50, "last_name"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@office_id", SqlDbType.Int, 0, "office_id"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@department", SqlDbType.NVarChar, 50, "department"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@license_name", SqlDbType.NVarChar, 50, "license_name"));
                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@id", SqlDbType.Int, 0, "id");
                parameter.Direction = ParameterDirection.Output;

                connection.Open();
                adapter.Fill(licensesTable);
                licenseGrid.ItemsSource = licensesTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }

        //Updating data 
        private void UpdateDB()
        {
            SqlCommandBuilder comandbuilder = new SqlCommandBuilder(adapter);
            adapter.Update(licensesTable);
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateDB();
        }
        //Deleting data from DB
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (licenseGrid.SelectedItems != null)
            {
                for (int i = 0; i < licenseGrid.SelectedItems.Count; i++)
                {
                    DataRowView datarowView = licenseGrid.SelectedItems[i] as DataRowView;
                    if (datarowView != null)
                    {
                        DataRow dataRow = (DataRow)datarowView.Row;
                        dataRow.Delete();
                    }
                }
            }
            UpdateDB();
        }
    }
}
