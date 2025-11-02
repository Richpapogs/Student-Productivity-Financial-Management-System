namespace StudentProductivityApp.Models
{
    using System.Collections.Generic;

    public class User
    {
        public string StudentId { get; set; }
        public string Password { get; set; }

        // Profile fields
        public string FullName { get; set; }
        public string Gender { get; set; }             // "M" or "F"
        public string CourseSection { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }

        // Financial / tasks
        public decimal Budget { get; set; } = 0m;
        public List<TodoItem> Todos { get; set; } = new List<TodoItem>();
        public List<FinanceEntry> Finances { get; set; } = new List<FinanceEntry>();
    }
}
