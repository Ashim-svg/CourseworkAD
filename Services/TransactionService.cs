using CourseworkAD.Model;

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
            if (_userService.CurrentUser == null)
            {
                return "Error: No user is logged in.";
            }

            // Associate the transaction with the logged-in user's username
            transaction.Username = _userService.CurrentUser.Username;

            // Debugging: Output the transaction details
            Console.WriteLine($"Adding Transaction for {transaction.Username}: Type = {transaction.Type}, Amount = {transaction.Amount}");

            // If the transaction is a debit (outflow), check if balance is sufficient
            if (transaction.Type == "Debit")
            {
                decimal currentBalance = GetCurrentBalance();
                if (currentBalance < transaction.Amount)
                {
                    return "Error: Insufficient balance for this transaction.";
                }
            }

            // If the transaction is a credit (inflow), clear any pending debts
            if (transaction.Type == "Credit")
            {
                ClearDebtFromInflows(transaction.Amount);
            }

            // Add the transaction to the user's data
            var appData = _userService.LoadData();
            appData.Transactions.Add(transaction);
            _userService.SaveData(appData);

            return "Transaction added successfully!";
        }

        // Get all transactions for the currently logged-in user
        public IEnumerable<Transaction> GetTransactions()
        {
            if (_userService.CurrentUser == null)
            {
                return Enumerable.Empty<Transaction>();
            }

            var appData = _userService.LoadData();
            return appData.Transactions.Where(t => t.Username == _userService.CurrentUser.Username);
        }

        // Get transactions filtered by type for the currently logged-in user
        public IEnumerable<Transaction> GetTransactionsByType(string type)
        {
            if (_userService.CurrentUser == null)
            {
                return Enumerable.Empty<Transaction>();
            }

            var appData = _userService.LoadData();
            return appData.Transactions
                .Where(t => t.Type.Equals(type, StringComparison.OrdinalIgnoreCase)
                            && t.Username == _userService.CurrentUser.Username);
        }

        // Get pending debts for the currently logged-in user
        public IEnumerable<Transaction> GetPendingDebts()
        {
            if (_userService.CurrentUser == null)
            {
                return Enumerable.Empty<Transaction>();
            }

            var appData = _userService.LoadData();
            return appData.Transactions.Where(t => t.Type.Equals("Debt", StringComparison.OrdinalIgnoreCase)
                                               && !t.IsDebtCleared
                                               && t.Username == _userService.CurrentUser.Username);
        }

        // Clear a specific debt for the currently logged-in user
        public bool ClearDebt(int transactionId, decimal paymentAmount)
        {
            if (_userService.CurrentUser == null)
            {
                return false;
            }

            var appData = _userService.LoadData();
            var transaction = appData.Transactions.FirstOrDefault(t => t.Id == transactionId
                                                                       && t.Type == "Debt"
                                                                       && t.Username == _userService.CurrentUser.Username);

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

        // Clear debts automatically from inflows
        private void ClearDebtFromInflows(decimal inflowAmount)
        {
            if (_userService.CurrentUser == null)
            {
                return;
            }

            var appData = _userService.LoadData();
            var pendingDebts = appData.Transactions
                .Where(t => t.Type == "Debt"
                            && !t.IsDebtCleared
                            && t.Username == _userService.CurrentUser.Username)
                .OrderBy(t => t.Date);

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

        // Get the current balance for the logged-in user
        private decimal GetCurrentBalance()
        {
            if (_userService.CurrentUser == null)
            {
                return 0;
            }

            var appData = _userService.LoadData();
            decimal totalInflows = appData.Transactions
                .Where(t => t.Type == "Credit" && t.Username == _userService.CurrentUser.Username)
                .Sum(t => t.Amount);
            decimal totalOutflows = appData.Transactions
                .Where(t => t.Type == "Debit" && t.Username == _userService.CurrentUser.Username)
                .Sum(t => t.Amount);

            decimal currentBalance = totalInflows - totalOutflows;
            Console.WriteLine($"Calculated Current Balance for {_userService.CurrentUser.Username}: {currentBalance} (Inflows: {totalInflows}, Outflows: {totalOutflows})");

            return currentBalance;
        }
    }
}
