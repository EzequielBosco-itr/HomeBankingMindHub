using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IClientLoanService
    {
        ClientLoan CreateClientLoan(double amount, string payments, long clientId, long loanId);
        void SaveClientLoan(ClientLoan clientLoan);
    }
}
