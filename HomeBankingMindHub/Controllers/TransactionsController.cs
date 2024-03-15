﻿using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.DTOs;
using System.Net;
using System.Transactions;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Services.Implements;
using Microsoft.Identity.Client;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private IClientService _clientService;
        private IAccountService _accountService;
        private ITransactionService _transactionService;

        public TransactionsController(IClientService clientService, IAccountService accountService, ITransactionService transactionService)
        {
            _clientService = clientService;
            _accountService = accountService;
            _transactionService = transactionService;
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
                    if (!_accountService.ExistsByAccountNumber(transaction.FromAccountNumber))
                    {
                        return StatusCode(403, "La cuenta de origen no existe.");
                    }

                    // Verifico si existe la cuenta de destino
                    if (!_accountService.ExistsByAccountNumber(transaction.ToAccountNumber))
                    {
                        return StatusCode(403, "La cuenta de destino no existe.");
                    }

                    // Obtengo email de cliente
                    string clientEmail = User.FindFirst("Client") == null ? User.FindFirst("Admin").Value : User.FindFirst("Client").Value;
                    if (string.IsNullOrEmpty(clientEmail))
                    {
                        return Forbid();
                    }

                    // Obtengo el objeto cliente
                    Client client = _clientService.GetClientByEmail(clientEmail);

                    // Verifico si la cuenta de origen pertenece al cliente autenticado
                    var accountOrigin = _accountService.GetAccountByNumber(transaction.FromAccountNumber);

                    bool belongsTo = _accountService.AccountBelongsToClient(accountOrigin.ClientId, client.Id);
                    if (!belongsTo)
                    {
                        return StatusCode(403, "La cuenta de origen no pertenece al cliente autenticado.");
                    }

                    // Verifico que la cuenta origen tenga el monto disponible
                    if (accountOrigin.Balance < transaction.Amount)
                    {
                        return StatusCode(403, "La transferencia supera el monto disponible en la cuenta.");
                    }

                    // creo transferencia debito
                    Transaction newTransactionDebit = _transactionService.CreateTransaction(TransactionType.DEBIT, transaction.Amount, transaction.Description, accountOrigin.Id, transaction.ToAccountNumber);

                    // creo transferencia credito
                    var accountTarget = _accountService.GetAccountByNumber(transaction.ToAccountNumber);


                    Transaction newTransactionCredit = _transactionService.CreateTransaction(TransactionType.CREDIT, transaction.Amount, transaction.Description, accountTarget.Id, transaction.FromAccountNumber);

                    _transactionService.SaveTransaction(newTransactionDebit);
                    _transactionService.SaveTransaction(newTransactionCredit);

                    accountOrigin.Balance -= transaction.Amount;
                    accountTarget.Balance += transaction.Amount;

                    _accountService.SaveAccount(accountOrigin);
                    _accountService.SaveAccount(accountTarget);

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
