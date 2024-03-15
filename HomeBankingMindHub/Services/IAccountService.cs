using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IAccountService
    {
        bool ExistsByAccountNumber(string accountNumber);
        bool AccountBelongsToClient(long AccountClientId, long clientId);
        IEnumerable<AccountDTO> GetAllAccountsDTO();
        IEnumerable<Account> GetAccountsByEmail(string email);
        Account GetAccountByNumber(string accountNumber);
        AccountDTO GetAccountDTOByIdAndClientEmail(long clientId, string email);
        IEnumerable<AccountDTO> GetAccountsDTOByEmail(string clientEmail);
        int GetAccountsCountByClient(long clientId);
        string CreateAccountNumber();
        Account CreateAccount(long clientId, string accountNumber);
        void SaveAccount(Account account);
    }
}
