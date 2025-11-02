using System;
using System.Windows.Forms;
using StudentProductivityApp.Models;
using StudentProductivityApp.Services;

namespace StudentProductivityApp
{
    public partial class CreateAccountForm : Form
    {
        public CreateAccountForm()
        {
            InitializeComponent();
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            // Collect values from textboxes
            var id = txtStudentId.Text.Trim();
            var pass = txtPassword.Text;
            var name = txtFullName.Text.Trim();
            var gender = rbMale.Checked ? "M" : rbFemale.Checked ? "F" : "";
            var course = txtCourse.Text.Trim();
            var ageText = txtAge.Text.Trim();
            var email = txtEmail.Text.Trim();
            var contact = txtContact.Text.Trim();

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(pass) ||
                string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(gender) ||
                string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(ageText) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(contact))
            {
                lblStatus.Text = "Please fill in all fields.";
                return;
            }

            if (!int.TryParse(ageText, out int age))
            {
                lblStatus.Text = "Please enter a valid age.";
                return;
            }

            var user = new User
            {
                StudentId = id,
                Password = pass,
                FullName = name,
                Gender = gender,
                CourseSection = course,
                Age = age,
                Email = email,
                Contact = contact
            };

            // Add user
            if (!DataService.AddUser(user))
            {
                lblStatus.Text = "Student ID already exists!";
                return;
            }

            lblStatus.Text = "Account created successfully!";
            MessageBox.Show("Account created! You can now log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}
