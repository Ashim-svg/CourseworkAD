using System;

namespace CourseworkAD.Model
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; } // Credit, Debit, or Debt
        public decimal Amount { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; }
        public string Tags { get; set; } // Comma-separated tags
        public bool IsDebtCleared { get; set; } = false; // For debt transactions
    }
}
