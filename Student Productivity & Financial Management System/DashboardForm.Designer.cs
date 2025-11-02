namespace StudentProductivityApp
{
    partial class DashboardForm
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
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabProfile = new System.Windows.Forms.TabPage();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lblProfileName = new System.Windows.Forms.Label();
            this.lblProfileId = new System.Windows.Forms.Label();
            this.lblCourse = new System.Windows.Forms.Label();
            this.lblAge = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblContact = new System.Windows.Forms.Label();
            this.btnLogout = new System.Windows.Forms.Button();

            this.tabTodo = new System.Windows.Forms.TabPage();
            this.txtNewTask = new System.Windows.Forms.TextBox();
            this.lbTodos = new System.Windows.Forms.CheckedListBox();
            this.lbCompleted = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnRecord = new System.Windows.Forms.Button();
            this.lblTodoTitle = new System.Windows.Forms.Label();
            this.lblCompletedTitle = new System.Windows.Forms.Label();

            this.tabFinance = new System.Windows.Forms.TabPage();
            this.lblBudget = new System.Windows.Forms.Label();
            this.txtBudget = new System.Windows.Forms.TextBox();
            this.btnSetBudget = new System.Windows.Forms.Button();
            this.lblExpenseName = new System.Windows.Forms.Label();
            this.txtExpenseName = new System.Windows.Forms.TextBox();
            this.lblExpenseAmount = new System.Windows.Forms.Label();
            this.txtExpenseAmount = new System.Windows.Forms.TextBox();
            this.btnAddExpense = new System.Windows.Forms.Button();
            this.lbExpenses = new System.Windows.Forms.ListBox();
            this.lblRemaining = new System.Windows.Forms.Label();
            this.btnAddRow = new System.Windows.Forms.Button();
            this.btnRemoveRow = new System.Windows.Forms.Button();
            this.btnRecordFinance = new System.Windows.Forms.Button();

            this.tabMain.SuspendLayout();
            this.tabProfile.SuspendLayout();
            this.tabTodo.SuspendLayout();
            this.tabFinance.SuspendLayout();
            this.SuspendLayout();

            // tabMain
            this.tabMain.Controls.Add(this.tabProfile);
            this.tabMain.Controls.Add(this.tabTodo);
            this.tabMain.Controls.Add(this.tabFinance);
            this.tabMain.Location = new System.Drawing.Point(12, 12);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(960, 580);
            this.tabMain.TabIndex = 0;

            // tabProfile - new layout (no tasks or total expenses)
            this.tabProfile.Controls.Add(this.lblWelcome);
            this.tabProfile.Controls.Add(this.lblProfileName);
            this.tabProfile.Controls.Add(this.lblProfileId);
            this.tabProfile.Controls.Add(this.lblCourse);
            this.tabProfile.Controls.Add(this.lblAge);
            this.tabProfile.Controls.Add(this.lblEmail);
            this.tabProfile.Controls.Add(this.lblContact);
            this.tabProfile.Controls.Add(this.btnLogout);
            this.tabProfile.Location = new System.Drawing.Point(4, 22);
            this.tabProfile.Name = "tabProfile";
            this.tabProfile.Padding = new System.Windows.Forms.Padding(3);
            this.tabProfile.Size = new System.Drawing.Size(952, 554);
            this.tabProfile.TabIndex = 0;
            this.tabProfile.Text = "Profile";
            this.tabProfile.UseVisualStyleBackColor = true;

            this.lblWelcome.Location = new System.Drawing.Point(40, 40);
            this.lblWelcome.Size = new System.Drawing.Size(400, 20);
            this.lblWelcome.Text = "Welcome,";

            this.lblProfileName.Location = new System.Drawing.Point(40, 80);
            this.lblProfileName.Size = new System.Drawing.Size(760, 40);
            this.lblProfileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);

            this.lblProfileId.Location = new System.Drawing.Point(40, 140);
            this.lblProfileId.Size = new System.Drawing.Size(400, 20);

            this.lblCourse.Location = new System.Drawing.Point(40, 180);
            this.lblCourse.Size = new System.Drawing.Size(600, 20);

            this.lblAge.Location = new System.Drawing.Point(40, 220);
            this.lblAge.Size = new System.Drawing.Size(200, 20);

            this.lblEmail.Location = new System.Drawing.Point(40, 260);
            this.lblEmail.Size = new System.Drawing.Size(400, 20);

            this.lblContact.Location = new System.Drawing.Point(40, 300);
            this.lblContact.Size = new System.Drawing.Size(300, 20);

            this.btnLogout.Location = new System.Drawing.Point(40, 360);
            this.btnLogout.Size = new System.Drawing.Size(100, 30);
            this.btnLogout.Text = "Logout";
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);

            // tabTodo (keeps your existing todo layout)
            this.tabTodo.Controls.Add(this.txtNewTask);
            this.tabTodo.Controls.Add(this.lbTodos);
            this.tabTodo.Controls.Add(this.lbCompleted);
            this.tabTodo.Controls.Add(this.btnAdd);
            this.tabTodo.Controls.Add(this.btnRemove);
            this.tabTodo.Controls.Add(this.btnBack);
            this.tabTodo.Controls.Add(this.btnRecord);
            this.tabTodo.Controls.Add(this.lblTodoTitle);
            this.tabTodo.Controls.Add(this.lblCompletedTitle);
            this.tabTodo.Location = new System.Drawing.Point(4, 22);
            this.tabTodo.Name = "tabTodo";
            this.tabTodo.Size = new System.Drawing.Size(952, 554);
            this.tabTodo.Text = "To-Do List";
            this.tabTodo.UseVisualStyleBackColor = true;

            this.lblTodoTitle.Location = new System.Drawing.Point(60, 30);
            this.lblTodoTitle.Size = new System.Drawing.Size(150, 20);
            this.lblTodoTitle.Text = "TO DO LIST HERE";

            this.lbTodos.Location = new System.Drawing.Point(60, 60);
            this.lbTodos.Size = new System.Drawing.Size(300, 300);

            this.txtNewTask.Location = new System.Drawing.Point(60, 380);
            this.txtNewTask.Size = new System.Drawing.Size(300, 20);

            this.btnAdd.Location = new System.Drawing.Point(380, 378);
            this.btnAdd.Size = new System.Drawing.Size(40, 24);
            this.btnAdd.Text = "+";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);

            this.btnRemove.Location = new System.Drawing.Point(60, 410);
            this.btnRemove.Size = new System.Drawing.Size(100, 30);
            this.btnRemove.Text = "Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);

            this.btnRecord.Location = new System.Drawing.Point(170, 410);
            this.btnRecord.Size = new System.Drawing.Size(100, 30);
            this.btnRecord.Text = "Record";
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);

            this.lblCompletedTitle.Location = new System.Drawing.Point(520, 30);
            this.lblCompletedTitle.Size = new System.Drawing.Size(150, 20);
            this.lblCompletedTitle.Text = "COMPLETED TASK:";

            this.lbCompleted.Location = new System.Drawing.Point(520, 60);
            this.lbCompleted.Size = new System.Drawing.Size(360, 300);
            this.lbCompleted.Visible = false;

            this.btnBack.Location = new System.Drawing.Point(520, 378);
            this.btnBack.Size = new System.Drawing.Size(100, 30);
            this.btnBack.Text = "Back";
            this.btnBack.Visible = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);

            // tabFinance (keeps your existing finance layout)
            this.tabFinance.Controls.Add(this.lblBudget);
            this.tabFinance.Controls.Add(this.txtBudget);
            this.tabFinance.Controls.Add(this.btnSetBudget);
            this.tabFinance.Controls.Add(this.lblExpenseName);
            this.tabFinance.Controls.Add(this.txtExpenseName);
            this.tabFinance.Controls.Add(this.lblExpenseAmount);
            this.tabFinance.Controls.Add(this.txtExpenseAmount);
            this.tabFinance.Controls.Add(this.btnAddExpense);
            this.tabFinance.Controls.Add(this.lbExpenses);
            this.tabFinance.Controls.Add(this.lblRemaining);
            this.tabFinance.Controls.Add(this.btnAddRow);
            this.tabFinance.Controls.Add(this.btnRemoveRow);
            this.tabFinance.Controls.Add(this.btnRecordFinance);
            this.tabFinance.Location = new System.Drawing.Point(4, 22);
            this.tabFinance.Name = "tabFinance";
            this.tabFinance.Size = new System.Drawing.Size(952, 554);
            this.tabFinance.Text = "Financial Tracker";
            this.tabFinance.UseVisualStyleBackColor = true;

            this.lblBudget.Location = new System.Drawing.Point(60, 40);
            this.lblBudget.Size = new System.Drawing.Size(100, 20);
            this.lblBudget.Text = "Set Budget:";

            this.txtBudget.Location = new System.Drawing.Point(160, 38);
            this.txtBudget.Size = new System.Drawing.Size(100, 20);

            this.btnSetBudget.Location = new System.Drawing.Point(270, 36);
            this.btnSetBudget.Size = new System.Drawing.Size(90, 25);
            this.btnSetBudget.Text = "Save Budget";
            this.btnSetBudget.Click += new System.EventHandler(this.btnSetBudget_Click);

            this.lblExpenseName.Location = new System.Drawing.Point(60, 90);
            this.lblExpenseName.Size = new System.Drawing.Size(120, 20);
            this.lblExpenseName.Text = "Expense Name:";

            this.txtExpenseName.Location = new System.Drawing.Point(180, 88);
            this.txtExpenseName.Size = new System.Drawing.Size(120, 20);

            this.lblExpenseAmount.Location = new System.Drawing.Point(320, 90);
            this.lblExpenseAmount.Size = new System.Drawing.Size(54, 20);
            this.lblExpenseAmount.Text = "Amount:";

            this.txtExpenseAmount.Location = new System.Drawing.Point(380, 88);
            this.txtExpenseAmount.Size = new System.Drawing.Size(80, 20);

            this.btnAddExpense.Location = new System.Drawing.Point(470, 86);
            this.btnAddExpense.Size = new System.Drawing.Size(100, 25);
            this.btnAddExpense.Text = "Add Expense";
            this.btnAddExpense.Click += new System.EventHandler(this.btnAddExpense_Click);

            this.lbExpenses.Location = new System.Drawing.Point(60, 150);
            this.lbExpenses.Size = new System.Drawing.Size(480, 220);

            this.lblRemaining.Location = new System.Drawing.Point(60, 400);
            this.lblRemaining.Size = new System.Drawing.Size(300, 25);
            this.lblRemaining.Text = "Remaining: ₱0.00";

            this.btnAddRow.Location = new System.Drawing.Point(560, 150);
            this.btnAddRow.Size = new System.Drawing.Size(40, 30);
            this.btnAddRow.Text = "+";
            this.btnAddRow.Click += new System.EventHandler(this.btnAddRow_Click);

            this.btnRemoveRow.Location = new System.Drawing.Point(610, 150);
            this.btnRemoveRow.Size = new System.Drawing.Size(40, 30);
            this.btnRemoveRow.Text = "-";
            this.btnRemoveRow.Enabled = false;
            this.btnRemoveRow.Click += new System.EventHandler(this.btnRemoveRow_Click);

            this.btnRecordFinance.Location = new System.Drawing.Point(560, 200);
            this.btnRecordFinance.Size = new System.Drawing.Size(120, 30);
            this.btnRecordFinance.Text = "Record";
            this.btnRecordFinance.Click += new System.EventHandler(this.btnRecordFinance_Click);

            // Form
            this.ClientSize = new System.Drawing.Size(984, 611);
            this.Controls.Add(this.tabMain);
            this.Name = "DashboardForm";
            this.Text = "Student Dashboard";

            this.tabMain.ResumeLayout(false);
            this.tabProfile.ResumeLayout(false);
            this.tabTodo.ResumeLayout(false);
            this.tabTodo.PerformLayout();
            this.tabFinance.ResumeLayout(false);
            this.tabFinance.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabProfile;
        private System.Windows.Forms.TabPage tabTodo;
        private System.Windows.Forms.TabPage tabFinance;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label lblProfileName;
        private System.Windows.Forms.Label lblProfileId;
        private System.Windows.Forms.Label lblCourse;
        private System.Windows.Forms.Label lblAge;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblContact;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.CheckedListBox lbTodos;
        private System.Windows.Forms.ListBox lbCompleted;
        private System.Windows.Forms.TextBox txtNewTask;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnRecord;
        private System.Windows.Forms.Label lblTodoTitle;
        private System.Windows.Forms.Label lblCompletedTitle;
        private System.Windows.Forms.Label lblBudget;
        private System.Windows.Forms.TextBox txtBudget;
        private System.Windows.Forms.Button btnSetBudget;
        private System.Windows.Forms.Label lblExpenseName;
        private System.Windows.Forms.TextBox txtExpenseName;
        private System.Windows.Forms.Label lblExpenseAmount;
        private System.Windows.Forms.TextBox txtExpenseAmount;
        private System.Windows.Forms.Button btnAddExpense;
        private System.Windows.Forms.ListBox lbExpenses;
        private System.Windows.Forms.Label lblRemaining;
        private System.Windows.Forms.Button btnAddRow;
        private System.Windows.Forms.Button btnRemoveRow;
        private System.Windows.Forms.Button btnRecordFinance;
    }
}
