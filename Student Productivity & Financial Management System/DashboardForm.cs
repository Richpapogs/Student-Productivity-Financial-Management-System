using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using StudentProductivityApp.Models;
using StudentProductivityApp.Services;

namespace StudentProductivityApp
{
    public partial class DashboardForm : Form
    {
        private User CurrentUser;
        private const int MaxTodos = 10;
        private const int MinTodos = 1;

        // Finance dynamic fields
        private List<TextBox> expectedBoxes = new List<TextBox>();
        private List<TextBox> actualBoxes = new List<TextBox>();
        private const int MaxFinanceRows = 6;
        private const int MinFinanceRows = 1;

        public DashboardForm(User user)
        {
            InitializeComponent();
            CurrentUser = user ?? throw new ArgumentNullException(nameof(user));

            // Hook up events (just in case Designer missed them)
            btnAdd.Click += btnAdd_Click;
            btnRemove.Click += btnRemove_Click;
            btnRecord.Click += btnRecord_Click;
            btnBack.Click += btnBack_Click;
            btnLogout.Click += btnLogout_Click;

            btnSetBudget.Click += btnSetBudget_Click;
            btnAddExpense.Click += btnAddExpense_Click;
            btnAddRow.Click += btnAddRow_Click;
            btnRemoveRow.Click += btnRemoveRow_Click;
            btnRecordFinance.Click += btnRecordFinance_Click;

            // Initialization
            LoadProfile();
            LoadTodosFromModel();
            LoadFinancesFromModel();
            InitFinanceDynamicControls();
            UpdateTodoButtons();
        }

        // ---------------- PROFILE ----------------
        private void LoadProfile()
        {
            lblWelcome.Text = $"Welcome, {CurrentUser.FullName}";
            lblProfileName.Text = $"Full Name: {CurrentUser.FullName} ({CurrentUser.Gender})";
            lblProfileId.Text = $"Student No.: {CurrentUser.StudentId}";
            lblCourse.Text = $"Course & Section: {CurrentUser.CourseSection}";
            lblAge.Text = $"Age: {CurrentUser.Age}";
            lblEmail.Text = $"Email Address: {CurrentUser.Email}";
            lblContact.Text = $"Contact No.: {CurrentUser.Contact}";
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ---------------- TO-DO ----------------
        private void LoadTodosFromModel()
        {
            lbTodos.Items.Clear();
            lbCompleted.Items.Clear();

            foreach (var t in CurrentUser.Todos.OrderBy(x => x.Id))
            {
                if (t.IsCompleted) lbCompleted.Items.Add($"{t.Id}. {t.Task}");
                else lbTodos.Items.Add($"{t.Id}. {t.Task}");
            }

            lbCompleted.Visible = lbCompleted.Items.Count > 0;
            btnBack.Visible = lbCompleted.Items.Count > 0;

            UpdateTodoButtons();
            LoadProfile();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var text = txtNewTask.Text?.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Please enter a task.");
                return;
            }

            if (CurrentUser.Todos.Count >= MaxTodos)
            {
                MessageBox.Show($"Maximum {MaxTodos} tasks allowed.");
                return;
            }

            int newId = (CurrentUser.Todos.Any() ? CurrentUser.Todos.Max(x => x.Id) : 0) + 1;
            CurrentUser.Todos.Add(new TodoItem { Id = newId, Task = text, IsCompleted = false });

            txtNewTask.Clear();
            LoadTodosFromModel();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lbTodos.SelectedItem == null)
            {
                MessageBox.Show("Select a task to remove (it will move to Completed).");
                return;
            }

            var selected = lbTodos.SelectedItem.ToString();
            if (!int.TryParse(selected.Split('.')[0], out int id)) return;

            var item = CurrentUser.Todos.FirstOrDefault(t => t.Id == id && !t.IsCompleted);
            if (item == null) return;

            item.IsCompleted = true;
            DataService.UpdateUser(CurrentUser);
            LoadTodosFromModel();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (lbCompleted.SelectedItem == null)
            {
                MessageBox.Show("Select a completed task to move back.");
                return;
            }

            var selected = lbCompleted.SelectedItem.ToString();
            if (!int.TryParse(selected.Split('.')[0], out int id)) return;

            var item = CurrentUser.Todos.FirstOrDefault(t => t.Id == id && t.IsCompleted);
            if (item == null) return;

            item.IsCompleted = false;
            DataService.UpdateUser(CurrentUser);
            LoadTodosFromModel();
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            DataService.UpdateUser(CurrentUser);
            MessageBox.Show("To-Do list saved.");
            LoadProfile();
        }

