using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using HomeBankingMindHub.DTOs;
using System.Transactions;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Services.Implements;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private ILoanService _loanService;
        private IClientService _clientService;
        private IClientLoanService _clientLoanService;
        private IAccountService _accountService;
        private ITransactionService _transactionService;

        public LoansController(ILoanService loanService, IClientService clientService, IClientLoanService clientLoanService, IAccountService accountService, ITransactionService transactionService)
        {
            _loanService = loanService;
            _clientService = clientService;
            _clientLoanService = clientLoanService;
            _accountService = accountService;
            _transactionService = transactionService;
        }

        [HttpGet]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Get()
        {
            try
            {
                var loansDTO = _loanService.GetAllLoansDTO();

                return Ok(loansDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult transfers([FromBody] LoanApplicationDTO loan)
        {
            using(var scope = new TransactionScope())
            {
                try
                {
                    // Valido datos
                    var loanValidate = _loanService.GetLoanById(loan.LoanId);
                    if (loanValidate == null)
                    {
                        return StatusCode(403, "Préstamo inválido");
                    }

                    if (loan.Amount <= 0 || loan.Amount > loanValidate.MaxAmount)
                    {
                        return StatusCode(403, "Monto de dinero inválido");
                    }

                    if (String.IsNullOrEmpty(loan.Payments))
                    {
                        return StatusCode(403, "Cuota inválida");
                    }

                    // Verifico que la cuota elegida pertenezca al préstamo seleccionado
                    var allowedPayments = loanValidate.Payments.Split(',').Select(p => p.Trim());
                    if (!allowedPayments.Contains(loan.Payments))
                    {
                        return StatusCode(403, "Cuota seleccionada no permitida en este préstamo");
                    }

                    if (!_accountService.ExistsByAccountNumber(loan.ToAccountNumber))
                    {
                        return StatusCode(403, "Cuenta de destino inválida");
                    }

                    // Obtengo email de cliente
                    string clientEmail = User.FindFirst("Client") == null ? User.FindFirst("Admin").Value : User.FindFirst("Client").Value;
                    if (string.IsNullOrEmpty(clientEmail))
                    {
                        return Forbid();
                    }

                    // Obtengo objeto client
                    Client client = _clientService.GetClientByEmail(clientEmail);

                    // Verifico si la cuenta de destino pertenece al cliente autenticado
                    var accountTarget = _accountService.GetAccountByNumber(loan.ToAccountNumber);
                    if (accountTarget == null || accountTarget.ClientId != client.Id)
                    {
                        return StatusCode(403, "La cuenta de destino no pertenece al cliente autenticado.");
                    }

                    // Creo loan con 20%
                    ClientLoan newClientLoan = _clientLoanService.CreateClientLoan(loan.Amount, loan.Payments, client.Id, loan.LoanId);

                    _clientLoanService.SaveClientLoan(newClientLoan);

                    string description = loanValidate.Name + " " + "Loan approved";
                    Transaction newTransactionCredit = _transactionService.CreateTransaction(TransactionType.CREDIT, loan.Amount, description, accountTarget.Id, loan.ToAccountNumber);

                    _transactionService.SaveTransaction(newTransactionCredit);

                    accountTarget.Balance += loan.Amount;
                    _accountService.SaveAccount(accountTarget);

                    scope.Complete();
                    return Created("", newClientLoan);

                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
        }

    }
}
