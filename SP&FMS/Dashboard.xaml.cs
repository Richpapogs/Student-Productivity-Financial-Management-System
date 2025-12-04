using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SP_FMS
{
    public partial class Dashboard : Window
    {
        private string studentId;       // for To-Do List and database queries
        private Student currentStudent; // full student info if needed

        // ✅ Constructor that accepts a Student object
        public Dashboard(Student student)
        {
            InitializeComponent();

            currentStudent = student;       // store full student info
            studentId = student.ID;         // needed for tasks and progress

            // Set welcome info
            lblWelcome.Text = "Welcome, " + currentStudent.FullName;
            lblDetails.Text = $"Course: {currentStudent.Course}\nEmail: {currentStudent.Email}\nContact: {currentStudent.Contact}";
            lblCourse.Text = currentStudent.Course;
            lblEmail.Text = currentStudent.Email;
            lblContact.Text = currentStudent.Contact;

            // Watermark behavior
            txtNewTask.GotFocus += TxtNewTask_GotFocus;
            txtNewTask.LostFocus += TxtNewTask_LostFocus;
            txtBudget.GotFocus += TxtBudget_GotFocus;
            txtBudget.LostFocus += TxtBudget_LostFocus;
            txtExpenseCategory.GotFocus += TxtExpenseCategory_GotFocus;
            txtExpenseCategory.LostFocus += TxtExpenseCategory_LostFocus;
            txtCost.GotFocus += TxtCost_GotFocus;
            txtCost.LostFocus += TxtCost_LostFocus;

            EnsureCompletionDateColumn();   // ensure completion_date column exists
            EnsureCreatedDateColumn();      // ensure created_date column exists
            EnsureFinancialTables();        // ensure financial tables exist
            EnsureRecordsTable();           // ensure weekly_records table exists
            EnsureWeeklyRecordBalanceColumn(); // add total_balance column if missing
            CreateWeeklyRecordIfNeeded();   // snapshot last 7 days into weekly_records if needed
            CleanupOldUncompletedTasks();   // remove uncompleted tasks older than current week
            CleanupOldCompletedTasks();     // remove completed tasks older than 7 days
            LoadTasks();                    // load tasks for this student
            LoadCompletedTasks();           // load completed tasks for this student
            CleanupOldTodoProgress();       // remove todo_progress records older than 7 days
            CleanupOldExpenses();           // remove expenses older than 7 days
            UpdatePieChart();               // update pie chart with 7-day statistics
            LoadExpenses();                 // load expenses for this student (today only)
            LoadBudget();                   // load budget for this student
            UpdateRemainingBudget();        // calculate and display remaining budget
            UpdateExpensePieChart();        // update expense pie chart with 7-day statistics
            UpdateRecordsTab();             // update 7-day records tab
            LoadWeeklyRecords();            // load weekly records rows
            if (colWeek != null)
            {
                colWeek.Header = "Week (" + DateTime.Today.Year + ")";
            }
            EnsureNotesColumn();
            LoadNotes();
        }

        #region To-Do List Methods

        private void LoadTasks()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                string query = "SELECT task_id, task_name, is_completed FROM todo_tasks WHERE student_id=@id AND is_completed=0";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", studentId);

                List<TodoTask> tasks = new List<TodoTask>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new TodoTask
                        {
                            task_id = reader.GetInt32("task_id"),
                            task_name = reader.GetString("task_name"),
                            is_completed = reader.GetBoolean("is_completed")
                        });
                    }
                }

                dgTodo.ItemsSource = tasks;
            }
        }

        private void SaveTodoChanges_Click(object sender, RoutedEventArgs e)
        {
            if (dgTodo.ItemsSource is not List<TodoTask> tasks) return;

            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                // Ensure completion_date column exists
                EnsureCompletionDateColumn();

                foreach (var task in tasks)
                {
                    // Check if task was previously not completed and is now being marked as completed
                    string checkQuery = "SELECT is_completed FROM todo_tasks WHERE task_id=@id";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@id", task.task_id);
                    object result = checkCmd.ExecuteScalar();
                    bool wasCompleted = result != null && Convert.ToInt32(result) == 1;

                    string query;
                    if (task.is_completed && !wasCompleted)
                    {
                        // Task is being marked as completed - set completion_date to today
                        query = "UPDATE todo_tasks SET is_completed=@completed, completion_date=CURDATE() WHERE task_id=@id";
                    }
                    else if (!task.is_completed && wasCompleted)
                    {
                        // Task is being unmarked - clear completion_date
                        query = "UPDATE todo_tasks SET is_completed=@completed, completion_date=NULL WHERE task_id=@id";
                    }
                    else
                    {
                        // Just update the completion status
                        query = "UPDATE todo_tasks SET is_completed=@completed WHERE task_id=@id";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@completed", task.is_completed ? 1 : 0);
                    cmd.Parameters.AddWithValue("@id", task.task_id);
                    cmd.ExecuteNonQuery();
                }
            }

            SaveProgress();
            LoadTasks();
            LoadCompletedTasks();
            CleanupOldCompletedTasks(); // Clean up old tasks after saving
            UpdatePieChart(); // Update pie chart after saving changes
            UpdateRecordsTab();
            LoadWeeklyRecords();
            EnsureWeeklyRecordForDate(DateTime.Today);
        }



        #endregion

        #region Watermark TextBox

        private void TxtNewTask_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtNewTask.Text == "Enter task...")
            {
                txtNewTask.Text = "";
                txtNewTask.Foreground = Brushes.Black;
            }
        }

        private void TxtNewTask_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewTask.Text))
            {
                txtNewTask.Text = "Enter task...";
                txtNewTask.Foreground = Brushes.Gray;
            }
        }

        #endregion

        #region Log Out / Back Button

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SaveNotes();
            // Open the login/main window
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            // Close current dashboard
            this.Close();
        }

        #endregion

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void LoadCompletedTasks()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                string query = "SELECT task_id, task_name FROM todo_tasks WHERE student_id=@id AND is_completed=1";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", studentId);

                List<TodoTask> completed = new List<TodoTask>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        completed.Add(new TodoTask
                        {
                            task_id = reader.GetInt32("task_id"),
                            task_name = reader.GetString("task_name")
                        });
                    }
                }

                lstCompletedTasks.ItemsSource = completed;
            }
        }
        private void lstCompletedTasks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstCompletedTasks.SelectedItem == null) return;

            var task = lstCompletedTasks.SelectedItem as TodoTask;

            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                string query = "UPDATE todo_tasks SET is_completed=0 WHERE task_id=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", task.task_id);

                cmd.ExecuteNonQuery();
            }

            LoadTasks();
            LoadCompletedTasks();

            MessageBox.Show("Task moved back to To-Do.");
        }
        private void RestoreTask_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstCompletedTasks.SelectedItem is not TodoTask task) return;

            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                EnsureCompletionDateColumn();

                // Restore task and clear completion_date
                string query = "UPDATE todo_tasks SET is_completed=0, completion_date=NULL WHERE task_id=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", task.task_id);
                cmd.ExecuteNonQuery();
            }

            LoadTasks();
            LoadCompletedTasks();
            UpdatePieChart(); // Update pie chart after restoring task
            SaveProgressSilent(); // Automatically update todo_progress when restoring task
            UpdateRecordsTab();
        }


        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewTask.Text) || txtNewTask.Text == "Enter task...")
            {
                MessageBox.Show("Please enter a task.");
                return;
            }

            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                EnsureCreatedDateColumn();

                string query = "INSERT INTO todo_tasks(student_id, task_name, is_completed, created_date) VALUES (@id, @task, 0, CURDATE())";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", studentId);
                cmd.Parameters.AddWithValue("@task", txtNewTask.Text);
                cmd.ExecuteNonQuery();
            }

            txtNewTask.Text = "Enter task...";
            txtNewTask.Foreground = Brushes.Gray;

            LoadTasks();
            UpdatePieChart(); // Update pie chart after adding task
            SaveProgressSilent(); // Automatically update todo_progress when adding task
            UpdateRecordsTab();
            LoadWeeklyRecords();
            EnsureWeeklyRecordForDate(DateTime.Today);
        }

        private void RemoveTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.DataContext is not TodoTask task) return;

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to remove the task '{task.task_name}'?",
                "Remove Task",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                string query = "DELETE FROM todo_tasks WHERE task_id=@taskId AND student_id=@studentId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@taskId", task.task_id);
                cmd.Parameters.AddWithValue("@studentId", studentId);
                cmd.ExecuteNonQuery();
            }

            LoadTasks();
            LoadCompletedTasks();
            UpdatePieChart();
            SaveProgressSilent();
            UpdateRecordsTab();
            LoadWeeklyRecords();
            EnsureWeeklyRecordForDate(DateTime.Today);

            MessageBox.Show("Task removed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveProgress()
        {
            SaveProgressSilent();
            // Show notification
            MessageBox.Show("Progress saved! 🎯", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveProgressSilent()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                // Count all tasks
                string queryTotal = "SELECT COUNT(*) FROM todo_tasks WHERE student_id=@id";
                MySqlCommand cmdTotal = new MySqlCommand(queryTotal, conn);
                cmdTotal.Parameters.AddWithValue("@id", studentId);
                int totalTasks = Convert.ToInt32(cmdTotal.ExecuteScalar());

                // Count completed tasks
                string queryCompleted = "SELECT COUNT(*) FROM todo_tasks WHERE student_id=@id AND is_completed=1";
                MySqlCommand cmdCompleted = new MySqlCommand(queryCompleted, conn);
                cmdCompleted.Parameters.AddWithValue("@id", studentId);
                int completedTasks = Convert.ToInt32(cmdCompleted.ExecuteScalar());

                // Check if a record already exists for today
                string checkQuery = "SELECT COUNT(*) FROM todo_progress WHERE student_id=@id AND date_recorded=CURDATE()";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@id", studentId);
                int existingRecords = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (existingRecords > 0)
                {
                    // Update existing record instead of inserting
                    string updateQuery = "UPDATE todo_progress SET completed_tasks=@completed, total_tasks=@total WHERE student_id=@id AND date_recorded=CURDATE()";
                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@id", studentId);
                    updateCmd.Parameters.AddWithValue("@completed", completedTasks);
                    updateCmd.Parameters.AddWithValue("@total", totalTasks);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    // Insert new record only if it doesn't exist
                    string insertQuery = "INSERT INTO todo_progress(student_id, date_recorded, completed_tasks, total_tasks) VALUES(@id, CURDATE(), @completed, @total)";
                    MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@id", studentId);
                    insertCmd.Parameters.AddWithValue("@completed", completedTasks);
                    insertCmd.Parameters.AddWithValue("@total", totalTasks);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }

        private void EnsureCompletionDateColumn()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                // Check if completion_date column exists
                string checkQuery = @"SELECT COUNT(*) 
                                      FROM INFORMATION_SCHEMA.COLUMNS 
                                      WHERE TABLE_SCHEMA = DATABASE() 
                                      AND TABLE_NAME = 'todo_tasks' 
                                      AND COLUMN_NAME = 'completion_date'";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                int columnExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (columnExists == 0)
                {
                    // Column doesn't exist, add it
                    string alterQuery = "ALTER TABLE todo_tasks ADD COLUMN completion_date DATE NULL";
                    MySqlCommand alterCmd = new MySqlCommand(alterQuery, conn);
                    alterCmd.ExecuteNonQuery();
                }
            }
        }

        private void EnsureCreatedDateColumn()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();
                string checkQuery = @"SELECT COUNT(*) 
                                      FROM INFORMATION_SCHEMA.COLUMNS 
                                      WHERE TABLE_SCHEMA = DATABASE() 
                                      AND TABLE_NAME = 'todo_tasks' 
                                      AND COLUMN_NAME = 'created_date'";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                int columnExists = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (columnExists == 0)
                {
                    string alterQuery = "ALTER TABLE todo_tasks ADD COLUMN created_date DATE NULL";
                    MySqlCommand alterCmd = new MySqlCommand(alterQuery, conn);
                    alterCmd.ExecuteNonQuery();
                }
            }
        }

        private void CleanupOldCompletedTasks()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                // Ensure completion_date column exists
                EnsureCompletionDateColumn();

                // Delete completed tasks that are older than 7 days
                string deleteQuery = @"DELETE FROM todo_tasks 
                                       WHERE student_id=@id 
                                       AND is_completed=1 
                                       AND completion_date IS NOT NULL 
                                       AND completion_date < DATE_SUB(CURDATE(), INTERVAL 7 DAY)";
                MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn);
                deleteCmd.Parameters.AddWithValue("@id", studentId);
                deleteCmd.ExecuteNonQuery();
            }
        }

        private void CleanupOldUncompletedTasks()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                EnsureCreatedDateColumn();

                DateTime weekStart = GetRollingStart(DateTime.Today);

                string deleteQuery = @"DELETE FROM todo_tasks
                                       WHERE student_id=@id
                                       AND is_completed=0
                                       AND ((created_date IS NOT NULL AND created_date < @start) OR created_date IS NULL)";
                MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn);
                deleteCmd.Parameters.AddWithValue("@id", studentId);
                deleteCmd.Parameters.AddWithValue("@start", weekStart);
                deleteCmd.ExecuteNonQuery();
            }
        }

        private void CleanupOldTodoProgress()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();
                string deleteQuery = @"DELETE FROM todo_progress
                                       WHERE student_id=@id
                                       AND date_recorded < DATE_SUB(CURDATE(), INTERVAL 7 DAY)";
                MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn);
                deleteCmd.Parameters.AddWithValue("@id", studentId);
                deleteCmd.ExecuteNonQuery();
            }
        }

        private void CleanupOldExpenses()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();
                string deleteQuery = @"DELETE FROM expenses
                                       WHERE student_id=@id
                                       AND date_added < DATE_SUB(CURDATE(), INTERVAL 7 DAY)";
                MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn);
                deleteCmd.Parameters.AddWithValue("@id", studentId);
                deleteCmd.ExecuteNonQuery();
            }
        }
        private void txtNotes_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtNotes.Text == "Add your notes here...")
            {
                txtNotes.Text = "";
                txtNotes.Foreground = Brushes.Black;
            }
        }

        private void txtNotes_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNotes.Text))
            {
                txtNotes.Text = "Add your notes here...";
                txtNotes.Foreground = Brushes.Gray;
            }
            SaveNotes();
        }

        private void EnsureNotesColumn()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();
                string q = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'students' AND COLUMN_NAME = 'notes'";
                MySqlCommand cmd = new MySqlCommand(q, conn);
                int exists = Convert.ToInt32(cmd.ExecuteScalar());
                if (exists == 0)
                {
                    string alter = "ALTER TABLE students ADD COLUMN notes TEXT NULL";
                    MySqlCommand alterCmd = new MySqlCommand(alter, conn);
                    alterCmd.ExecuteNonQuery();
                }
            }
        }

        private void LoadNotes()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();
                string q = "SELECT notes FROM students WHERE id=@id";
                MySqlCommand cmd = new MySqlCommand(q, conn);
                cmd.Parameters.AddWithValue("@id", studentId);
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value && !string.IsNullOrWhiteSpace(result.ToString()))
                {
                    txtNotes.Text = result.ToString();
                    txtNotes.Foreground = Brushes.Black;
                }
                else
                {
                    txtNotes.Text = "Add your notes here...";
                    txtNotes.Foreground = Brushes.Gray;
                }
            }
        }

        private void SaveNotes()
        {
            string content = txtNotes.Text;
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();
                string q = "UPDATE students SET notes=@notes WHERE id=@id";
                MySqlCommand cmd = new MySqlCommand(q, conn);
                object val = (string.IsNullOrWhiteSpace(content) || content == "Add your notes here...") ? DBNull.Value : content;
                cmd.Parameters.AddWithValue("@notes", val);
                cmd.Parameters.AddWithValue("@id", studentId);
                cmd.ExecuteNonQuery();
            }
        }

       

        #region Pie Chart

        private void UpdatePieChart()
        {
            int completedCount = 0;
            int uncompletedCount = 0;
            DateTime weekStart = GetRollingStart(DateTime.Today);

            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                // Get tasks from the last 7 days
                // Count completed tasks from last 7 days and all uncompleted tasks
                string query = @"SELECT 
                                    SUM(CASE WHEN is_completed = 1 AND completion_date >= @start THEN 1 ELSE 0 END) as completed,
                                    SUM(CASE WHEN is_completed = 0 AND (created_date IS NOT NULL AND created_date >= @start) THEN 1 ELSE 0 END) as uncompleted
                                FROM todo_tasks 
                                WHERE student_id = @id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", studentId);
                cmd.Parameters.AddWithValue("@start", weekStart);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        completedCount = reader.IsDBNull("completed") ? 0 : reader.GetInt32("completed");
                        uncompletedCount = reader.IsDBNull("uncompleted") ? 0 : reader.GetInt32("uncompleted");
                    }
                }

                // If the query doesn't work (column might not exist), fallback to simple count
                if (completedCount == 0 && uncompletedCount == 0)
                {
                    conn.Close();
                    conn.Open();

                    // Simple fallback query
                    string fallbackQuery = @"SELECT 
                                                SUM(CASE WHEN is_completed = 1 AND completion_date >= @start THEN 1 ELSE 0 END) as completed,
                                                SUM(CASE WHEN is_completed = 0 AND (created_date IS NOT NULL AND created_date >= @start) THEN 1 ELSE 0 END) as uncompleted
                                            FROM todo_tasks 
                                            WHERE student_id = @id";
                    
                    MySqlCommand fallbackCmd = new MySqlCommand(fallbackQuery, conn);
                    fallbackCmd.Parameters.AddWithValue("@id", studentId);
                    fallbackCmd.Parameters.AddWithValue("@start", weekStart);
                    
                    using (var reader = fallbackCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            completedCount = reader.IsDBNull("completed") ? 0 : reader.GetInt32("completed");
                            uncompletedCount = reader.IsDBNull("uncompleted") ? 0 : reader.GetInt32("uncompleted");
                        }
                    }
                }
            }

            int total = completedCount + uncompletedCount;

            if (total > 0)
            {
                double completedPercent = Math.Round((double)completedCount / total * 100, 1);
                double uncompletedPercent = Math.Round((double)uncompletedCount / total * 100, 1);

                // Update labels with count and percentage: "count | percentage%"
                lblCompletedInfo.Text = $"{completedCount} | {completedPercent}%";
                lblUncompletedInfo.Text = $"{uncompletedCount} | {uncompletedPercent}%";

                // Draw pie chart
                DrawPieChart(completedPercent, uncompletedPercent);
            }
            else
            {
                // No tasks - show empty state
                lblCompletedInfo.Text = "0 | 0%";
                lblUncompletedInfo.Text = "0 | 0%";
                DrawPieChart(0, 0);
            }
        }

        private void DrawPieChart(double completedPercent, double uncompletedPercent)
        {
            const double radius = 100;
            const double centerX = 100;
            const double centerY = 100;

            // Calculate angles
            double completedAngle = (completedPercent / 100.0) * 360.0;
            double uncompletedAngle = (uncompletedPercent / 100.0) * 360.0;

            if (completedPercent == 0 && uncompletedPercent == 0)
            {
                pathCompleted.Visibility = Visibility.Collapsed;
                pathUncompleted.Data = new EllipseGeometry(new Point(centerX, centerY), radius, radius);
                pathUncompleted.Fill = Brushes.LightGray;
                pathUncompleted.Visibility = Visibility.Visible;
                return;
            }

            if (completedPercent >= 100)
            {
                pathCompleted.Data = new EllipseGeometry(new Point(centerX, centerY), radius, radius);
                pathCompleted.Visibility = Visibility.Visible;
                pathUncompleted.Visibility = Visibility.Collapsed;
                return;
            }

            if (uncompletedPercent >= 100)
            {
                pathUncompleted.Data = new EllipseGeometry(new Point(centerX, centerY), radius, radius);
                pathUncompleted.Fill = new SolidColorBrush(Color.FromRgb(244, 67, 54));
                pathUncompleted.Visibility = Visibility.Visible;
                pathCompleted.Visibility = Visibility.Collapsed;
                return;
            }

            // Draw completed slice (Green) - starts from top
            if (completedPercent > 0)
            {
                double startAngle = -90; // Start from top
                double endAngle = startAngle + completedAngle;

                Point startPoint = new Point(centerX, centerY - radius);
                Point endPoint = CalculatePointOnCircle(centerX, centerY, radius, endAngle);

                PathFigure completedFigure = new PathFigure
                {
                    StartPoint = new Point(centerX, centerY),
                    IsClosed = true
                };

                completedFigure.Segments.Add(new LineSegment(startPoint, true));
                completedFigure.Segments.Add(new ArcSegment
                {
                    Point = endPoint,
                    Size = new Size(radius, radius),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc = completedAngle > 180
                });

                PathGeometry completedGeometry = new PathGeometry();
                completedGeometry.Figures.Add(completedFigure);
                pathCompleted.Data = completedGeometry;
                pathCompleted.Visibility = Visibility.Visible;
            }
            else
            {
                pathCompleted.Visibility = Visibility.Collapsed;
            }

            // Draw uncompleted slice (Red) - starts where completed ends
            if (uncompletedPercent > 0)
            {
                double startAngle = -90 + completedAngle;
                double endAngle = startAngle + uncompletedAngle;

                Point startPoint = CalculatePointOnCircle(centerX, centerY, radius, startAngle);
                Point endPoint = CalculatePointOnCircle(centerX, centerY, radius, endAngle);

                PathFigure uncompletedFigure = new PathFigure
                {
                    StartPoint = new Point(centerX, centerY),
                    IsClosed = true
                };

                uncompletedFigure.Segments.Add(new LineSegment(startPoint, true));
                uncompletedFigure.Segments.Add(new ArcSegment
                {
                    Point = endPoint,
                    Size = new Size(radius, radius),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc = uncompletedAngle > 180
                });

                PathGeometry uncompletedGeometry = new PathGeometry();
                uncompletedGeometry.Figures.Add(uncompletedFigure);
                pathUncompleted.Data = uncompletedGeometry;
                pathUncompleted.Fill = new SolidColorBrush(Color.FromRgb(244, 67, 54));
                pathUncompleted.Visibility = Visibility.Visible;
            }
            else
            {
                pathUncompleted.Visibility = Visibility.Collapsed;
            }
        }

        private Point CalculatePointOnCircle(double centerX, double centerY, double radius, double angleDegrees)
        {
            double angleRadians = angleDegrees * Math.PI / 180.0;
            double x = centerX + radius * Math.Cos(angleRadians);
            double y = centerY + radius * Math.Sin(angleRadians);
            return new Point(x, y);
        }

        #endregion

        #region Change Password

        private void ChangePassword_Click(object sender, MouseButtonEventArgs e)
        {
            ChangePasswordWindow changePasswordWindow = new ChangePasswordWindow(studentId);
            changePasswordWindow.Owner = this;
            changePasswordWindow.ShowDialog();
        }

        private void ChangePassword_MouseEnter(object sender, MouseEventArgs e)
        {
            txtChangePassword.Foreground = Brushes.Red;
        }

        private void ChangePassword_MouseLeave(object sender, MouseEventArgs e)
        {
            txtChangePassword.Foreground = Brushes.Black;
        }

        #endregion

        #region Financial Management

        private void EnsureFinancialTables()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                // Check if student_budget table exists
                string checkBudgetTable = @"SELECT COUNT(*) 
                                           FROM INFORMATION_SCHEMA.TABLES 
                                           WHERE TABLE_SCHEMA = DATABASE() 
                                           AND TABLE_NAME = 'student_budget'";
                MySqlCommand checkBudgetCmd = new MySqlCommand(checkBudgetTable, conn);
                int budgetTableExists = Convert.ToInt32(checkBudgetCmd.ExecuteScalar());

                if (budgetTableExists == 0)
                {
                    string createBudgetTable = @"CREATE TABLE student_budget (
                        budget_id INT AUTO_INCREMENT PRIMARY KEY,
                        student_id VARCHAR(20) NOT NULL,
                        budget_amount DECIMAL(10, 2) NOT NULL,
                        budget_remaining DECIMAL(10, 2) NOT NULL,
                        date_set DATE NOT NULL,
                        FOREIGN KEY (student_id) REFERENCES students(id),
                        UNIQUE KEY unique_student_date (student_id, date_set)
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4";
                    MySqlCommand createBudgetCmd = new MySqlCommand(createBudgetTable, conn);
                    createBudgetCmd.ExecuteNonQuery();
                }
                else
                {
                    // Check if budget_remaining column exists
                    string checkBudgetRemainingColumn = @"SELECT COUNT(*) 
                                                         FROM INFORMATION_SCHEMA.COLUMNS 
                                                         WHERE TABLE_SCHEMA = DATABASE() 
                                                         AND TABLE_NAME = 'student_budget' 
                                                         AND COLUMN_NAME = 'budget_remaining'";
                    MySqlCommand checkBudgetRemainingCmd = new MySqlCommand(checkBudgetRemainingColumn, conn);
                    int budgetRemainingColumnExists = Convert.ToInt32(checkBudgetRemainingCmd.ExecuteScalar());

                    if (budgetRemainingColumnExists == 0)
                    {
                        // Add budget_remaining column
                        string alterQuery = "ALTER TABLE student_budget ADD COLUMN budget_remaining DECIMAL(10, 2) NOT NULL DEFAULT 0";
                        MySqlCommand alterCmd = new MySqlCommand(alterQuery, conn);
                        alterCmd.ExecuteNonQuery();
                    }
                }

                // Check if expenses table exists
                string checkExpensesTable = @"SELECT COUNT(*) 
                                              FROM INFORMATION_SCHEMA.TABLES 
                                              WHERE TABLE_SCHEMA = DATABASE() 
                                              AND TABLE_NAME = 'expenses'";
                MySqlCommand checkExpensesCmd = new MySqlCommand(checkExpensesTable, conn);
                int expensesTableExists = Convert.ToInt32(checkExpensesCmd.ExecuteScalar());

                if (expensesTableExists == 0)
                {
                    string createExpensesTable = @"CREATE TABLE expenses (
                        expense_id INT AUTO_INCREMENT PRIMARY KEY,
                        student_id VARCHAR(20) NOT NULL,
                        category VARCHAR(100) NOT NULL,
                        cost DECIMAL(10, 2) NOT NULL,
                        date_added DATE NOT NULL,
                        FOREIGN KEY (student_id) REFERENCES students(id)
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4";
                    MySqlCommand createExpensesCmd = new MySqlCommand(createExpensesTable, conn);
                    createExpensesCmd.ExecuteNonQuery();
                }
            }
        }

        #region Watermark TextBox (Financial)

        private void TxtBudget_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBudget.Text == "Enter Budget")
            {
                txtBudget.Text = "";
                txtBudget.Foreground = Brushes.Black;
            }
        }

        private void TxtBudget_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBudget.Text))
            {
                txtBudget.Text = "Enter Budget";
                txtBudget.Foreground = Brushes.Gray;
            }
        }

        private void TxtExpenseCategory_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtExpenseCategory.Text == "Enter Expense Category")
            {
                txtExpenseCategory.Text = "";
                txtExpenseCategory.Foreground = Brushes.Black;
            }
        }

        private void TxtExpenseCategory_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtExpenseCategory.Text))
            {
                txtExpenseCategory.Text = "Enter Expense Category";
                txtExpenseCategory.Foreground = Brushes.Gray;
            }
        }

        private void TxtCost_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtCost.Text == "Enter Cost")
            {
                txtCost.Text = "";
                txtCost.Foreground = Brushes.Black;
            }
        }

        private void TxtCost_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCost.Text))
            {
                txtCost.Text = "Enter Cost";
                txtCost.Foreground = Brushes.Gray;
            }
        }

        #endregion

        private void LoadBudget()
        {
            // Budget is loaded when calculating remaining budget
            UpdateRemainingBudget();
        }

        private void LoadExpenses()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                // Only show today's expenses in the data grid
                string query = "SELECT expense_id, student_id, category, cost FROM expenses WHERE student_id=@id AND date_added=CURDATE() ORDER BY expense_id DESC";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", studentId);

                List<Expense> expenses = new List<Expense>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        expenses.Add(new Expense
                        {
                            expense_id = reader.GetInt32("expense_id"),
                            student_id = reader.GetString("student_id"),
                            category = reader.GetString("category"),
                            cost = reader.GetDecimal("cost")
                        });
                    }
                }

                dgExpenses.ItemsSource = expenses;
            }
        }

        private string CategorizeExpense(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return "Others";

            string input = userInput.ToLower().Trim();

            // Food keywords
            string[] foodKeywords = { "food", "meal", "eat", "restaurant", "cafe", "snack", "lunch", "dinner", "breakfast", "groceries", "grocery", "market", "store", "buy", "purchase", "junk", "drink", "beverage", "water", "juice", "coffee", "tea" };

            // Transportation keywords
            string[] transportKeywords = { "transport", "transportation", "fare", "taxi", "uber", "grab", "jeepney", "bus", "train", "lrt", "mrt", "gas", "gasoline", "fuel", "parking", "trike", "tricycle", "motor", "motorcycle" };

            // Check for food
            foreach (string keyword in foodKeywords)
            {
                if (input.Contains(keyword))
                    return "Food";
            }

            // Check for transportation
            foreach (string keyword in transportKeywords)
            {
                if (input.Contains(keyword))
                    return "Transportation";
            }

            // Default to Others
            return "Others";
        }

        private void AddBudget_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBudget.Text) || txtBudget.Text == "Enter Budget")
            {
                MessageBox.Show("Please enter a budget amount.");
                return;
            }

            if (!decimal.TryParse(txtBudget.Text, out decimal budgetAmount) || budgetAmount <= 0)
            {
                MessageBox.Show("Please enter a valid budget amount.");
                return;
            }

            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                // Check if budget exists for today
                string checkQuery = "SELECT COUNT(*) FROM student_budget WHERE student_id=@id AND date_set=CURDATE()";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@id", studentId);
                int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                // Calculate budget_remaining (budget_amount - today's expenses)
                decimal totalExpensesToday = 0;
                string expensesQuery = "SELECT SUM(cost) FROM expenses WHERE student_id=@id AND date_added=CURDATE()";
                MySqlCommand expensesCmd = new MySqlCommand(expensesQuery, conn);
                expensesCmd.Parameters.AddWithValue("@id", studentId);
                object expensesResult = expensesCmd.ExecuteScalar();
                if (expensesResult != null && expensesResult != DBNull.Value)
                {
                    totalExpensesToday = Convert.ToDecimal(expensesResult);
                }
                decimal budgetRemaining = budgetAmount - totalExpensesToday;

                if (exists > 0)
                {
                    // Update existing budget
                    string updateQuery = "UPDATE student_budget SET budget_amount=@amount, budget_remaining=@remaining WHERE student_id=@id AND date_set=CURDATE()";
                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@id", studentId);
                    updateCmd.Parameters.AddWithValue("@amount", budgetAmount);
                    updateCmd.Parameters.AddWithValue("@remaining", budgetRemaining);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    // Insert new budget
                    string insertQuery = "INSERT INTO student_budget(student_id, budget_amount, budget_remaining, date_set) VALUES(@id, @amount, @remaining, CURDATE())";
                    MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@id", studentId);
                    insertCmd.Parameters.AddWithValue("@amount", budgetAmount);
                    insertCmd.Parameters.AddWithValue("@remaining", budgetRemaining);
                    insertCmd.ExecuteNonQuery();
                }
            }

            txtBudget.Text = "Enter Budget";
            txtBudget.Foreground = Brushes.Gray;
            UpdateRemainingBudget();
            EnsureWeeklyRecordForDate(DateTime.Today);
            MessageBox.Show("Budget added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddExpense_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtExpenseCategory.Text) || txtExpenseCategory.Text == "Enter Expense Category")
            {
                MessageBox.Show("Please enter an expense category.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCost.Text) || txtCost.Text == "Enter Cost")
            {
                MessageBox.Show("Please enter a cost.");
                return;
            }

            if (!decimal.TryParse(txtCost.Text, out decimal cost) || cost <= 0)
            {
                MessageBox.Show("Please enter a valid cost amount.");
                return;
            }

            // Auto-categorize the expense
            string categorizedCategory = CategorizeExpense(txtExpenseCategory.Text);

            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                string query = "INSERT INTO expenses(student_id, category, cost, date_added) VALUES(@id, @category, @cost, CURDATE())";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", studentId);
                cmd.Parameters.AddWithValue("@category", categorizedCategory);
                cmd.Parameters.AddWithValue("@cost", cost);
                cmd.ExecuteNonQuery();
            }

            txtExpenseCategory.Text = "Enter Expense Category";
            txtExpenseCategory.Foreground = Brushes.Gray;
            txtCost.Text = "Enter Cost";
            txtCost.Foreground = Brushes.Gray;

            LoadExpenses();
            UpdateRemainingBudget();
            UpdateExpensePieChart(); // Update expense pie chart after adding expense
            UpdateRecordsTab();
            EnsureWeeklyRecordForDate(DateTime.Today);
        }

        private void RemoveExpense_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Expense expense)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to remove the expense '{expense.category}' - ₱{expense.cost:N2}?",
                    "Remove Expense",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using (var conn = DBHelper.GetConnection())
                    {
                        conn.Open();

                        string query = "DELETE FROM expenses WHERE expense_id=@id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", expense.expense_id);
                        cmd.ExecuteNonQuery();
                    }

                    LoadExpenses();
                    UpdateRemainingBudget();
                    UpdateExpensePieChart(); // Update expense pie chart after removing expense
                    UpdateRecordsTab();
                    MessageBox.Show("Expense removed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void UpdateRemainingBudget()
        {
            decimal budget = 0;
            decimal totalExpenses = 0;
            decimal budgetRemaining = 0;

            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                // Get today's budget and budget_remaining
                string budgetQuery = "SELECT budget_amount, budget_remaining FROM student_budget WHERE student_id=@id AND date_set=CURDATE()";
                MySqlCommand budgetCmd = new MySqlCommand(budgetQuery, conn);
                budgetCmd.Parameters.AddWithValue("@id", studentId);
                
                using (var reader = budgetCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        budget = reader.GetDecimal("budget_amount");
                        budgetRemaining = reader.GetDecimal("budget_remaining");
                    }
                }

                // If budget_remaining is not set, calculate it
                if (budgetRemaining == 0 && budget > 0)
                {
                    // Get total expenses for today
                    string expensesQuery = "SELECT SUM(cost) FROM expenses WHERE student_id=@id AND date_added=CURDATE()";
                    MySqlCommand expensesCmd = new MySqlCommand(expensesQuery, conn);
                    expensesCmd.Parameters.AddWithValue("@id", studentId);
                    object expensesResult = expensesCmd.ExecuteScalar();
                    if (expensesResult != null && expensesResult != DBNull.Value)
                    {
                        totalExpenses = Convert.ToDecimal(expensesResult);
                    }
                    budgetRemaining = budget - totalExpenses;

                    // Update budget_remaining in database
                    conn.Close();
                    conn.Open();
                    string updateQuery = "UPDATE student_budget SET budget_remaining=@remaining WHERE student_id=@id AND date_set=CURDATE()";
                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@id", studentId);
                    updateCmd.Parameters.AddWithValue("@remaining", budgetRemaining);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    // Get total expenses for today to recalculate
                    string expensesQuery = "SELECT SUM(cost) FROM expenses WHERE student_id=@id AND date_added=CURDATE()";
                    MySqlCommand expensesCmd = new MySqlCommand(expensesQuery, conn);
                    expensesCmd.Parameters.AddWithValue("@id", studentId);
                    object expensesResult = expensesCmd.ExecuteScalar();
                    if (expensesResult != null && expensesResult != DBNull.Value)
                    {
                        totalExpenses = Convert.ToDecimal(expensesResult);
                    }
                    budgetRemaining = budget - totalExpenses;

                    // Update budget_remaining in database
                    conn.Close();
                    conn.Open();
                    string updateQuery = "UPDATE student_budget SET budget_remaining=@remaining WHERE student_id=@id AND date_set=CURDATE()";
                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@id", studentId);
                    updateCmd.Parameters.AddWithValue("@remaining", budgetRemaining);
                    updateCmd.ExecuteNonQuery();
                }
            }

            lblRemainingBudget.Text = $"₱{budgetRemaining:N2}";
            
            // Change color based on remaining budget
            if (budgetRemaining < 0)
            {
                lblRemainingBudget.Foreground = Brushes.Red;
            }
            else if (budgetRemaining < budget * 0.2m && budget > 0)
            {
                lblRemainingBudget.Foreground = Brushes.Orange;
            }
            else
            {
                lblRemainingBudget.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80)); // #4CAF50
            }
        }

        #endregion

        #region Expense Pie Chart

        private void UpdateExpensePieChart()
        {
            decimal foodTotal = 0;
            decimal transportationTotal = 0;
            decimal othersTotal = 0;
            DateTime weekStart = GetRollingStart(DateTime.Today);

            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                // Get expenses from the last 7 days, grouped by category
                string query = @"SELECT 
                                    category,
                                    SUM(cost) as total_cost
                                FROM expenses 
                                WHERE student_id = @id 
                                AND date_added >= @start
                                GROUP BY category";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", studentId);
                cmd.Parameters.AddWithValue("@start", weekStart);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string category = reader.GetString("category");
                        decimal categoryTotal = reader.GetDecimal("total_cost");

                        switch (category.ToLower())
                        {
                            case "food":
                                foodTotal = categoryTotal;
                                break;
                            case "transportation":
                                transportationTotal = categoryTotal;
                                break;
                            case "others":
                                othersTotal = categoryTotal;
                                break;
                        }
                    }
                }
            }

            decimal total = foodTotal + transportationTotal + othersTotal;

            if (total > 0)
            {
                double foodPercent = Math.Round((double)(foodTotal / total * 100), 1);
                double transportationPercent = Math.Round((double)(transportationTotal / total * 100), 1);
                double othersPercent = Math.Round((double)(othersTotal / total * 100), 1);

                // Update labels with amount and percentage: "₱amount | percentage%"
                lblFoodInfo.Text = $"₱{foodTotal:N2} | {foodPercent}%";
                lblTransportationInfo.Text = $"₱{transportationTotal:N2} | {transportationPercent}%";
                lblOthersInfo.Text = $"₱{othersTotal:N2} | {othersPercent}%";

                // Draw pie chart
                DrawExpensePieChart(foodPercent, transportationPercent, othersPercent);
            }
            else
            {
                // No expenses - show empty state
                lblFoodInfo.Text = "₱0.00 | 0%";
                lblTransportationInfo.Text = "₱0.00 | 0%";
                lblOthersInfo.Text = "₱0.00 | 0%";
                DrawExpensePieChart(0, 0, 0);
            }
            UpdateRecordsTab();
            LoadWeeklyRecords();
        }

        private void DrawExpensePieChart(double foodPercent, double transportationPercent, double othersPercent)
        {
            const double radius = 100;
            const double centerX = 100;
            const double centerY = 100;

            // Calculate angles
            double foodAngle = (foodPercent / 100.0) * 360.0;
            double transportationAngle = (transportationPercent / 100.0) * 360.0;
            double othersAngle = (othersPercent / 100.0) * 360.0;

            if (foodPercent == 0 && transportationPercent == 0 && othersPercent == 0)
            {
                pathFood.Visibility = Visibility.Collapsed;
                pathTransportation.Visibility = Visibility.Collapsed;
                pathOthers.Data = new EllipseGeometry(new Point(centerX, centerY), radius, radius);
                pathOthers.Fill = Brushes.LightGray;
                pathOthers.Visibility = Visibility.Visible;
                return;
            }

            if (foodPercent >= 100)
            {
                pathFood.Data = new EllipseGeometry(new Point(centerX, centerY), radius, radius);
                pathFood.Fill = new SolidColorBrush(Color.FromRgb(33, 150, 243));
                pathFood.Visibility = Visibility.Visible;
                pathTransportation.Visibility = Visibility.Collapsed;
                pathOthers.Visibility = Visibility.Collapsed;
                return;
            }

            if (transportationPercent >= 100)
            {
                pathTransportation.Data = new EllipseGeometry(new Point(centerX, centerY), radius, radius);
                pathTransportation.Fill = new SolidColorBrush(Color.FromRgb(255, 152, 0));
                pathTransportation.Visibility = Visibility.Visible;
                pathFood.Visibility = Visibility.Collapsed;
                pathOthers.Visibility = Visibility.Collapsed;
                return;
            }

            if (othersPercent >= 100)
            {
                pathOthers.Data = new EllipseGeometry(new Point(centerX, centerY), radius, radius);
                pathOthers.Fill = new SolidColorBrush(Color.FromRgb(156, 39, 176));
                pathOthers.Visibility = Visibility.Visible;
                pathFood.Visibility = Visibility.Collapsed;
                pathTransportation.Visibility = Visibility.Collapsed;
                return;
            }

            // Draw Food slice (Blue) - starts from top
            if (foodPercent > 0)
            {
                double startAngle = -90; // Start from top
                double endAngle = startAngle + foodAngle;

                Point startPoint = new Point(centerX, centerY - radius);
                Point endPoint = CalculatePointOnCircle(centerX, centerY, radius, endAngle);

                PathFigure foodFigure = new PathFigure
                {
                    StartPoint = new Point(centerX, centerY),
                    IsClosed = true
                };

                foodFigure.Segments.Add(new LineSegment(startPoint, true));
                foodFigure.Segments.Add(new ArcSegment
                {
                    Point = endPoint,
                    Size = new Size(radius, radius),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc = foodAngle > 180
                });

                PathGeometry foodGeometry = new PathGeometry();
                foodGeometry.Figures.Add(foodFigure);
                pathFood.Data = foodGeometry;
                pathFood.Visibility = Visibility.Visible;
            }
            else
            {
                pathFood.Visibility = Visibility.Collapsed;
            }

            // Draw Transportation slice (Orange) - starts where Food ends
            if (transportationPercent > 0)
            {
                double startAngle = -90 + foodAngle;
                double endAngle = startAngle + transportationAngle;

                Point startPoint = CalculatePointOnCircle(centerX, centerY, radius, startAngle);
                Point endPoint = CalculatePointOnCircle(centerX, centerY, radius, endAngle);

                PathFigure transportationFigure = new PathFigure
                {
                    StartPoint = new Point(centerX, centerY),
                    IsClosed = true
                };

                transportationFigure.Segments.Add(new LineSegment(startPoint, true));
                transportationFigure.Segments.Add(new ArcSegment
                {
                    Point = endPoint,
                    Size = new Size(radius, radius),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc = transportationAngle > 180
                });

                PathGeometry transportationGeometry = new PathGeometry();
                transportationGeometry.Figures.Add(transportationFigure);
                pathTransportation.Data = transportationGeometry;
                pathTransportation.Visibility = Visibility.Visible;
            }
            else
            {
                pathTransportation.Visibility = Visibility.Collapsed;
            }

            // Draw Others slice (Purple) - starts where Transportation ends
            if (othersPercent > 0)
            {
                double startAngle = -90 + foodAngle + transportationAngle;
                double endAngle = startAngle + othersAngle;

                Point startPoint = CalculatePointOnCircle(centerX, centerY, radius, startAngle);
                Point endPoint = CalculatePointOnCircle(centerX, centerY, radius, endAngle);

                PathFigure othersFigure = new PathFigure
                {
                    StartPoint = new Point(centerX, centerY),
                    IsClosed = true
                };

                othersFigure.Segments.Add(new LineSegment(startPoint, true));
                othersFigure.Segments.Add(new ArcSegment
                {
                    Point = endPoint,
                    Size = new Size(radius, radius),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc = othersAngle > 180
                });

                PathGeometry othersGeometry = new PathGeometry();
                othersGeometry.Figures.Add(othersFigure);
                pathOthers.Data = othersGeometry;
                pathOthers.Fill = new SolidColorBrush(Color.FromRgb(156, 39, 176));
                pathOthers.Visibility = Visibility.Visible;
            }
            else
            {
                pathOthers.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateRecordsTab()
        {
            double todoCompletedPercent = 0;
            double todoUncompletedPercent = 0;

            int completedCount = 0;
            int uncompletedCount = 0;
            DateTime weekStart = GetRollingStart(DateTime.Today);
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT 
                                    SUM(CASE WHEN is_completed = 1 AND completion_date >= @start THEN 1 ELSE 0 END) as completed,
                                    SUM(CASE WHEN is_completed = 0 AND (created_date IS NOT NULL AND created_date >= @start) THEN 1 ELSE 0 END) as uncompleted
                                  FROM todo_tasks 
                                  WHERE student_id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", studentId);
                cmd.Parameters.AddWithValue("@start", weekStart);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        completedCount = reader.IsDBNull("completed") ? 0 : reader.GetInt32("completed");
                        uncompletedCount = reader.IsDBNull("uncompleted") ? 0 : reader.GetInt32("uncompleted");
                    }
                }
            }
            int todoTotal = completedCount + uncompletedCount;
            if (todoTotal > 0)
            {
                todoCompletedPercent = Math.Round((double)completedCount / todoTotal * 100, 1);
                todoUncompletedPercent = Math.Round((double)uncompletedCount / todoTotal * 100, 1);
            }
            txtRecordTodoCompletedPercent.Text = todoCompletedPercent.ToString("F1") + "%";
            txtRecordTodoUncompletedPercent.Text = todoUncompletedPercent.ToString("F1") + "%";

            decimal foodTotal = 0;
            decimal transportationTotal = 0;
            decimal othersTotal = 0;
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT category, SUM(cost) as total_cost FROM expenses 
                                  WHERE student_id = @id AND date_added >= @start
                                  GROUP BY category";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", studentId);
                cmd.Parameters.AddWithValue("@start", weekStart);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string category = reader.GetString("category");
                        decimal total = reader.GetDecimal("total_cost");
                        switch (category.ToLower())
                        {
                            case "food":
                                foodTotal = total;
                                break;
                            case "transportation":
                                transportationTotal = total;
                                break;
                            case "others":
                                othersTotal = total;
                                break;
                        }
                    }
                }
            }
            decimal expTotal = foodTotal + transportationTotal + othersTotal;
            double foodPercent = 0, transportationPercent = 0, othersPercent = 0;
            if (expTotal > 0)
            {
                foodPercent = Math.Round((double)(foodTotal / expTotal * 100), 1);
                transportationPercent = Math.Round((double)(transportationTotal / expTotal * 100), 1);
                othersPercent = Math.Round((double)(othersTotal / expTotal * 100), 1);
            }
            txtRecordFoodPercent.Text = foodPercent.ToString("F1") + "%";
            txtRecordTransportationPercent.Text = transportationPercent.ToString("F1") + "%";
            txtRecordOthersPercent.Text = othersPercent.ToString("F1") + "%";
        }

        #endregion

        private void dgTodo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void EnsureRecordsTable()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();
                string createTable = @"CREATE TABLE IF NOT EXISTS weekly_records (
                    record_id INT AUTO_INCREMENT PRIMARY KEY,
                    student_id VARCHAR(20) NOT NULL,
                    week_start_date DATE NOT NULL,
                    week_end_date DATE NOT NULL,
                    completed_tasks INT NOT NULL,
                    total_tasks INT NOT NULL,
                    food_total DECIMAL(10,2) NOT NULL DEFAULT 0,
                    food_last_date DATE NULL,
                    transportation_total DECIMAL(10,2) NOT NULL DEFAULT 0,
                    transportation_last_date DATE NULL,
                    others_total DECIMAL(10,2) NOT NULL DEFAULT 0,
                    others_last_date DATE NULL,
                    total_budget DECIMAL(10,2) NOT NULL DEFAULT 0,
                    total_balance DECIMAL(10,2) NOT NULL DEFAULT 0,
                    budget_last_date DATE NULL,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    UNIQUE KEY unique_student_week (student_id, week_end_date),
                    FOREIGN KEY (student_id) REFERENCES students(id)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4";
                MySqlCommand cmd = new MySqlCommand(createTable, conn);
                cmd.ExecuteNonQuery();
            }
        }

        private void EnsureWeeklyRecordBalanceColumn()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();
                string checkQuery = @"SELECT COUNT(*) 
                                      FROM INFORMATION_SCHEMA.COLUMNS 
                                      WHERE TABLE_SCHEMA = DATABASE() 
                                      AND TABLE_NAME = 'weekly_records' 
                                      AND COLUMN_NAME = 'total_balance'";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (exists == 0)
                {
                    string alterQuery = "ALTER TABLE weekly_records ADD COLUMN total_balance DECIMAL(10,2) NOT NULL DEFAULT 0 AFTER total_budget";
                    MySqlCommand alterCmd = new MySqlCommand(alterQuery, conn);
                    try { alterCmd.ExecuteNonQuery(); } catch { }
                }
            }
        }

        private void CreateWeeklyRecordIfNeeded()
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();
                // Initialize first weekly record only when there is real activity
                string firstActivityQuery = @"
                    SELECT MIN(activity_date) AS first_date FROM (
                        SELECT MIN(created_date) AS activity_date FROM todo_tasks WHERE student_id=@id AND created_date IS NOT NULL
                        UNION ALL
                        SELECT MIN(date_added) FROM expenses WHERE student_id=@id
                        UNION ALL
                        SELECT MIN(date_set) FROM student_budget WHERE student_id=@id
                    ) t";
                MySqlCommand firstCmd = new MySqlCommand(firstActivityQuery, conn);
                firstCmd.Parameters.AddWithValue("@id", studentId);
                object firstDateObj = firstCmd.ExecuteScalar();
                if (firstDateObj == null || firstDateObj == DBNull.Value) return; // no activity yet

                DateTime weekStart = Convert.ToDateTime(firstDateObj);
                DateTime weekEnd = weekStart.AddDays(6);

                string existsQuery = @"SELECT COUNT(*) FROM weekly_records WHERE student_id=@id AND week_start_date=@ws AND week_end_date=@we";
                MySqlCommand existsCmd = new MySqlCommand(existsQuery, conn);
                existsCmd.Parameters.AddWithValue("@id", studentId);
                existsCmd.Parameters.AddWithValue("@ws", weekStart);
                existsCmd.Parameters.AddWithValue("@we", weekEnd);
                int exists = Convert.ToInt32(existsCmd.ExecuteScalar());
                if (exists > 0) return;

                // Latest todo_progress within week
                int completed = 0, total = 0; DateTime? todoDate = null;
                string todoQuery = @"SELECT completed_tasks, total_tasks, date_recorded
                                      FROM todo_progress
                                      WHERE student_id=@id AND date_recorded BETWEEN @start AND @end
                                      ORDER BY date_recorded DESC LIMIT 1";
                MySqlCommand todoCmd = new MySqlCommand(todoQuery, conn);
                todoCmd.Parameters.AddWithValue("@id", studentId);
                todoCmd.Parameters.AddWithValue("@start", weekStart);
                todoCmd.Parameters.AddWithValue("@end", weekEnd);
                using (var reader = todoCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        completed = reader.GetInt32("completed_tasks");
                        total = reader.GetInt32("total_tasks");
                        todoDate = reader.GetDateTime("date_recorded");
                    }
                }

                // Expenses totals and last dates
                decimal foodTotal = 0, transTotal = 0, othersTotal = 0;
                DateTime? foodDate = null, transDate = null, othersDate = null;

                string expFood = @"SELECT COALESCE(SUM(cost),0) total, MAX(date_added) last_date FROM expenses 
                                   WHERE student_id=@id AND category='Food' AND date_added BETWEEN @start AND @end";
                using (var cmdFood = new MySqlCommand(expFood, conn))
                {
                    cmdFood.Parameters.AddWithValue("@id", studentId);
                    cmdFood.Parameters.AddWithValue("@start", weekStart);
                    cmdFood.Parameters.AddWithValue("@end", weekEnd);
                    using (var r = cmdFood.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            foodTotal = r.IsDBNull("total") ? 0 : r.GetDecimal("total");
                            if (!r.IsDBNull("last_date")) foodDate = r.GetDateTime("last_date");
                        }
                    }
                }

                string expTrans = @"SELECT COALESCE(SUM(cost),0) total, MAX(date_added) last_date FROM expenses 
                                    WHERE student_id=@id AND category='Transportation' AND date_added BETWEEN @start AND @end";
                using (var cmdTrans = new MySqlCommand(expTrans, conn))
                {
                    cmdTrans.Parameters.AddWithValue("@id", studentId);
                    cmdTrans.Parameters.AddWithValue("@start", weekStart);
                    cmdTrans.Parameters.AddWithValue("@end", weekEnd);
                    using (var r = cmdTrans.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            transTotal = r.IsDBNull("total") ? 0 : r.GetDecimal("total");
                            if (!r.IsDBNull("last_date")) transDate = r.GetDateTime("last_date");
                        }
                    }
                }

                string expOthers = @"SELECT COALESCE(SUM(cost),0) total, MAX(date_added) last_date FROM expenses 
                                     WHERE student_id=@id AND category='Others' AND date_added BETWEEN @start AND @end";
                using (var cmdOthers = new MySqlCommand(expOthers, conn))
                {
                    cmdOthers.Parameters.AddWithValue("@id", studentId);
                    cmdOthers.Parameters.AddWithValue("@start", weekStart);
                    cmdOthers.Parameters.AddWithValue("@end", weekEnd);
                    using (var r = cmdOthers.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            othersTotal = r.IsDBNull("total") ? 0 : r.GetDecimal("total");
                            if (!r.IsDBNull("last_date")) othersDate = r.GetDateTime("last_date");
                        }
                    }
                }

                // Sum budget and balance within week
                decimal totalBudget = 0; decimal totalBalance = 0; DateTime? budgetDate = null;
                string sumBudgetQuery = @"SELECT COALESCE(SUM(budget_amount),0) as sum_budget, MAX(date_set) as last_date
                                           FROM student_budget 
                                           WHERE student_id=@id AND date_set BETWEEN @start AND @end";
                using (var sumBudgetCmd = new MySqlCommand(sumBudgetQuery, conn))
                {
                    sumBudgetCmd.Parameters.AddWithValue("@id", studentId);
                    sumBudgetCmd.Parameters.AddWithValue("@start", weekStart);
                    sumBudgetCmd.Parameters.AddWithValue("@end", weekEnd);
                    using (var r = sumBudgetCmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            totalBudget = r.IsDBNull(r.GetOrdinal("sum_budget")) ? 0 : r.GetDecimal("sum_budget");
                            if (!r.IsDBNull(r.GetOrdinal("last_date"))) budgetDate = r.GetDateTime("last_date");
                        }
                    }
                }

                string sumBalanceQuery = @"SELECT COALESCE(SUM(budget_remaining),0) as sum_balance
                                           FROM student_budget 
                                           WHERE student_id=@id AND date_set BETWEEN @start AND @end";
                using (var sumBalanceCmd = new MySqlCommand(sumBalanceQuery, conn))
                {
                    sumBalanceCmd.Parameters.AddWithValue("@id", studentId);
                    sumBalanceCmd.Parameters.AddWithValue("@start", weekStart);
                    sumBalanceCmd.Parameters.AddWithValue("@end", weekEnd);
                    using (var r = sumBalanceCmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            totalBalance = r.IsDBNull(r.GetOrdinal("sum_balance")) ? 0 : r.GetDecimal("sum_balance");
                        }
                    }
                }

                // Insert weekly record
                string insert = @"INSERT INTO weekly_records (
                                    student_id, week_start_date, week_end_date, completed_tasks, total_tasks,
                                    food_total, food_last_date, transportation_total, transportation_last_date,
                                    others_total, others_last_date, total_budget, total_balance, budget_last_date)
                                  VALUES (
                                    @id, @ws, @we, @completed, @total,
                                    @food, @foodDate, @trans, @transDate,
                                    @others, @othersDate, @budget, @balance, @budgetDate)";
                MySqlCommand insertCmd = new MySqlCommand(insert, conn);
                insertCmd.Parameters.AddWithValue("@id", studentId);
                insertCmd.Parameters.AddWithValue("@ws", weekStart);
                insertCmd.Parameters.AddWithValue("@we", weekEnd);
                insertCmd.Parameters.AddWithValue("@completed", completed);
                insertCmd.Parameters.AddWithValue("@total", total);
                insertCmd.Parameters.AddWithValue("@food", foodTotal);
                insertCmd.Parameters.AddWithValue("@foodDate", (object?)foodDate ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@trans", transTotal);
                insertCmd.Parameters.AddWithValue("@transDate", (object?)transDate ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@others", othersTotal);
                insertCmd.Parameters.AddWithValue("@othersDate", (object?)othersDate ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@budget", totalBudget);
                insertCmd.Parameters.AddWithValue("@balance", totalBalance);
                insertCmd.Parameters.AddWithValue("@budgetDate", (object?)budgetDate ?? DBNull.Value);

                try { insertCmd.ExecuteNonQuery(); } catch { /* ignore if unique already exists */ }
            }
        }

        private void EnsureWeeklyRecordForDate(DateTime activityDate)
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();
                DateTime ws = activityDate.Date;
                DateTime we = ws.AddDays(6);
                string existsQuery = @"SELECT COUNT(*) FROM weekly_records WHERE student_id=@id AND @today BETWEEN week_start_date AND week_end_date";
                MySqlCommand existsCmd = new MySqlCommand(existsQuery, conn);
                existsCmd.Parameters.AddWithValue("@id", studentId);
                existsCmd.Parameters.AddWithValue("@today", activityDate.Date);
                int exists = Convert.ToInt32(existsCmd.ExecuteScalar());
                if (exists > 0) return;

                // compute summaries within ws..we
                int completed = 0, total = 0;
                string todoQuery = @"SELECT completed_tasks, total_tasks, date_recorded
                                      FROM todo_progress
                                      WHERE student_id=@id AND date_recorded BETWEEN @start AND @end
                                      ORDER BY date_recorded DESC LIMIT 1";
                using (var todoCmd = new MySqlCommand(todoQuery, conn))
                {
                    todoCmd.Parameters.AddWithValue("@id", studentId);
                    todoCmd.Parameters.AddWithValue("@start", ws);
                    todoCmd.Parameters.AddWithValue("@end", we);
                    using (var r = todoCmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            completed = r.GetInt32("completed_tasks");
                            total = r.GetInt32("total_tasks");
                        }
                    }
                }

                decimal foodTotal = 0, transTotal = 0, othersTotal = 0;
                DateTime? foodDate = null, transDate = null, othersDate = null;
                string expFood = @"SELECT COALESCE(SUM(cost),0) total, MAX(date_added) last_date FROM expenses 
                                   WHERE student_id=@id AND category='Food' AND date_added BETWEEN @start AND @end";
                using (var cmdFood = new MySqlCommand(expFood, conn))
                {
                    cmdFood.Parameters.AddWithValue("@id", studentId);
                    cmdFood.Parameters.AddWithValue("@start", ws);
                    cmdFood.Parameters.AddWithValue("@end", we);
                    using (var r = cmdFood.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            foodTotal = r.IsDBNull("total") ? 0 : r.GetDecimal("total");
                            if (!r.IsDBNull("last_date")) foodDate = r.GetDateTime("last_date");
                        }
                    }
                }

                string expTrans = @"SELECT COALESCE(SUM(cost),0) total, MAX(date_added) last_date FROM expenses 
                                    WHERE student_id=@id AND category='Transportation' AND date_added BETWEEN @start AND @end";
                using (var cmdTrans = new MySqlCommand(expTrans, conn))
                {
                    cmdTrans.Parameters.AddWithValue("@id", studentId);
                    cmdTrans.Parameters.AddWithValue("@start", ws);
                    cmdTrans.Parameters.AddWithValue("@end", we);
                    using (var r = cmdTrans.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            transTotal = r.IsDBNull("total") ? 0 : r.GetDecimal("total");
                            if (!r.IsDBNull("last_date")) transDate = r.GetDateTime("last_date");
                        }
                    }
                }

                string expOthers = @"SELECT COALESCE(SUM(cost),0) total, MAX(date_added) last_date FROM expenses 
                                     WHERE student_id=@id AND category='Others' AND date_added BETWEEN @start AND @end";
                using (var cmdOthers = new MySqlCommand(expOthers, conn))
                {
                    cmdOthers.Parameters.AddWithValue("@id", studentId);
                    cmdOthers.Parameters.AddWithValue("@start", ws);
                    cmdOthers.Parameters.AddWithValue("@end", we);
                    using (var r = cmdOthers.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            othersTotal = r.IsDBNull("total") ? 0 : r.GetDecimal("total");
                            if (!r.IsDBNull("last_date")) othersDate = r.GetDateTime("last_date");
                        }
                    }
                }

                decimal totalBudget = SumBudget(ws, we);
                decimal totalBalance = SumBalance(ws, we);

                string insert = @"INSERT INTO weekly_records (
                                    student_id, week_start_date, week_end_date, completed_tasks, total_tasks,
                                    food_total, food_last_date, transportation_total, transportation_last_date,
                                    others_total, others_last_date, total_budget, total_balance, budget_last_date)
                                  VALUES (
                                    @id, @ws, @we, @completed, @total,
                                    @food, @foodDate, @trans, @transDate,
                                    @others, @othersDate, @budget, @balance, NULL)";
                using (var insertCmd = new MySqlCommand(insert, conn))
                {
                    insertCmd.Parameters.AddWithValue("@id", studentId);
                    insertCmd.Parameters.AddWithValue("@ws", ws);
                    insertCmd.Parameters.AddWithValue("@we", we);
                    insertCmd.Parameters.AddWithValue("@completed", completed);
                    insertCmd.Parameters.AddWithValue("@total", total);
                    insertCmd.Parameters.AddWithValue("@food", foodTotal);
                    insertCmd.Parameters.AddWithValue("@foodDate", (object?)foodDate ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@trans", transTotal);
                    insertCmd.Parameters.AddWithValue("@transDate", (object?)transDate ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@others", othersTotal);
                    insertCmd.Parameters.AddWithValue("@othersDate", (object?)othersDate ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@budget", totalBudget);
                    insertCmd.Parameters.AddWithValue("@balance", totalBalance);
                    try { insertCmd.ExecuteNonQuery(); } catch { }
                }
            }
        }

        private class WeeklyRecordRow
        {
            public string Week { get; set; } = string.Empty;
            public string CompletedTaskDisplay { get; set; } = string.Empty;
            public string TotalTaskDisplay { get; set; } = string.Empty;
            public string FoodExpenseDisplay { get; set; } = string.Empty;
            public string TransportationExpenseDisplay { get; set; } = string.Empty;
            public string OthersExpenseDisplay { get; set; } = string.Empty;
            public string TotalBudgetDisplay { get; set; } = string.Empty;
            public string TotalBalanceDisplay { get; set; } = string.Empty;
        }

        private void LoadWeeklyRecords()
        {
            List<WeeklyRecordRow> rows = new List<WeeklyRecordRow>();
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();
                string q = @"SELECT week_start_date, week_end_date
                              FROM weekly_records WHERE student_id=@id ORDER BY week_end_date DESC";
                MySqlCommand cmd = new MySqlCommand(q, conn);
                cmd.Parameters.AddWithValue("@id", studentId);
                using (var r = cmd.ExecuteReader())
                {
                    List<(DateTime ws, DateTime we)> ranges = new List<(DateTime, DateTime)>();
                    while (r.Read())
                    {
                        ranges.Add((r.GetDateTime("week_start_date"), r.GetDateTime("week_end_date")));
                    }
                    r.Close();

                    foreach (var (ws, we) in ranges)
                    {
                        // Live recompute totals for this week range
                        int completed = 0;
                        int total = 0;
                        decimal food = 0, trans = 0, others = 0;

                        // Completed tasks
                        using (var c1 = new MySqlCommand("SELECT COUNT(*) FROM todo_tasks WHERE student_id=@id AND is_completed=1 AND completion_date BETWEEN @ws AND @we", conn))
                        {
                            c1.Parameters.AddWithValue("@id", studentId);
                            c1.Parameters.AddWithValue("@ws", ws);
                            c1.Parameters.AddWithValue("@we", we);
                            completed = Convert.ToInt32(c1.ExecuteScalar());
                        }

                        // Total tasks created in range
                        using (var c2 = new MySqlCommand("SELECT COUNT(*) FROM todo_tasks WHERE student_id=@id AND created_date BETWEEN @ws AND @we", conn))
                        {
                            c2.Parameters.AddWithValue("@id", studentId);
                            c2.Parameters.AddWithValue("@ws", ws);
                            c2.Parameters.AddWithValue("@we", we);
                            total = Convert.ToInt32(c2.ExecuteScalar());
                        }

                        // Fallback to todo_progress snapshot if task dates are missing
                        if (completed == 0 && total == 0)
                        {
                            using (var cProg = new MySqlCommand("SELECT completed_tasks, total_tasks FROM todo_progress WHERE student_id=@id AND date_recorded BETWEEN @ws AND @we ORDER BY date_recorded DESC LIMIT 1", conn))
                            {
                                cProg.Parameters.AddWithValue("@id", studentId);
                                cProg.Parameters.AddWithValue("@ws", ws);
                                cProg.Parameters.AddWithValue("@we", we);
                                using (var rp = cProg.ExecuteReader())
                                {
                                    if (rp.Read())
                                    {
                                        completed = rp.GetInt32("completed_tasks");
                                        total = rp.GetInt32("total_tasks");
                                    }
                                }
                            }
                        }

                        // Expenses per category
                        using (var cFood = new MySqlCommand("SELECT COALESCE(SUM(cost),0) FROM expenses WHERE student_id=@id AND category='Food' AND date_added BETWEEN @ws AND @we", conn))
                        {
                            cFood.Parameters.AddWithValue("@id", studentId);
                            cFood.Parameters.AddWithValue("@ws", ws);
                            cFood.Parameters.AddWithValue("@we", we);
                            object res = cFood.ExecuteScalar();
                            food = res == null || res == DBNull.Value ? 0 : Convert.ToDecimal(res);
                        }

                        using (var cTrans = new MySqlCommand("SELECT COALESCE(SUM(cost),0) FROM expenses WHERE student_id=@id AND category='Transportation' AND date_added BETWEEN @ws AND @we", conn))
                        {
                            cTrans.Parameters.AddWithValue("@id", studentId);
                            cTrans.Parameters.AddWithValue("@ws", ws);
                            cTrans.Parameters.AddWithValue("@we", we);
                            object res = cTrans.ExecuteScalar();
                            trans = res == null || res == DBNull.Value ? 0 : Convert.ToDecimal(res);
                        }

                        using (var cOthers = new MySqlCommand("SELECT COALESCE(SUM(cost),0) FROM expenses WHERE student_id=@id AND category='Others' AND date_added BETWEEN @ws AND @we", conn))
                        {
                            cOthers.Parameters.AddWithValue("@id", studentId);
                            cOthers.Parameters.AddWithValue("@ws", ws);
                            cOthers.Parameters.AddWithValue("@we", we);
                            object res = cOthers.ExecuteScalar();
                            others = res == null || res == DBNull.Value ? 0 : Convert.ToDecimal(res);
                        }

                        decimal budget = SumBudget(ws, we);
                        decimal balance = SumBalance(ws, we);

                        rows.Add(new WeeklyRecordRow
                        {
                            Week = ws.ToString("MM-dd") + " → " + we.ToString("MM-dd"),
                            CompletedTaskDisplay = completed.ToString(),
                            TotalTaskDisplay = total.ToString(),
                            FoodExpenseDisplay = "₱" + food.ToString("N0"),
                            TransportationExpenseDisplay = "₱" + trans.ToString("N0"),
                            OthersExpenseDisplay = "₱" + others.ToString("N0"),
                            TotalBudgetDisplay = "₱" + budget.ToString("N0"),
                            TotalBalanceDisplay = "₱" + balance.ToString("N0")
                        });
                    }
                }
            }
            dgWeeklyRecords.ItemsSource = rows;
        }

        private decimal SumBudget(DateTime start, DateTime end)
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();
                string q = "SELECT COALESCE(SUM(budget_amount),0) FROM student_budget WHERE student_id=@id AND date_set BETWEEN @start AND @end";
                MySqlCommand cmd = new MySqlCommand(q, conn);
                cmd.Parameters.AddWithValue("@id", studentId);
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", end);
                object result = cmd.ExecuteScalar();
                return result == null || result == DBNull.Value ? 0 : Convert.ToDecimal(result);
            }
        }

        private decimal SumBalance(DateTime start, DateTime end)
        {
            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();
                string q = "SELECT COALESCE(SUM(budget_remaining),0) FROM student_budget WHERE student_id=@id AND date_set BETWEEN @start AND @end";
                MySqlCommand cmd = new MySqlCommand(q, conn);
                cmd.Parameters.AddWithValue("@id", studentId);
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", end);
                object result = cmd.ExecuteScalar();
                return result == null || result == DBNull.Value ? 0 : Convert.ToDecimal(result);
            }
        }

        private DateTime GetWeekStartTuesday(DateTime date)
        {
            int daysSinceTuesday = ((int)date.DayOfWeek - (int)DayOfWeek.Tuesday + 7) % 7;
            return date.AddDays(-daysSinceTuesday);
        }

        private DateTime GetRollingStart(DateTime date)
        {
            return date.Date.AddDays(-6);
        }
    }
}
