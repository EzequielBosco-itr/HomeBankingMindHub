using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HomeBankingMindHub.Models
{
    public class DBInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client 
                    { 
                        Email = "vcoronado@gmail.com", 
                        FirstName="Victor", 
                        LastName="Coronado", 
                        Password="123456"
                    },
                    new Client
                    {
                        Email = "ezequielbosco@gmail.com",
                        FirstName = "Ezequiel",
                        LastName = "Bosco",
                        Password = "123456"
                    },
                    new Client
                    {
                        Email = "luca@gmail.com",
                        FirstName = "Luca",
                        LastName = "Corona",
                        Password = "123456"
                    },
                };

                context.Clients.AddRange(clients);

                context.SaveChanges();
            }

            if (!context.Set<Account>().Any())
            {
                var accountVictor = context.Clients.FirstOrDefault(c => c.Email == "vcoronado@gmail.com");
                var accountEzequiel = context.Clients.FirstOrDefault(c => c.Email == "ezequielbosco@gmail.com");
                var accountLuca = context.Clients.FirstOrDefault(c => c.Email == "luca@gmail.com");

                AddAccountIfNotNull(context, accountVictor, "VIN001", 0);
                AddAccountIfNotNull(context, accountEzequiel, "EZN001", 100000);
                AddAccountIfNotNull(context, accountLuca, "LUN001", 500000);

                context.SaveChanges();

            }


            if (!context.Set<Transaction>().Any())
            {

                var account1 = context.Account.FirstOrDefault(c => c.Number == "VIN001");
                var account2 = context.Account.FirstOrDefault(c => c.Number == "EZN001");
                var account3 = context.Account.FirstOrDefault(c => c.Number == "LUN001");


                AddTransaction(context, TransactionType.CREDIT, account1, 10000, "Transferencia recibida");
                AddTransaction(context, TransactionType.DEBIT, account1, -2000, "Compra en tienda mercado libre");
                AddTransaction(context, TransactionType.DEBIT, account1, -3000, "Compra en tienda xxxx");

                AddTransaction(context, TransactionType.CREDIT, account2, 200000, "Transferencia recibida");
                AddTransaction(context, TransactionType.DEBIT, account2, -60000, "Compra en tienda mercado libre");
                AddTransaction(context, TransactionType.DEBIT, account2, -20000, "Compra en tienda xxxx");

                AddTransaction(context, TransactionType.CREDIT, account3, 300000, "Transferencia recibida");
                AddTransaction(context, TransactionType.DEBIT, account3, -40000, "Compra en tienda mercado libre");
                AddTransaction(context, TransactionType.DEBIT, account3, -100000, "Compra en tienda xxxx");

                context.SaveChanges();

            }
        }
        private static void AddAccountIfNotNull(DbContext context, Client client, string accountNumber, double balance)
        {
            if (client != null)
            {
                var account = new Account
                {
                    ClientId = client.Id,
                    CreationDate = DateTime.Now,
                    Number = accountNumber,
                    Balance = balance
                };

                context.Set<Account>().Add(account);
            }
        }

        private static void AddTransaction(DbContext context, TransactionType type, Account account, double amount, string description)
        {
            if (account != null)
            {
                var transaction = new Transaction
                {
                    AccountId = account.Id,
                    Type = type,
                    Date = DateTime.Now,
                    Amount = amount,
                    Description = description
                };

                context.Set<Transaction>().Add(transaction);
            }
        }
    }
}
