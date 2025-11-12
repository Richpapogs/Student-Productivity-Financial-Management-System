using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

            // Watermark behavior
            txtNewTask.GotFocus += TxtNewTask_GotFocus;
            txtNewTask.LostFocus += TxtNewTask_LostFocus;

            LoadTasks();                    // load tasks for this student
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

                foreach (var task in tasks)
                {
                    string query = "UPDATE todo_tasks SET is_completed=@completed WHERE task_id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@completed", task.is_completed ? 1 : 0);
                    cmd.Parameters.AddWithValue("@id", task.task_id);
                    cmd.ExecuteNonQuery();
                }
            }

            SaveProgress();
            LoadTasks();
            LoadCompletedTasks();
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

                string query = "UPDATE todo_tasks SET is_completed=0 WHERE task_id=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", task.task_id);
                cmd.ExecuteNonQuery();
            }

            LoadTasks();
            LoadCompletedTasks();
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
        }

        private void SaveProgress()
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

                // Insert into progress history
                string insertQuery = "INSERT INTO todo_progress(student_id, date_recorded, completed_tasks, total_tasks) VALUES(@id, CURDATE(), @completed, @total)";
                MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@id", studentId);
                insertCmd.Parameters.AddWithValue("@completed", completedTasks);
                insertCmd.Parameters.AddWithValue("@total", totalTasks);
                insertCmd.ExecuteNonQuery();
            }

            // Optional notification
            MessageBox.Show("Progress saved! 🎯", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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


    }
}


  

