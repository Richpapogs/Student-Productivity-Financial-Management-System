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
            string studentId = txtStudentId.Text;
            string password = txtPassword.Password;

            if (studentId == "12345" && password == "admin")
            {
                MessageBox.Show("Login successful!", "Welcome");

                // Open Dashboard only on success
                Dashboard dashboard = new Dashboard();
                dashboard.Show();

                // Close login window
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid Student ID or Password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void Create_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Redirecting to create account page...", "Create Account");
        }

        private void ForgotPassword_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show("Password recovery coming soon!", "Forgot Password");
        }
    }
}