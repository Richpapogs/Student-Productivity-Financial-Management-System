using MySql.Data.MySqlClient;
using System.Reflection.PortableExecutable;
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


namespace SP_FMS
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            string id = txtStudentId.Text;
            string password = txtPassword.Password;

            string connectionString = "server=localhost;database=sp_fms;uid=root;pwd=;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id, password, first_name, last_name, course, email, contact FROM students WHERE id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@pass", password);

                    using (MySqlDataReader reader = cmd.ExecuteReader())

                        if (reader.Read())
                        {
                            // Login successful, create Student object
                            Student student = new Student
                            {
                                ID = reader["id"].ToString(),
                                FullName = reader["first_name"].ToString() + " " + reader["last_name"].ToString(),
                                Course = reader["course"].ToString(),
                                Email = reader["email"].ToString(),
                                Contact = reader["contact"].ToString()
                            };

                            MessageBox.Show("Login successful!", "Welcome");

                            Dashboard dashboard = new Dashboard(student);
                            dashboard.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Invalid Student ID or Password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database connection failed:\n" + ex.Message);
                }
            }
        }



        private void Create_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Redirecting to create account page...", "Create Account");
            CreateAccountPage createAccountPage = new CreateAccountPage();
            createAccountPage.Show();
            this.Close();
        }

        private void ForgotPassword_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show("Password recovery coming soon!", "Forgot Password");
        }
    }
}