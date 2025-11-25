using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SP_FMS
{
    
    public partial class CreateAccountPage : Window
    {
        public CreateAccountPage()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
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

            // Basic validation
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(course) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(contact) ||
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
                using (var conn = DBHelper.GetConnection())
                {
                    conn.Open();

                    // Prevent duplicates
                    string checkSql = "SELECT COUNT(*) FROM students WHERE id=@id OR email=@email";
                    using (var checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@id", id);
                        checkCmd.Parameters.AddWithValue("@email", email);
                        int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (exists > 0)
                        {
                            MessageBox.Show("ID or Email already exists.", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }

                    // Hash password (salt + hash)
                    string saltedHash = HashPassword(password);

                    string sql = @"INSERT INTO students (id, first_name, last_name, course, email, contact, password)
                                   VALUES (@id, @fname, @lname, @course, @email, @contact, @password)";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@fname", firstName);
                        cmd.Parameters.AddWithValue("@lname", lastName);
                        cmd.Parameters.AddWithValue("@course", course);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@contact", contact);
                        cmd.Parameters.AddWithValue("@password", PasswordHelper.HashPassword(password));


                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Create Student object and open dashboard
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
        private string HashPassword(string password)
        {
            // generate a 16-byte salt
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // derive a 32-byte subkey (sha256) with 10000 iterations
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = deriveBytes.GetBytes(32);
                string saltB64 = Convert.ToBase64String(salt);
                string hashB64 = Convert.ToBase64String(hash);
                return saltB64 + ":" + hashB64;
            }
        }

        // Optional: verify (not used here but handy later)
        private bool VerifyPassword(string password, string storedSaltedHash)
        {
            if (string.IsNullOrEmpty(storedSaltedHash)) return false;
            var parts = storedSaltedHash.Split(':');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHash = Convert.FromBase64String(parts[1]);

            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] testHash = deriveBytes.GetBytes(32);
                return testHash.SequenceEqual(storedHash);
            }
        }
    }
}
