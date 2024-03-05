using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.DTOs;
using System.Net;
using System.Transactions;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ITransactionRepository _transactionRepository;

        public TransactionsController(IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult transfers([FromBody] TransferDTO transaction)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    // Valido datos
                    if (String.IsNullOrEmpty(transaction.FromAccountNumber))
                    {
                        return StatusCode(403, "Cuenta de origen inválida.");
                    }

                    if (String.IsNullOrEmpty(transaction.ToAccountNumber))
                    {
                        return StatusCode(403, "Cuenta de destino inválida.");
                    }
                    if (transaction.Amount <= 0)
                    {
                        return StatusCode(403, "Monto inválida, debe ser mayor a 0.");
                    }
                    if (String.IsNullOrEmpty(transaction.Description))
                    {
                        return StatusCode(403, "Descripción inválida.");
                    }

                    // Verifico que los números de cuenta no sean iguales
                    if (transaction.FromAccountNumber == transaction.ToAccountNumber)
                    {
                        return StatusCode(403, "La cuenta de destino es la misma que la de origen.");
                    }

                    // Verifico si existe la cuenta de origen 
                    if (!_accountRepository.ExistsByAccountNumber(transaction.FromAccountNumber))
                    {
                        return StatusCode(403, "La cuenta de origen no existe.");
                    }

                    // Verifico si existe la cuenta de destino
                    if (!_accountRepository.ExistsByAccountNumber(transaction.ToAccountNumber))
                    {
                        return StatusCode(403, "La cuenta de destino no existe.");
                    }

                    // Obtengo email de cliente
                    string clientEmail = User.FindFirst("Client")?.Value;

                    if (string.IsNullOrEmpty(clientEmail))
                    {
                        return Forbid();
                    }

                    // Obtengo el objeto cliente
                    Client client = _clientRepository.FindByEmail(clientEmail);

                    // Verifico si la cuenta de origen pertenece al cliente autenticado
                    var accountOrigin = _accountRepository.FindByNumber(transaction.FromAccountNumber);

                    if (accountOrigin == null || accountOrigin.ClientId != client.Id)
                    {
                        return StatusCode(403, "La cuenta de origen no pertenece al cliente autenticado.");
                    }

                    // Verifico que la cuenta origen tenga el monto disponible
                    if (accountOrigin.Balance < transaction.Amount)
                    {
                        return StatusCode(403, "La transferencia supera el monto disponible en la cuenta.");
                    }

                    Transaction newTransactionDebit = new Transaction
                    {
                        Type = TransactionType.DEBIT,
                        Amount = -transaction.Amount,
                        Description = transaction.Description + " " + transaction.FromAccountNumber,
                        AccountId = accountOrigin.Id,
                        Date = DateTime.Now
                    };

                    var accountTarget = _accountRepository.FindByNumber(transaction.ToAccountNumber);

                    Transaction newTransactionCredit = new Transaction
                    {
                        Type = TransactionType.CREDIT,
                        Amount = transaction.Amount,
                        Description = transaction.Description + " " + transaction.ToAccountNumber,
                        AccountId = accountTarget.Id,
                        Date = DateTime.Now
                    };

                    _transactionRepository.Save(newTransactionDebit);
                    _transactionRepository.Save(newTransactionCredit);

                    accountOrigin.Balance -= transaction.Amount;
                    accountTarget.Balance += transaction.Amount;

                    _accountRepository.Save(accountOrigin);
                    _accountRepository.Save(accountTarget);

                    scope.Complete();
                    return Created("", newTransactionDebit);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
        }

    }
}
