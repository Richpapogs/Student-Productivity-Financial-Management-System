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
            string id = txtStudentId.Text.Trim();
            string password = txtPassword.Password.Trim();

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                try
                {
                    conn.Open();

                    string query = "SELECT password FROM students WHERE id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    string storedHash = cmd.ExecuteScalar()?.ToString();

                    if (storedHash == null)
                    {
                        MessageBox.Show("Invalid Student ID or Password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Verify hashed password
                    if (!PasswordHelper.VerifyPassword(password, storedHash))
                    {
                        MessageBox.Show("Invalid Student ID or Password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Fetch full student record
                    string infoQuery = @"SELECT id, first_name, last_name, course, email, contact 
                                 FROM students WHERE id=@id";

                    MySqlCommand cmdInfo = new MySqlCommand(infoQuery, conn);
                    cmdInfo.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = cmdInfo.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Student student = new Student
                            {
                                ID = reader["id"].ToString(),
                                FullName = $"{reader["first_name"]} {reader["last_name"]}",
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
                            MessageBox.Show("Login data incomplete.", "Error");
                        }
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
            CreateAccountPage createAccountPage = new CreateAccountPage();
            createAccountPage.Show();
            this.Close();
        }

        private void ForgotPassword_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ForgotPasswordWindow forgotPasswordWindow = new ForgotPasswordWindow();
            forgotPasswordWindow.Owner = this;
            forgotPasswordWindow.ShowDialog();
        }
    }
}