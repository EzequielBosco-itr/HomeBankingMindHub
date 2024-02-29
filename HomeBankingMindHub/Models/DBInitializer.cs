using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HomeBankingMindHub.Models
{
    public class DBInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            //if (!context.Clients.Any())
            //{
            //    var clients = new Client[]
            //    {
            //        new Client 
            //        { 
            //            Email = "vcoronado@gmail.com", 
            //            FirstName="Victor", 
            //            LastName="Coronado", 
            //            Password="123456"
            //        },
            //        new Client
            //        {
            //            Email = "ezequielbosco@gmail.com",
            //            FirstName = "Ezequiel",
            //            LastName = "Bosco",
            //            Password = "123456"
            //        },
            //        new Client
            //        {
            //            Email = "luca@gmail.com",
            //            FirstName = "Luca",
            //            LastName = "Corona",
            //            Password = "123456"
            //        },
            //    };

            //    context.Clients.AddRange(clients);

            //    context.SaveChanges();
            //}

            //if (!context.Set<Account>().Any())
            //{
            //    var accountVictor = context.Clients.FirstOrDefault(c => c.Email == "vcoronado@gmail.com");
            //    var accountEzequiel = context.Clients.FirstOrDefault(c => c.Email == "ezequielbosco@gmail.com");
            //    var accountLuca = context.Clients.FirstOrDefault(c => c.Email == "luca@gmail.com");

            //    AddAccountIfNotNull(context, accountVictor, "VIN001", 0);
            //    AddAccountIfNotNull(context, accountEzequiel, "EZN001", 100000);
            //    AddAccountIfNotNull(context, accountLuca, "LUN001", 500000);

            //    context.SaveChanges();

            //}


            //if (!context.Set<Transaction>().Any())
            //{

            //    var account1 = context.Accounts.FirstOrDefault(c => c.Number == "VIN001");
            //    var account2 = context.Accounts.FirstOrDefault(c => c.Number == "EZN001");
            //    var account3 = context.Accounts.FirstOrDefault(c => c.Number == "LUN001");


            //    AddTransaction(context, TransactionType.CREDIT, account1, 10000, "Transferencia recibida");
            //    AddTransaction(context, TransactionType.DEBIT, account1, -2000, "Compra en tienda mercado libre");
            //    AddTransaction(context, TransactionType.DEBIT, account1, -3000, "Compra en tienda xxxx");

            //    AddTransaction(context, TransactionType.CREDIT, account2, 200000, "Transferencia recibida");
            //    AddTransaction(context, TransactionType.DEBIT, account2, -60000, "Compra en tienda mercado libre");
            //    AddTransaction(context, TransactionType.DEBIT, account2, -20000, "Compra en tienda xxxx");

            //    AddTransaction(context, TransactionType.CREDIT, account3, 300000, "Transferencia recibida");
            //    AddTransaction(context, TransactionType.DEBIT, account3, -40000, "Compra en tienda mercado libre");
            //    AddTransaction(context, TransactionType.DEBIT, account3, -100000, "Compra en tienda xxxx");

            //    context.SaveChanges();

            //}

            if (!context.Loans.Any())
            {
                //crearemos 3 prestamos Hipotecario, Personal y Automotriz
                var loans = new Loan[]
                {
                    new Loan { Name = "Hipotecario", MaxAmount = 500000, Payments = "12,24,36,48,60" },
                    new Loan { Name = "Personal", MaxAmount = 100000, Payments = "6,12,24" },
                    new Loan { Name = "Automotriz", MaxAmount = 300000, Payments = "6,12,24,36" },
                };

                foreach(Loan loan in loans)
                {
                    context.Loans.Add(loan);
                }

                context.SaveChanges();

                //ahora agregaremos los clientloan (Prestamos del cliente)
                //usaremos al único cliente que tenemos y le agregaremos un préstamo de cada item
                //var client1 = context.Clients.FirstOrDefault(c => c.Email == "vcoronado@gmail.com");
                //if (client1 != null)
                //{
                //    //ahora usaremos los 3 tipos de prestamos
                //    var loan1 = context.Loans.FirstOrDefault(l => l.Name == "Hipotecario");
                //    if (loan1 != null)
                //    {
                //        var clientLoan1 = new ClientLoan
                //        {
                //            Amount=400000, ClientId = client1.Id, LoanId= loan1.Id, Payments="60"
                //        };
                //        context.ClientLoans.Add(clientLoan1);
                //    }

                //    var loan2 = context.Loans.FirstOrDefault(l => l.Name == "Personal");
                //    if (loan2 != null)
                //    {
                //        var clientLoan2 = new ClientLoan
                //        {
                //            Amount = 50000,
                //            ClientId = client1.Id,
                //            LoanId = loan2.Id,
                //            Payments = "12"
                //        };
                //        context.ClientLoans.Add(clientLoan2);
                //    }

                //    var loan3 = context.Loans.FirstOrDefault(l => l.Name == "Automotriz");
                //    if (loan3 != null)
                //    {
                //        var clientLoan3 = new ClientLoan
                //        {
                //            Amount = 100000,
                //            ClientId = client1.Id,
                //            LoanId = loan3.Id,
                //            Payments = "24"
                //        };
                //        context.ClientLoans.Add(clientLoan3);
                //    }

                //    //guardamos todos los prestamos
                //    context.SaveChanges();

                //}

            }

            //if (!context.Cards.Any())
            //{
            //    //buscamos al unico cliente
            //    var client1 = context.Clients.FirstOrDefault(c => c.Email == "vcoronado@gmail.com");
            //    if (client1 != null)
            //    {
            //        //le agregamos 2 tarjetas de crédito una GOLD y una TITANIUM, de tipo DEBITO Y CREDITO RESPECTIVAMENTE
            //        var cards = new Card[]
            //        {
            //            new Card {
            //                ClientId= client1.Id,
            //                CardHolder = client1.FirstName + " " + client1.LastName,
            //                Type = CardType.DEBIT.ToString(),
            //                Color = CardColor.GOLD.ToString(),
            //                Number = "3325-6745-7876-4445",
            //                Cvv = 990,
            //                FromDate= DateTime.Now,
            //                ThruDate= DateTime.Now.AddYears(4),
            //            },
            //            new Card {
            //                ClientId= client1.Id,
            //                CardHolder = client1.FirstName + " " + client1.LastName,
            //                Type = CardType.CREDIT.ToString(),
            //                Color = CardColor.TITANIUM.ToString(),
            //                Number = "2234-6745-552-7888",
            //                Cvv = 750,
            //                FromDate= DateTime.Now,
            //                ThruDate= DateTime.Now.AddYears(5),
            //            },
            //        };

            //        foreach (Card card in cards)
            //        {
            //            context.Cards.Add(card);
            //        }
            //        context.SaveChanges();
            //    }
            //}

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
