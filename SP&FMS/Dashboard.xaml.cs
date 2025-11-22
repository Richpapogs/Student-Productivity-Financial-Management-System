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
            lblDetails.Text = $"Course: {currentStudent.Course}\n\nEmail: {currentStudent.Email}\n\nContact: {currentStudent.Contact}";

            // Watermark behavior
            txtNewTask.GotFocus += TxtNewTask_GotFocus;
            txtNewTask.LostFocus += TxtNewTask_LostFocus;

            EnsureCompletionDateColumn();   // ensure completion_date column exists
            LoadTasks();                    // load tasks for this student
            LoadCompletedTasks();           // load completed tasks for this student
            CleanupOldCompletedTasks();     // remove completed tasks older than 7 days
            UpdatePieChart();               // update pie chart with 7-day statistics
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

                string query = "INSERT INTO todo_tasks(student_id, task_name, is_completed) VALUES (@id, @task, 0)";
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
        }

        private void RemoveTask_Click(object sender, RoutedEventArgs e)
        {
            // Get the task from the button's DataContext
            if (sender is Button button && button.DataContext is TodoTask task)
            {
                // Ask for confirmation
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to remove the task '{task.task_name}'?",
                    "Remove Task",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Delete the task from database
                    using (var conn = DBHelper.GetConnection())
                    {
                        conn.Open();

                        string query = "DELETE FROM todo_tasks WHERE task_id=@id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", task.task_id);
                        cmd.ExecuteNonQuery();
                    }

                    // Reload tasks
                    LoadTasks();
                    UpdatePieChart(); // Update pie chart after removing task
                    SaveProgressSilent(); // Automatically update todo_progress when removing task
                    MessageBox.Show("Task removed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
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
        }

        #region Pie Chart

        private void UpdatePieChart()
        {
            int completedCount = 0;
            int uncompletedCount = 0;

            using (var conn = DBHelper.GetConnection())
            {
                conn.Open();

                // Get tasks from the last 7 days
                // Count completed tasks from last 7 days and all uncompleted tasks
                string query = @"SELECT 
                                    SUM(CASE WHEN is_completed = 1 AND completion_date >= DATE_SUB(CURDATE(), INTERVAL 7 DAY) THEN 1 ELSE 0 END) as completed,
                                    SUM(CASE WHEN is_completed = 0 THEN 1 ELSE 0 END) as uncompleted
                                FROM todo_tasks 
                                WHERE student_id = @id 
                                AND (is_completed = 0 OR (is_completed = 1 AND completion_date >= DATE_SUB(CURDATE(), INTERVAL 7 DAY)))";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", studentId);

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
                                                SUM(CASE WHEN is_completed = 1 THEN 1 ELSE 0 END) as completed,
                                                SUM(CASE WHEN is_completed = 0 THEN 1 ELSE 0 END) as uncompleted
                                            FROM todo_tasks 
                                            WHERE student_id = @id";
                    
                    MySqlCommand fallbackCmd = new MySqlCommand(fallbackQuery, conn);
                    fallbackCmd.Parameters.AddWithValue("@id", studentId);
                    
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
                pathUncompleted.Visibility = Visibility.Visible;
            }
            else
            {
                pathUncompleted.Visibility = Visibility.Collapsed;
            }

            // If no tasks, show a full circle (gray or white)
            if (completedPercent == 0 && uncompletedPercent == 0)
            {
                PathFigure emptyFigure = new PathFigure
                {
                    StartPoint = new Point(centerX, centerY),
                    IsClosed = true
                };

                emptyFigure.Segments.Add(new LineSegment(new Point(centerX, centerY - radius), true));
                emptyFigure.Segments.Add(new ArcSegment
                {
                    Point = new Point(centerX, centerY - radius),
                    Size = new Size(radius, radius),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc = true
                });

                PathGeometry emptyGeometry = new PathGeometry();
                emptyGeometry.Figures.Add(emptyFigure);
                pathUncompleted.Data = emptyGeometry;
                pathUncompleted.Fill = Brushes.LightGray;
                pathUncompleted.Visibility = Visibility.Visible;
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

    }
}


  

