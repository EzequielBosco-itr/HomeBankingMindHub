using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Implements
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public Transaction CreateTransaction(TransactionType type, double amount, string description, long accountId, string accountNumber)
        {
            Transaction newTransactionDebit = new Transaction
            {
                Type = type,
                Amount = -amount,
                Description = $"{description} {accountNumber}",
                AccountId = accountId,
            };
            return newTransactionDebit;
        }

        public void SaveTransaction(Transaction transaction) 
        {
            _transactionRepository.Save(transaction);
        }
    }
}
