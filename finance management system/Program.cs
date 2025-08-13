using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category}.");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Mobile Money] Sent {transaction.Amount:C} for {transaction.Category}.");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Crypto Wallet] Transferred {transaction.Amount:C} for {transaction.Category}.");
        }
    }

    
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. New balance: {Balance:C}");
        }
    }

    
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance)
        {
        }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
            }
            else
            {
                Balance -= transaction.Amount;
                Console.WriteLine($"Transaction applied. Updated balance: {Balance:C}");
            }
        }
    }

    
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            
            var savingsAccount = new SavingsAccount("ACC12345", 1000m);

            
            var t1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
            var t2 = new Transaction(2, DateTime.Now, 200m, "Utilities");
            var t3 = new Transaction(3, DateTime.Now, 300m, "Entertainment");

            
            ITransactionProcessor mobileProcessor = new MobileMoneyProcessor();
            ITransactionProcessor bankProcessor = new BankTransferProcessor();
            ITransactionProcessor cryptoProcessor = new CryptoWalletProcessor();

            mobileProcessor.Process(t1);
            bankProcessor.Process(t2);
            cryptoProcessor.Process(t3);

            
            savingsAccount.ApplyTransaction(t1);
            savingsAccount.ApplyTransaction(t2);
            savingsAccount.ApplyTransaction(t3);

            
            _transactions.AddRange(new[] { t1, t2, t3 });
        }
    }

    
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}
