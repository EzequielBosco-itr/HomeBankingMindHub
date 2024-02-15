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

        }
    }
}
