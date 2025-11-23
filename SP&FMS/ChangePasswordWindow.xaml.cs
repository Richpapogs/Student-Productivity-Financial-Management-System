using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace SP_FMS
{
    public partial class ChangePasswordWindow : Window
    {
        private string studentId;

        public ChangePasswordWindow(string studentId)
        {
            InitializeComponent();
            this.studentId = studentId;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            // Clear previous error messages
            lblErrorMessage.Visibility = Visibility.Collapsed;
            lblErrorMessage.Text = "";

            // Get password values
            string currentPassword = txtCurrentPassword.Password;
            string newPassword = txtNewPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            // Validation
            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                ShowError("Please enter your current password.");
                return;
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                ShowError("Please enter a new password.");
                return;
            }

            if (newPassword.Length < 6)
            {
                ShowError("New password must be at least 6 characters long.");
                return;
            }

            if (newPassword != confirmPassword)
            {
                ShowError("New password and confirm password do not match.");
                return;
            }

            if (currentPassword == newPassword)
            {
                ShowError("New password must be different from current password.");
                return;
            }

            // Verify current password and update
            try
            {
                using (var conn = DBHelper.GetConnection())
                {
                    conn.Open();

                    // Get stored password hash
                    string query = "SELECT password FROM students WHERE id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", studentId);

                    string storedHash = cmd.ExecuteScalar()?.ToString();

                    if (storedHash == null)
                    {
                        ShowError("Student account not found.");
                        return;
                    }

                    // Verify current password
                    if (!PasswordHelper.VerifyPassword(currentPassword, storedHash))
                    {
                        ShowError("Current password is incorrect.");
                        return;
                    }

                    // Update password with new hash
                    string updateQuery = "UPDATE students SET password=@newPassword WHERE id=@id";
                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@newPassword", PasswordHelper.HashPassword(newPassword));
                    updateCmd.Parameters.AddWithValue("@id", studentId);

                    int rowsAffected = updateCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Password changed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    else
                    {
                        ShowError("Failed to update password. Please try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("An error occurred: " + ex.Message);
            }
        }

        private void ShowError(string message)
        {
            lblErrorMessage.Text = message;
            lblErrorMessage.Visibility = Visibility.Visible;
        }
    }
}





