using System;

namespace CourseworkAD.Model
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; } // Credit, Debit, Debt
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Tags { get; set; }
        public string Notes { get; set; }
        public bool IsDebtCleared { get; set; }
        public string Username { get; set; } // For debt transactions


    }
}
