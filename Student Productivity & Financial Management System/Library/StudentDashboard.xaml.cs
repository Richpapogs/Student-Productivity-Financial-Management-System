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
using System.Data;
using MySql.Data.MySqlClient;

namespace Library.Pages
{
    public partial class StudentDashboard : Window
    {
        private string studentId;

        public StudentDashboard(string id)
        {
            InitializeComponent();
            studentId = id;
            LoadProfile();
            LoadHistory();
        }

        private void LoadProfile()
        {
            using (var conn = DBHelper.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM students WHERE student_id=@id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", studentId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblID.Text = reader["student_id"].ToString();
                                lblName.Text = reader["first_name"] + " " + reader["last_name"];
                                lblCourse.Text = reader["course_section"].ToString();
                                lblEmail.Text = reader["email"].ToString();
                                lblContact.Text = reader["contact_number"].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading profile: " + ex.Message);
                }
            }
        }

        private void SubmitBorrow_Click(object sender, RoutedEventArgs e)
        {
            string book1 = txtBook1.Text.Trim();
            string book2 = txtBook2.Text.Trim();
            string board = txtBoardGame.Text.Trim();

            if (string.IsNullOrEmpty(book1) && string.IsNullOrEmpty(book2) && string.IsNullOrEmpty(board))
            {
                MessageBox.Show("Please enter at least one ID to borrow.", "Empty Fields", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var conn = DBHelper.GetConnection())
            {
                try
                {
                    conn.Open();

                    string query = "INSERT INTO borrow_records (student_id, book_id, boardgame_id, date_borrowed, time_borrowed) " +
                                   "VALUES (@id, @book, @board, CURDATE(), CURTIME())";

                    // Borrow Book 1
                    if (!string.IsNullOrEmpty(book1))
                    {
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", studentId);
                            cmd.Parameters.AddWithValue("@book", book1);
                            cmd.Parameters.AddWithValue("@board", DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Borrow Book 2
                    if (!string.IsNullOrEmpty(book2))
                    {
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", studentId);
                            cmd.Parameters.AddWithValue("@book", book2);
                            cmd.Parameters.AddWithValue("@board", DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Borrow BoardGame
                    if (!string.IsNullOrEmpty(board))
                    {
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", studentId);
                            cmd.Parameters.AddWithValue("@book", DBNull.Value);
                            cmd.Parameters.AddWithValue("@board", board);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Borrow successfully recorded!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtBook1.Clear();
                    txtBook2.Clear();
                    txtBoardGame.Clear();
                    LoadHistory();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadHistory()
        {
            using (var conn = DBHelper.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT record_id, book_id, boardgame_id, 
                        DATE_FORMAT(date_borrowed, '%Y-%m-%d') AS date_borrowed,
                        TIME_FORMAT(time_borrowed, '%h:%i %p') AS time_borrowed,
                        TIME_FORMAT(time_returned, '%h:%i %p') AS time_returned
                 FROM borrow_records
                 WHERE student_id=@id
                 ORDER BY date_borrowed DESC";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", studentId);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgHistory.ItemsSource = dt.DefaultView;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading history: " + ex.Message);
                }
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