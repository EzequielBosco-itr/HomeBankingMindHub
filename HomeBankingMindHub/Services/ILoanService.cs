using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ILoanService
    {
        IEnumerable<LoanDTO> GetAllLoansDTO();
        Loan GetLoanById(long id);
    }
}
