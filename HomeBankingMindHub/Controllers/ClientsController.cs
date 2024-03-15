using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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

        private IClientService _clientService;
        private IAccountService _accountService;
        private ICardService _cardService;

        public ClientsController(IClientService clientService, IAccountService accountService, ICardService cardService)
        {
            _clientService = clientService;
            _accountService = accountService;
            _cardService = cardService;
        }


        
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()
        {
            try
            {
                var clientsDTO = _clientService.GetAllClientsDTO();

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
                var client = _clientService.GetClientDTOById(id);

                if (client == null)

                {
                    return Forbid();
                }

                return Ok(client);
             
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrent()
        {
            try
            {
                string email = User.FindFirst("Client") == null ? User.FindFirst("Admin").Value : User.FindFirst("Client").Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Forbid();
                }

                ClientDTO client = _clientService.GetClientDTOByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                return Ok(client);
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
                if (_clientService.ClientExistsByEmail(client.Email))
                {
                    return StatusCode(403, "Email en uso");
                }

                // verifico si es de la empresa para dar admin
                bool isAdmin = client.Email.Contains("vt.com.ar");

                // Hash pass
                string passwordHash = PasswordHashUtils.HashPassword(client.Password);

                // Creo cliente
                Client newClient = _clientService.CreateClient(client.Email, passwordHash, client.FirstName, client.LastName, isAdmin);

                _clientService.SaveClient(newClient);

                // Obtengo cliente
                newClient = _clientService.GetClientByEmail(client.Email);

                // Creo número de cuenta
                var accountNumber = _accountService.CreateAccountNumber();

                // Creo cuenta
                var newAccount = _accountService.CreateAccount(newClient.Id, accountNumber);

                // guardo
                _accountService.SaveAccount(newAccount);

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
                string clientEmail = User.FindFirst("Client") == null ? User.FindFirst("Admin").Value : User.FindFirst("Client").Value;
                if (string.IsNullOrEmpty(clientEmail))
                {
                    return Forbid();
                }

                // Obtengo el objeto cliente
                Client client = _clientService.GetClientByEmail(clientEmail);

                // Verifico si el cliente ya tiene 3 cuentas
                int clientAccountsCount = _accountService.GetAccountsCountByClient(client.Id);
                if (clientAccountsCount >= 3)
                {
                    return StatusCode(403, "Error. El cliente ya tiene el máximo de cuentas registradas (3).");
                }

                // Creo número de cuenta
                string accountNumber = _accountService.CreateAccountNumber();

                // Creo cuenta
                var newAccount = _accountService.CreateAccount(client.Id, accountNumber);

                // guardo
                _accountService.SaveAccount(newAccount);
                // Retorno respuesta
                return Created("", newAccount);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetAccounts()
        {
            try
            {
                // Obtengo el email del cliente
                string clientEmail = User.FindFirst("Client") == null ? User.FindFirst("Admin").Value : User.FindFirst("Client").Value;
                if (string.IsNullOrEmpty(clientEmail))
                {
                    return Forbid();
                }

                // Obtengo todas las cuentas asociadas al cliente
                var accounts = _accountService.GetAccountsByEmail(clientEmail);

                return Ok(accounts);
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
                string clientEmail = User.FindFirst("Client") == null ? User.FindFirst("Admin").Value : User.FindFirst("Client").Value;
                if (string.IsNullOrEmpty(clientEmail))
                {
                    return Forbid();
                }

                // Obtengo el cliente
                Client client = _clientService.GetClientByEmail(clientEmail);

                // Verifico si el cliente ya tiene 6 tarjetas
                int clientCardsCount = _cardService.GetCardsCountByClient(client.Id);
                if (clientCardsCount >= 6)
                {
                    return StatusCode(403, "Error. El cliente ya tiene el máximo de tarjetas registradas (6).");
                }

                // Verifico si ya hay una tarjeta del mismo tipo y color
                bool hasSameTypeAndColor = _cardService.ExistsByTypeAndColor(client.Id, card.Type, card.Color);
                if (hasSameTypeAndColor)
                {
                    return StatusCode(403, "Error. El cliente ya tiene una tarjeta con el mismo tipo y color.");
                }

                // Creo número de tarjeta
                string cardNumber = _cardService.CreateCardNumber();

                // Creo número de Cvv
                int cardCvv = _cardService.CreateCardCvv();

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
                var newCard = _cardService.CreateCard(client.Id, client.FirstName, client.LastName, card.Type, card.Color, cardNumber, cardCvv);

                _cardService.SaveCard(newCard);
                return Created("", newCard);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCards()
        {
            try
            {
                // Obtengo el email del cliente
                string clientEmail = User.FindFirst("Client") == null ? User.FindFirst("Admin").Value : User.FindFirst("Client").Value;
                if (string.IsNullOrEmpty(clientEmail))
                {
                    return Forbid();
                }

                // Obtengo todas las cards asociadas al cliente
                var cardsDTO = _cardService.GetCardsByEmail(clientEmail);

                return Ok(cardsDTO);
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

}
