using Microsoft.EntityFrameworkCore;

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
    }
}
