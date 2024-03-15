using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Implements
{
    public class ClientLoanService : IClientLoanService
    {
        private readonly IClientLoanRepository _clientLoanRepository;

        public ClientLoanService(IClientLoanRepository clientLoanRepository)
        {
            _clientLoanRepository = clientLoanRepository;
        }

        public ClientLoan CreateClientLoan(double amount, string payments, long clientId, long loanId)
        {
            ClientLoan newClientLoan = new ClientLoan
            {
                Amount = amount * 1.2,
                Payments = payments,
                ClientId = clientId,
                LoanId = loanId
            };
            return newClientLoan;
        }
        public void SaveClientLoan(ClientLoan clientLoan)
        {
            _clientLoanRepository.Save(clientLoan);
        }
    }
}
