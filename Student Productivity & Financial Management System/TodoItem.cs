namespace StudentProductivityApp.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Task { get; set; }

        // Added for tracking completion (required by DashboardForm)
        public bool IsCompleted { get; set; } = false;
    }
}
