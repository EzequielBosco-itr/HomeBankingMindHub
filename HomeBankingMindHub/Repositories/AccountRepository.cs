using HomeBankingMindHub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace HomeBankingMindHub.Repositories
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return FindAll()
                .Include(account => account.Transactions);
        }

        public Account GetAccountById(long id)
        {
            return FindByCondition(account => account.Id == id)
                .Include(account => account.Transactions)
                .FirstOrDefault();
        }

        public Account GetAccountByIdAndClientEmail(long id, string email)
        {
            return FindByCondition(account => account.Id == id && account.Client.Email == email)
                .Include(account => account.Transactions)
                .FirstOrDefault();
        }

        public void Save(Account account)
        {
            Create(account);
            SaveChanges();
        }

        public IEnumerable<Account> GetAccountsByEmail(string clientEmail)
        {
            return FindByCondition(account => account.Client.Email == clientEmail)
            .Include(account => account.Transactions)
            .ToList();
        }

        public int GetAccountsCountByClient(long clientId)
        {
            return FindByCondition(account => account.Client.Id == clientId).Count();
        }

        public bool ExistsByAccountNumber(string accountNumber)
        {
            return FindByCondition(account => account.Number == accountNumber).Any();
        }
    }
}
