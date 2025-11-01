using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System;
using Library.Pages;

namespace Library
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            string studentId = txtStudentId.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill in all fields.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var conn = DBHelper.GetConnection())
            {
                try
                {
                    conn.Open();

                    // Check if Admin
                    string adminQuery = "SELECT * FROM admins WHERE username=@username AND password=@password";
                    using (MySqlCommand adminCmd = new MySqlCommand(adminQuery, conn))
                    {
                        adminCmd.Parameters.AddWithValue("@username", studentId);
                        adminCmd.Parameters.AddWithValue("@password", password);

                        using (var reader = adminCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                MessageBox.Show("Welcome Admin!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                                AdminDashboard admin = new AdminDashboard();
                                admin.Show();
                                this.Close();
                                return;
                            }
                        }
                    }

                    // Check if Student
                    string studentQuery = "SELECT * FROM students WHERE student_id=@id AND password=@password";
                    using (MySqlCommand studentCmd = new MySqlCommand(studentQuery, conn))
                    {
                        studentCmd.Parameters.AddWithValue("@id", studentId);
                        studentCmd.Parameters.AddWithValue("@password", password);

                        using (var reader = studentCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string name = reader["first_name"].ToString() + " " + reader["last_name"].ToString();
                                MessageBox.Show($"Welcome {name}!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                                StudentDashboard dashboard = new StudentDashboard(studentId);
                                dashboard.Show();
                                this.Close();
                                return;
                            }
                        }
                    }

                    MessageBox.Show("Invalid ID or Password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database connection error:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            CreateAccountPage create = new CreateAccountPage();
            create.Show();
            this.Close();
        }

        private void ForgotPassword_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show("Please contact the librarian to reset your password.", "Forgot Password", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
