using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;



namespace HomeBankingMindHub.Controllers

{

    [Route("api/[controller]")]

    [ApiController]

    public class ClientsController : ControllerBase

    {

        private IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private ICardRepository _cardRepository;


        public ClientsController(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)

        {

            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }



        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()

        {

            try

            {

                var clients = _clientRepository.GetAllClients();



                var clientsDTO = new List<ClientDTO>();



                foreach (Client client in clients)

                {

                    var newClientDTO = new ClientDTO

                    {

                        Id = client.Id,

                        Email = client.Email,

                        FirstName = client.FirstName,

                        LastName = client.LastName,

                        Accounts = client.Accounts.Select(ac => new AccountDTO

                        {

                            Id = ac.Id,

                            Balance = ac.Balance,

                            CreationDate = ac.CreationDate,

                            Number = ac.Number

                        }).ToList(),

                        Loans = client.ClientLoans.Select(cl => new ClientLoanDTO

                        {
                            Id = cl.Id,
                            LoanId = cl.LoanId,
                            Name = cl.Loan.Name,
                            Amount = cl.Amount,
                            Payments = int.Parse(cl.Payments)

                        }).ToList(),

                        Cards = client.Cards.Select(c => new CardDTO
                        {
                            Id = c.Id,
                            CardHolder = c.CardHolder,
                            Color = c.Color,
                            Cvv = c.Cvv,
                            FromDate = c.FromDate,
                            Number = c.Number,
                            ThruDate = c.ThruDate,
                            Type = c.Type
                        }).ToList()

                    };



                    clientsDTO.Add(newClientDTO);

                }





                return Ok(clientsDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }



        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get(long id)

        {

            try

            {

                var client = _clientRepository.FindById(id);

                if (client == null)

                {

                    return Forbid();

                }



                var clientDTO = new ClientDTO

                {

                    Id = client.Id,

                    Email = client.Email,

                    FirstName = client.FirstName,

                    LastName = client.LastName,

                    Accounts = client.Accounts.Select(ac => new AccountDTO

                    {

                        Id = ac.Id,

                        Balance = ac.Balance,

                        CreationDate = ac.CreationDate,

                        Number = ac.Number

                    }).ToList(),

                    Loans = client.ClientLoans.Select(cl => new ClientLoanDTO

                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)

                    }).ToList(),

                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color,
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type
                    }).ToList()

                };



                return Ok(clientDTO);

            }

            catch (Exception ex)

