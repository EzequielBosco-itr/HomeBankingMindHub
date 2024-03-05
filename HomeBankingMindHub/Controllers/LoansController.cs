using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using HomeBankingMindHub.DTOs;
using System.Transactions;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ILoanRepository _loanRepository;
        private IClientLoanRepository _clientLoanRepository;
        private ITransactionRepository _transactionRepository;

        public LoansController(IClientRepository clientRepository, IAccountRepository accountRepository, ILoanRepository loanRepository, IClientLoanRepository clientLoanRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Get()
        {
            try
            {
                var loans = _loanRepository.GetAll();

                var loansDTO = new List<LoanDTO>();

                foreach (Loan loan in loans)
                {
                    var newLoanDTO = new LoanDTO
                    {
                        Id = loan.Id,
                        Name = loan.Name,
                        MaxAmount = loan.MaxAmount,
                        Payments = loan.Payments
                    };

                    loansDTO.Add(newLoanDTO);
                }

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
                    var loanValidate = _loanRepository.FindById(loan.LoanId);
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

                    if (!_accountRepository.ExistsByAccountNumber(loan.ToAccountNumber))
                    {
                        return StatusCode(403, "Cuenta de destino inválida");
                    }

                    // Obtengo email de cliente
                    string clientEmail = User.FindFirst("Client")?.Value;

                    if (string.IsNullOrEmpty(clientEmail))
                    {
                        return Forbid();
                    }

                    // Obtengo objeto client
                    Client client = _clientRepository.FindByEmail(clientEmail);

                    // Verifico si la cuenta de destino pertenece al cliente autenticado
                    var accountTarget = _accountRepository.FindByNumber(loan.ToAccountNumber);
                    if (accountTarget == null || accountTarget.ClientId != client.Id)
                    {
                        return StatusCode(403, "La cuenta de destino no pertenece al cliente autenticado.");
                    }

                    // Creo loan con 20%
                    ClientLoan newClientLoan = new ClientLoan
                    {
                        Amount = loan.Amount * 0.2,
                        Payments = loan.Payments,
                        ClientId = client.Id,
                        LoanId = loan.LoanId
                    };

                    _clientLoanRepository.Save(newClientLoan);

                    Transaction newTransactionCredit = new Transaction
                    {
                        Type = TransactionType.CREDIT,
                        Amount = loan.Amount,
                        Description = loanValidate.Name + " " + "Loan approved",
                        AccountId = accountTarget.Id,
                        Date = DateTime.Now
                    };

                    _transactionRepository.Save(newTransactionCredit);

                    accountTarget.Balance += loan.Amount;
                    _accountRepository.Save(accountTarget);

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
