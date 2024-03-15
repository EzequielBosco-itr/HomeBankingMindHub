using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Implements
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;

        public LoanService(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public IEnumerable<LoanDTO> GetAllLoansDTO()
        {
            var loans = _loanRepository.GetAll();
            var loansDTO = new List<LoanDTO>();

            foreach (Loan loan in loans)
            {
                var newLoanDTO = new LoanDTO(loan);

                loansDTO.Add(newLoanDTO);
            }
            return loansDTO;
        }

        public Loan GetLoanById(long loanId)
        {
            Loan loan = _loanRepository.FindById(loanId);
            if (loan == null)
            {
                throw new Exception("Préstamo inválido");
            }
            return loan;
        }
    }
}
