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


namespace Library.Pages
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
        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            string id = txtID.Text.Trim();
            string fname = txtFName.Text.Trim();
            string lname = txtLName.Text.Trim();
            string course = txtCourse.Text.Trim();
            string email = txtEmail.Text.Trim();
            string contact = txtContact.Text.Trim(); 
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(fname) || string.IsNullOrEmpty(lname) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill in all required fields.", "Incomplete Form", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var conn = DBHelper.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO students (student_id, first_name, last_name, course_section, email, contact_number, password) " +
                                   "VALUES (@id, @fname, @lname, @course, @email, @contact, @password)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@fname", fname);
                        cmd.Parameters.AddWithValue("@lname", lname);
                        cmd.Parameters.AddWithValue("@course", course);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@contact", contact);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Account successfully created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow login = new MainWindow();
                    login.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }
    }
}
