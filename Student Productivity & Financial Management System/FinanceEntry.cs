using System;

namespace StudentProductivityApp.Models
{
    public class FinanceEntry
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
