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
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        private Student _student;
        public Dashboard(Student student)
        {
            InitializeComponent();
            _student = student;

            // Update UI labels
            lblWelcome.Text = $"Welcome, {_student.FullName}!";
            lblDetails.Text = $"ID: {_student.ID}\nCourse: {_student.Course}\nEmail: {_student.Email}\nContact: {_student.Contact}";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
