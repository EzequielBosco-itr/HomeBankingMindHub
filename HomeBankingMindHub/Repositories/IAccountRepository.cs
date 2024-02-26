using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();

        Account GetAccountById(long id);
        Account GetAccountByIdAndClientEmail(long id, string email);

        void Save(Account account);

    }
}
