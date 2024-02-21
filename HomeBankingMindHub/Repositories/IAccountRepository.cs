using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();

        Account GetAccountById(long id);

        void Save(Account account);

    }
}
