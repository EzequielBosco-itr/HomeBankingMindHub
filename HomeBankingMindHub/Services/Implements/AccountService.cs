using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Models;
using System.Security.Principal;
using HomeBankingMindHub.Utils;
using HomeBankingMindHub.DTOs;

namespace HomeBankingMindHub.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public bool ExistsByAccountNumber(string accountNumber)
        {
            var exists = _accountRepository.ExistsByAccountNumber(accountNumber);
            if (!exists)
            {
                throw new Exception("La cuenta seleccionada no existe.");
            }
            return exists;
        }

        public bool AccountBelongsToClient(long accountClientId, long clientId) 
        {
            bool BelongsTo = accountClientId == clientId;
            if (!BelongsTo)
            {
                throw new Exception("La cuenta de origen no pertenece al cliente autenticado.");
            }
            return BelongsTo;
        }

        public IEnumerable<AccountDTO> GetAllAccountsDTO()
        {
            var accounts = _accountRepository.GetAllAccounts();
            var accountsDTO = new List<AccountDTO>();

            foreach (Account account in accounts)
            {
                var newAccountDTO = new AccountDTO(account);

                accountsDTO.Add(newAccountDTO);
            }
            return accountsDTO;
        }

        public IEnumerable<Account> GetAccountsByEmail(string email) 
        {
            var accounts = _accountRepository.GetAccountsByEmail(email);
            return accounts;
        }

        public Account GetAccountByNumber(string accountNumber) 
        {
            var account = _accountRepository.FindByNumber(accountNumber);
            if (account == null)
            {
                throw new Exception("La cuenta no fue encontrada o es nula.");
            }
            return account;
        }

        public AccountDTO GetAccountDTOByIdAndClientEmail(long clientId, string email) 
        {
            var account = _accountRepository.GetAccountByIdAndClientEmail(clientId, email);
            if (account == null)
            {
                throw new Exception("Cuenta no encontrada por id y email del cliente");
            }

            var accountDTO = new AccountDTO(account);
            return accountDTO;
        }

        public IEnumerable<AccountDTO> GetAccountsDTOByEmail(string clientEmail)
        {

            var accounts = _accountRepository.GetAccountsByEmail(clientEmail);

            var accountsDTO = new List<AccountDTO>();

            foreach (Account account in accounts)
            {
                var newAccountDTO = new AccountDTO(account);
                accountsDTO.Add(newAccountDTO);
            };
            return accountsDTO;
        }

        public int GetAccountsCountByClient(long clientId)
        {
            int clientAccountsCount = _accountRepository.GetAccountsCountByClient(clientId);
            if (clientAccountsCount >= 3)
            {
                throw new Exception("Error. El cliente ya tiene el máximo de cuentas registradas (3).");
            }
            return clientAccountsCount;
        }

        public string CreateAccountNumber()
        {
            int randomAccountNumber = RandomNumberUtils.GenerateRandomNumber(100000, 999999);
            string accountNumber = $"VIN-{randomAccountNumber}";

            // Verifico si el número de cuenta es único
            while (_accountRepository.ExistsByAccountNumber(accountNumber))
            {
                // Si ya existe, genero un nuevo número y vuelvo a verificar
                randomAccountNumber = RandomNumberUtils.GenerateRandomNumber(100000, 999999);
                accountNumber = $"VIN-{randomAccountNumber}";
            }
            return accountNumber;
        }

        public Account CreateAccount(long clientId, string accountNumber)
        {
            var newAccount = new Account
            {
                ClientId = clientId,
                Number = accountNumber,
                Balance = 0,
                CreationDate = DateTime.Now,
            };
            return newAccount;
        }

        public void SaveAccount(Account account)
        {
            _accountRepository.Save(account);
        }
    }
}
