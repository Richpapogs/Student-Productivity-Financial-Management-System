namespace StudentProductivityApp
{
    partial class CreateAccountForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblStudentId = new System.Windows.Forms.Label();
            this.txtStudentId = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblFullName = new System.Windows.Forms.Label();
            this.txtFullName = new System.Windows.Forms.TextBox();

            this.lblGender = new System.Windows.Forms.Label();
            this.rbMale = new System.Windows.Forms.RadioButton();
            this.rbFemale = new System.Windows.Forms.RadioButton();

            this.lblCourse = new System.Windows.Forms.Label();
            this.txtCourse = new System.Windows.Forms.TextBox();

            this.lblAge = new System.Windows.Forms.Label();
            this.txtAge = new System.Windows.Forms.TextBox();

            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();

            this.lblContact = new System.Windows.Forms.Label();
            this.txtContact = new System.Windows.Forms.TextBox();

            this.btnCreateAccount = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();

            // 
            // Form settings
            // 
            this.ClientSize = new System.Drawing.Size(520, 420);
            this.Text = "Create Account";

            // 
            // lblTitle
            // 
            this.lblTitle.Location = new System.Drawing.Point(180, 12);
            this.lblTitle.Size = new System.Drawing.Size(160, 24);
            this.lblTitle.Text = "Create Account";

            // Student ID
            this.lblStudentId.Location = new System.Drawing.Point(40, 56);
            this.lblStudentId.Size = new System.Drawing.Size(80, 20);
            this.lblStudentId.Text = "Student ID:";
            this.txtStudentId.Location = new System.Drawing.Point(140, 54);
            this.txtStudentId.Size = new System.Drawing.Size(200, 20);

            // Password
            this.lblPassword.Location = new System.Drawing.Point(40, 86);
            this.lblPassword.Size = new System.Drawing.Size(80, 20);
            this.lblPassword.Text = "Password:";
            this.txtPassword.Location = new System.Drawing.Point(140, 84);
            this.txtPassword.Size = new System.Drawing.Size(200, 20);

            // Full name
            this.lblFullName.Location = new System.Drawing.Point(40, 116);
            this.lblFullName.Size = new System.Drawing.Size(80, 20);
            this.lblFullName.Text = "Full Name:";
            this.txtFullName.Location = new System.Drawing.Point(140, 114);
            this.txtFullName.Size = new System.Drawing.Size(320, 20);

            // Gender
            this.lblGender.Location = new System.Drawing.Point(40, 146);
            this.lblGender.Size = new System.Drawing.Size(80, 20);
            this.lblGender.Text = "Gender:";
            this.rbMale.Location = new System.Drawing.Point(140, 144);
            this.rbMale.Text = "M";
            this.rbMale.AutoSize = true;
            this.rbFemale.Location = new System.Drawing.Point(190, 144);
            this.rbFemale.Text = "F";
            this.rbFemale.AutoSize = true;

            // Course & Section
            this.lblCourse.Location = new System.Drawing.Point(40, 176);
            this.lblCourse.Size = new System.Drawing.Size(100, 20);
            this.lblCourse.Text = "Course & Section:";
            this.txtCourse.Location = new System.Drawing.Point(140, 174);
            this.txtCourse.Size = new System.Drawing.Size(320, 20);

            // Age
            this.lblAge.Location = new System.Drawing.Point(40, 206);
            this.lblAge.Size = new System.Drawing.Size(80, 20);
            this.lblAge.Text = "Age:";
            this.txtAge.Location = new System.Drawing.Point(140, 204);
            this.txtAge.Size = new System.Drawing.Size(80, 20);

            // Email
            this.lblEmail.Location = new System.Drawing.Point(40, 236);
            this.lblEmail.Size = new System.Drawing.Size(80, 20);
            this.lblEmail.Text = "Email Add:";
            this.txtEmail.Location = new System.Drawing.Point(140, 234);
            this.txtEmail.Size = new System.Drawing.Size(320, 20);

            // Contact
            this.lblContact.Location = new System.Drawing.Point(40, 266);
            this.lblContact.Size = new System.Drawing.Size(80, 20);
            this.lblContact.Text = "Contact No:";
            this.txtContact.Location = new System.Drawing.Point(140, 264);
            this.txtContact.Size = new System.Drawing.Size(200, 20);

            // Create button
            this.btnCreateAccount.Location = new System.Drawing.Point(140, 304);
            this.btnCreateAccount.Size = new System.Drawing.Size(140, 30);
            this.btnCreateAccount.Text = "Create Account";
            this.btnCreateAccount.Click += new System.EventHandler(this.btnCreateAccount_Click);

            // Status label
            this.lblStatus.Location = new System.Drawing.Point(40, 350);
            this.lblStatus.Size = new System.Drawing.Size(420, 24);

            // Add controls to form
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblStudentId);
            this.Controls.Add(this.txtStudentId);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblFullName);
            this.Controls.Add(this.txtFullName);
            this.Controls.Add(this.lblGender);
            this.Controls.Add(this.rbMale);
            this.Controls.Add(this.rbFemale);
            this.Controls.Add(this.lblCourse);
            this.Controls.Add(this.txtCourse);
            this.Controls.Add(this.lblAge);
            this.Controls.Add(this.txtAge);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.lblContact);
            this.Controls.Add(this.txtContact);
            this.Controls.Add(this.btnCreateAccount);
            this.Controls.Add(this.lblStatus);
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblStudentId;
        private System.Windows.Forms.TextBox txtStudentId;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.Label lblGender;
        private System.Windows.Forms.RadioButton rbMale;
        private System.Windows.Forms.RadioButton rbFemale;
        private System.Windows.Forms.Label lblCourse;
        private System.Windows.Forms.TextBox txtCourse;
        private System.Windows.Forms.Label lblAge;
        private System.Windows.Forms.TextBox txtAge;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblContact;
        private System.Windows.Forms.TextBox txtContact;
        private System.Windows.Forms.Button btnCreateAccount;
        private System.Windows.Forms.Label lblStatus;
    }
}
