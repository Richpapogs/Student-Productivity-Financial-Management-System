using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Data;

namespace Library.Pages
{
    /// <summary>
    /// Interaction logic for AdminDashboard.xaml
    /// </summary>
    public partial class AdminDashboard : Window
    {
        public AdminDashboard()
        {
            InitializeComponent();
            LoadBorrowRecords();
        }

        private void LoadBorrowRecords()
        {
            using (var conn = DBHelper.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT record_id, student_id, book_id, boardgame_id, 
                                     DATE_FORMAT(date_borrowed, '%Y-%m-%d') AS date_borrowed,
                                     TIME_FORMAT(time_borrowed, '%h:%i %p') AS time_borrowed,
                                     TIME_FORMAT(time_returned, '%h:%i %p') AS time_returned,
                                     status
                                     FROM borrow_records
                                     ORDER BY date_borrowed DESC, time_borrowed DESC";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgBorrowRecords.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading records: " + ex.Message);
                }
            }
        }

        private void LogoutBorrow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.Tag is int recordId)
            {
                using (var conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string update = @"UPDATE borrow_records 
                                      SET time_returned = CURTIME(), status = 'Returned' 
                                      WHERE record_id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(update, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", recordId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Borrower has been logged out and marked as returned.");
                LoadBorrowRecords(); // refresh grid
            }
        }
    

        

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }
    }
}