        private void UpdateTodoButtons()
        {
            btnAdd.Enabled = CurrentUser.Todos.Count < MaxTodos;
            btnRemove.Enabled = CurrentUser.Todos.Count > MinTodos;
        }

        // ---------------- FINANCE ----------------
        private void LoadFinancesFromModel()
        {
            lbExpenses.Items.Clear();
            foreach (var e in CurrentUser.Finances.OrderBy(f => f.Date))
                lbExpenses.Items.Add($"{e.Date:yyyy-MM-dd} - {e.Name}: ₱{e.Amount:N2}");

            var spent = CurrentUser.Finances.Sum(f => f.Amount);
            var remaining = CurrentUser.Budget - spent;
            lblRemaining.Text = $"Remaining: ₱{remaining:N2}";
        }

        private void btnSetBudget_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtBudget.Text, out decimal b))
            {
                MessageBox.Show("Enter a valid budget number.");
                return;
            }

            CurrentUser.Budget = b;
            DataService.UpdateUser(CurrentUser);
            LoadFinancesFromModel();
            MessageBox.Show("Budget saved.");
        }

        private void btnAddExpense_Click(object sender, EventArgs e)
        {
            var name = txtExpenseName.Text?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Enter expense name.");
                return;
            }

            if (!decimal.TryParse(txtExpenseAmount.Text, out decimal amount))
            {
                MessageBox.Show("Enter a valid amount.");
                return;
            }

            CurrentUser.Finances.Add(new FinanceEntry { Name = name, Amount = amount, Date = DateTime.Now });
            DataService.UpdateUser(CurrentUser);
            txtExpenseName.Clear();
            txtExpenseAmount.Clear();
            LoadFinancesFromModel();
        }

        // Initialize dynamic finance controls
        private void InitFinanceDynamicControls()
        {
            expectedBoxes.Add(txtExpenseName);
            actualBoxes.Add(txtExpenseAmount);
            btnRemoveRow.Enabled = false;
        }

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            if (expectedBoxes.Count >= MaxFinanceRows)
            {
                MessageBox.Show("Maximum 6 rows reached.");
                return;
            }

            TextBox lastExp = expectedBoxes.Last();
            TextBox lastAct = actualBoxes.Last();
            int newY = lastExp.Location.Y + 30;

            TextBox newExp = new TextBox
            {
                Name = $"txtExpected_{expectedBoxes.Count + 1}",
                Location = new System.Drawing.Point(lastExp.Location.X, newY),
                Size = lastExp.Size
            };

            TextBox newAct = new TextBox
            {
                Name = $"txtActual_{actualBoxes.Count + 1}",
                Location = new System.Drawing.Point(lastAct.Location.X, newY),
                Size = lastAct.Size
            };

            tabFinance.Controls.Add(newExp);
            tabFinance.Controls.Add(newAct);
            expectedBoxes.Add(newExp);
            actualBoxes.Add(newAct);

            btnRemoveRow.Enabled = expectedBoxes.Count > MinFinanceRows;
            btnAddRow.Enabled = expectedBoxes.Count < MaxFinanceRows;
        }

        private void btnRemoveRow_Click(object sender, EventArgs e)
        {
            if (expectedBoxes.Count <= MinFinanceRows)
            {
                MessageBox.Show("At least one row must remain.");
                return;
            }

            var lastExp = expectedBoxes.Last();
            var lastAct = actualBoxes.Last();

            tabFinance.Controls.Remove(lastExp);
            tabFinance.Controls.Remove(lastAct);
            expectedBoxes.Remove(lastExp);
            actualBoxes.Remove(lastAct);

            btnRemoveRow.Enabled = expectedBoxes.Count > MinFinanceRows;
            btnAddRow.Enabled = expectedBoxes.Count < MaxFinanceRows;
        }

        private void btnRecordFinance_Click(object sender, EventArgs e)
        {
            decimal totalExpected = 0, totalActual = 0;

            foreach (var b in expectedBoxes)
                if (decimal.TryParse(b.Text, out decimal vE)) totalExpected += vE;

            foreach (var b in actualBoxes)
                if (decimal.TryParse(b.Text, out decimal vA)) totalActual += vA;

            CurrentUser.Finances.Add(new FinanceEntry
            {
                Name = "Overall",
                Amount = totalActual,
                Date = DateTime.Now
            });

            DataService.UpdateUser(CurrentUser);
            MessageBox.Show($"Recorded! Expected ₱{totalExpected:N2} | Actual ₱{totalActual:N2}");
            LoadFinancesFromModel();
        }
    }
}
