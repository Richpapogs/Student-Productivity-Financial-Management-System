using System;
using System.Windows.Forms;
using StudentProductivityApp.Services;
using StudentProductivityApp.Models;

namespace StudentProductivityApp
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            // Ensure event handlers are connected
            btnLogin.Click += btnLogin_Click;
            btnCreate.Click += btnCreate_Click;
        }

        // ===== LOGIN BUTTON =====
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string id = txtStudentId.Text.Trim();
            string pass = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(pass))
            {
                lblStatus.Text = "Please enter your ID and password.";
                return;
            }

            var user = DataService.FindUser(id, pass);
            if (user == null)
            {
                lblStatus.Text = "Invalid Student ID or password.";
                return;
            }

            // ✅ Open Dashboard only once
            this.Hide();
            using (var dash = new DashboardForm(user))
            {
                dash.ShowDialog();
            }

            // When dashboard closes, also close login form
            this.Close();
        }

        // ===== CREATE ACCOUNT BUTTON =====
        private void btnCreate_Click(object sender, EventArgs e)
        {
            using (var createForm = new CreateAccountForm())
            {
                // Temporarily hide login form
                this.Hide();
                createForm.ShowDialog();

                // Show login form again after closing the Create Account window
                this.Show();
            }
        }
    }
}
