using MySql.Data.MySqlClient;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace SP_FMS
{
    public partial class ForgotPasswordWindow : Window
    {
        public ForgotPasswordWindow()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            // Clear previous messages
            lblErrorMessage.Visibility = Visibility.Collapsed;
            borderSuccess.Visibility = Visibility.Collapsed;
            lblErrorMessage.Text = "";

            string studentId = txtStudentId.Text.Trim();
            string email = txtEmail.Text.Trim();

            // Validation
            if (string.IsNullOrWhiteSpace(studentId))
            {
                ShowError("Please enter your Student ID.");
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Please enter your Email.");
                return;
            }

            // Verify Student ID and Email match
            try
            {
                using (var conn = DBHelper.GetConnection())
                {
                    conn.Open();

                    // Check if Student ID and Email match
                    string query = "SELECT id, email FROM students WHERE id=@id AND email=@email";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", studentId);
                    cmd.Parameters.AddWithValue("@email", email);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            ShowError("Student ID and Email do not match. Please verify your information.");
                            return;
                        }
                    }

                    // Generate temporary password (8 characters: mix of letters and numbers)
                    string tempPassword = GenerateTemporaryPassword();

                    // Update password in database
                    string updateQuery = "UPDATE students SET password=@newPassword WHERE id=@id";
                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@newPassword", PasswordHelper.HashPassword(tempPassword));
                    updateCmd.Parameters.AddWithValue("@id", studentId);

                    int rowsAffected = updateCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Show success message with temporary password
                        lblTempPassword.Text = tempPassword;
                        borderSuccess.Visibility = Visibility.Visible;
                        
                        // Disable inputs and button after success
                        txtStudentId.IsEnabled = false;
                        txtEmail.IsEnabled = false;
                        (sender as System.Windows.Controls.Button).IsEnabled = false;
                    }
                    else
                    {
                        ShowError("Failed to generate temporary password. Please try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("An error occurred: " + ex.Message);
            }
        }

        private string GenerateTemporaryPassword()
        {
            // Generate a random 5-digit password (numbers only)
            StringBuilder password = new StringBuilder();
            
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[5];
                rng.GetBytes(randomBytes);

                for (int i = 0; i < 5; i++)
                {
                    // Generate a digit from 0-9
                    int digit = randomBytes[i] % 10;
                    password.Append(digit);
                }
            }

            return password.ToString();
        }

        private void ShowError(string message)
        {
            lblErrorMessage.Text = message;
            lblErrorMessage.Visibility = Visibility.Visible;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Close the Forgot Password window and return to MainWindow
            this.Close();
        }
    }
}