            {

                return StatusCode(500, ex.Message);

            }

        }

        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                Console.WriteLine("Paso las verificaciones, encontro al cliente");

                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                    Loans = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color,
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type
                    }).ToList()
                };

                Console.WriteLine("Creo el DTO");
                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] Client client)
        {
            try
            {
                //validamos datos antes
                if (String.IsNullOrEmpty(client.Email))
                {
                    return StatusCode(403, "Email inválido");
                }
                if (String.IsNullOrEmpty(client.Password))
                {
                    return StatusCode(403, "Contraseña inválida");
                }
                if (String.IsNullOrEmpty(client.FirstName))
                {
                    return StatusCode(403, "Primer nombre inválido");
                }
                if (String.IsNullOrEmpty(client.LastName))
                {
                    return StatusCode(403, "Apellido inválido");
                }

                //buscamos si ya existe el usuario
                //Client user = _clientRepository.FindByEmail(client.Email);

                if (_clientRepository.ExistsByEmail(client.Email))
                {
                    return StatusCode(403, "Email está en uso");
                }

                // verifico si es de la empresa para dar admin
                bool isAdmin = client.Email.Contains("vt.com.ar");

                Client newClient = new Client
                {
                    Email = client.Email,
                    Password = client.Password,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    IsAdmin = isAdmin,
                };

                _clientRepository.Save(newClient);
                return Created("", newClient);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Post()
        {
            try
            {
                // Obtengo email de cliente
                string clientEmail = User.FindFirst("Client")?.Value;

                if (string.IsNullOrEmpty(clientEmail))
                {
                    return Forbid();
                }

                // Obtengo el objeto cliente
                Client client = _clientRepository.FindByEmail(clientEmail);

                // Verifico si el cliente ya tiene 3 cuentas
                int clientAccountsCount = _accountRepository.GetAccountsCountByClient(client.Id);
                if (clientAccountsCount >= 3)
                {
                    return StatusCode(403, "Error. El cliente ya tiene el máximo de cuentas registradas (3).");
                }

                // Creo número de cuenta
                int randomAccountNumber = RandomNumber.GenerateRandomNumber(100000, 999999);
                string accountNumber = $"VIN-{randomAccountNumber}";

                // Verifico si el número de cuenta es único
                while (_accountRepository.ExistsByAccountNumber(accountNumber))
                {
                    // Si ya existe, genero un nuevo número y vuelvo a verificar
                    randomAccountNumber = RandomNumber.GenerateRandomNumber(100000, 999999);
                    accountNumber = $"VIN-{randomAccountNumber}";
                }

                var newAccount = new Account
                {
                    ClientId = client.Id,
                    Number = accountNumber,
                    Balance = 0,
                    CreationDate = DateTime.Now,
                };

                // guardo
                _accountRepository.Save(newAccount);
                // Retorno respuesta
                return Created("", newAccount);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/accounts")]
        public IActionResult GetAccounts()
        {
            try
            {
                // Obtengo el email del cliente
                string clientEmail = User.FindFirst("Client")?.Value;

                if (string.IsNullOrEmpty(clientEmail))
                {
                    return Forbid();
                }

                // Obtengo todas las cuentas asociadas al cliente
                var accounts = _accountRepository.GetAccountsByEmail(clientEmail);

                var accountsDTO = new List<AccountDTO>();

                foreach (Account account in accounts)
                {
                    var newAccountDTO = new AccountDTO
                    {
                        Id = account.Id,

                        Number = account.Number,

                        CreationDate = account.CreationDate,

                        Balance = account.Balance,

                        Transactions = account.Transactions.Select(tr => new TransactionDTO
                        {
                            Id = tr.Id,
                            Type = tr.Type.ToString(),
                            Amount = tr.Amount,
                            Description = tr.Description,
                            Date = tr.Date
                        }).ToList()
                    };

                    accountsDTO.Add(newAccountDTO);
                }

                return Ok(accountsDTO);
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Post([FromBody] CardCreationDTO card)
        {
            try
            {
                //validamos datos antes
                if (String.IsNullOrEmpty(card.Type))
                {
                    return StatusCode(403, "Seleccionar un tipo de tarjeta.");
                }
                if (String.IsNullOrEmpty(card.Color))
                {
                    return StatusCode(403, "Seleccionar un color de tarjeta. ");
                }

                // Obtengo email de cliente
                string clientEmail = User.FindFirst("Client")?.Value;

                if (string.IsNullOrEmpty(clientEmail))
                {
                    return Forbid();
                }

                // Obtengo el cliente
                Client client = _clientRepository.FindByEmail(clientEmail);

                // Verifico si el cliente ya tiene 6 tarjetas
                int clientCardsCount = _cardRepository.GetCardsCountByClient(client.Id);
                if (clientCardsCount >= 6)
                {
                    return StatusCode(403, "Error. El cliente ya tiene el máximo de tarjetas registradas (6).");
                }

                // Verifico si ya hay una tarjeta del mismo tipo y color
                bool hasSameTypeAndColor = _cardRepository.ExistsByTypeAndColor(client.Id, card.Type, card.Color);

                if (hasSameTypeAndColor)
                {
                    return StatusCode(403, "Error. El cliente ya tiene una tarjeta con el mismo tipo y color.");
                }

                // Creo número de tarjeta
                int randomCardNumber = RandomNumber.GenerateRandomNumber(1000, 9999);
                int randomCardNumber1 = RandomNumber.GenerateRandomNumber(1000, 9999);
                int randomCardNumber2 = RandomNumber.GenerateRandomNumber(1000, 9999);
                int randomCardNumber3 = RandomNumber.GenerateRandomNumber(1000, 9999);


                string cardNumber = $"{randomCardNumber}{randomCardNumber1}{randomCardNumber2}{randomCardNumber3}";

                // Verifico si el número de tarjeta es único
                while (_cardRepository.ExistsByCardNumber(cardNumber))
                {
                    // Si ya existe, genero un nuevo número y vuelvo a verificar
                    randomCardNumber = RandomNumber.GenerateRandomNumber(1000, 9999);
                    randomCardNumber1 = RandomNumber.GenerateRandomNumber(1000, 9999);
                    randomCardNumber2 = RandomNumber.GenerateRandomNumber(1000, 9999);
                    randomCardNumber3 = RandomNumber.GenerateRandomNumber(1000, 9999);
                    cardNumber = $"{randomCardNumber}{randomCardNumber1}{randomCardNumber2}{randomCardNumber3}";
                }

                // Agrego formato de tarjeta
                string cardNumberFormat = $"{randomCardNumber}-{randomCardNumber1}-{randomCardNumber2}-{randomCardNumber3}";

                // Creo número de Cvv
                long randomCardCvv = RandomNumber.GenerateRandomNumber(100, 999);

                int cardCvv = (int)randomCardCvv;

                // Convierto la cadena a enum CardType
                if (!Enum.TryParse<CardType>(card.Type, out CardType cardType))
                {
                    return StatusCode(400, "Tipo de tarjeta no válido.");
                }

                // Convierto la cadena a enum CardColor
                if (!Enum.TryParse<CardColor>(card.Color, out CardColor cardColor))
                {
                    return StatusCode(400, "Color de tarjeta no válido.");
                }

                // Creo tarjeta
                var newCard = new Card
                {
                    ClientId = client.Id,
                    CardHolder = client.FirstName + " " + client.LastName,
                    Type = card.Type,
                    Color = card.Color,
                    Number = cardNumberFormat,
                    Cvv = cardCvv,
                    FromDate = DateTime.Now,
                    ThruDate = DateTime.Now.AddYears(5),
                };

                _cardRepository.Save(newCard);
                return Created("", newCard);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/cards")]
        public IActionResult GetCards()
        {
            try
            {
                // Obtengo el email del cliente
                string clientEmail = User.FindFirst("Client")?.Value;

                if (string.IsNullOrEmpty(clientEmail))
                {
                    return Forbid();
                }

                // Obtengo todas las cuentas asociadas al cliente
                var cards = _cardRepository.GetCardsByEmail(clientEmail);

                var cardsDTO = new List<CardDTO>();

                foreach (Card card in cards)
                {
                    var newCardDTO = new CardDTO
                    {
                        Id = card.Id,
                        CardHolder = card.CardHolder,
                        Type = card.Type,
                        Color = card.Color,
                        Number = card.Number,
                        Cvv = card.Cvv,
                        FromDate = card.FromDate,
                        ThruDate = card.ThruDate
                    };

                    cardsDTO.Add(newCardDTO);
                }

                return Ok(cardsDTO);
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

}
