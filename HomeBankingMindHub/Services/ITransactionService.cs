using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ITransactionService
    {
        Transaction CreateTransaction(TransactionType type, double amount, string description, long accountId, string accountNumber);
        void SaveTransaction(Transaction transaction);
    }
}
