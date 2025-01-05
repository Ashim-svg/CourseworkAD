using System;
using System.Collections.Generic;
using System.Linq;
using CourseworkAD.Model;
using CourseworkAD.Models;

namespace CourseworkAD.Services
{
    public class TransactionService
    {
        private readonly UserService _userService;

        public TransactionService(UserService userService)
        {
            _userService = userService;
        }

        // Add a new transaction and save it
        public string AddTransaction(Transaction transaction)
        {
            // Debugging: Output the transaction details
            Console.WriteLine($"Adding Transaction: Type = {transaction.Type}, Amount = {transaction.Amount}");

            // If the transaction is a debit (outflow), check if balance is sufficient
            if (transaction.Type == "Debit")
            {
                decimal currentBalance = GetCurrentBalance();
                if (currentBalance < transaction.Amount)
                {
                    return "Error: Insufficient balance for this transaction.";
                }
            }

            // If the transaction is a debt payment, clear the debt
            if (transaction.Type == "Credit")
            {
                ClearDebtFromInflows(transaction.Amount);
            }

            // Proceed to add transaction
            var appData = _userService.LoadData();
            appData.Transactions.Add(transaction);
            _userService.SaveData(appData);

            return "Transaction added successfully!";
        }

        // Get all transactions
        public IEnumerable<Transaction> GetTransactions()
        {
            var appData = _userService.LoadData();
            return appData.Transactions;
        }

        // Get transactions filtered by type
        public IEnumerable<Transaction> GetTransactionsByType(string type)
        {
            var appData = _userService.LoadData();
            return appData.Transactions.Where(t => t.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
        }

        // Get pending debts
        public IEnumerable<Transaction> GetPendingDebts()
        {
            var appData = _userService.LoadData();
            return appData.Transactions.Where(t => t.Type.Equals("Debt", StringComparison.OrdinalIgnoreCase) && !t.IsDebtCleared);
        }

        // Clear a debt
        public bool ClearDebt(int transactionId, decimal paymentAmount)
        {
            var appData = _userService.LoadData();
            var transaction = appData.Transactions.FirstOrDefault(t => t.Id == transactionId && t.Type == "Debt");

            if (transaction == null || transaction.IsDebtCleared)
                return false;

            if (transaction.Amount <= paymentAmount)
            {
                transaction.IsDebtCleared = true;
                transaction.Amount = 0;
            }
            else
            {
                transaction.Amount -= paymentAmount;
            }

            _userService.SaveData(appData);
            return true;
        }

        // Clear debt from inflows
        private void ClearDebtFromInflows(decimal inflowAmount)
        {
            var appData = _userService.LoadData();
            var pendingDebts = appData.Transactions.Where(t => t.Type == "Debt" && !t.IsDebtCleared).OrderBy(t => t.Date);

            foreach (var debt in pendingDebts)
            {
                if (inflowAmount <= 0)
                    break;

                if (debt.Amount <= inflowAmount)
                {
                    inflowAmount -= debt.Amount;
                    debt.Amount = 0;
                    debt.IsDebtCleared = true;
                }
                else
                {
                    debt.Amount -= inflowAmount;
                    inflowAmount = 0;
                }
            }

            _userService.SaveData(appData);
        }

        // Method to get current balance
        private decimal GetCurrentBalance()
        {
            var appData = _userService.LoadData();
            decimal totalInflows = appData.Transactions.Where(t => t.Type == "Credit").Sum(t => t.Amount);
            decimal totalOutflows = appData.Transactions.Where(t => t.Type == "Debit").Sum(t => t.Amount);

            decimal currentBalance = totalInflows - totalOutflows;
            Console.WriteLine($"Calculated Current Balance: {currentBalance} (Inflows: {totalInflows}, Outflows: {totalOutflows})");

            return currentBalance;
        }
    }
}
