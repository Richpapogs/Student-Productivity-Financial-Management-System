
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Generators;
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

namespace SP_FMS
{
    /// <summary>
    /// Interaction logic for CreateAccountPage.xaml
    /// </summary>
    public partial class CreateAccountPage : Window
    {

        public CreateAccountPage()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            // Example: go back to the login window
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            string id = txtID.Text.Trim();
            string firstName = txtFName.Text.Trim();
            string lastName = txtLName.Text.Trim();
            string course = txtCourse.Text.Trim();
            string email = txtEmail.Text.Trim();
            string contact = txtContact.Text.Trim();
            string password = txtPassword.Password;

            // ✅ Full validation
            if (string.IsNullOrWhiteSpace(id) ||
                string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(course) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(contact) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in all required fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Please enter a valid email address.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!contact.All(char.IsDigit))
            {
                MessageBox.Show("Contact number should contain only digits.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            

            try
            {
                // ✅ Save to MySQL
                string connStr = "server=localhost;user=root;database=sp_fms;port=3306;password=;";
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string sql = @"INSERT INTO students (id, first_name, last_name, course, email, contact, password)
                           VALUES (@id, @fname, @lname, @course, @email, @contact, @password)";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@fname", firstName);
                        cmd.Parameters.AddWithValue("@lname", lastName);
                        cmd.Parameters.AddWithValue("@course", course);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@contact", contact);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Account created successfully!", "Success");

                // ✅ Pass Student object to Dashboard
                Student student = new Student
                {
                    ID = id,
                    FullName = firstName + " " + lastName,
                    Course = course,
                    Email = email,
                    Contact = contact
                };

                Dashboard dashboard = new Dashboard(student);
                dashboard.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving account: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}