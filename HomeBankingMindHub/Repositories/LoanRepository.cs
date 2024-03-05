using HomeBankingMindHub.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingMindHub.Repositories
{
    public class LoanRepository : RepositoryBase<Loan>, ILoanRepository
    {
        public LoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Loan> GetAll()
        {
            return FindAll();
        }

        public Loan FindById(long id) 
        {
            return FindByCondition(loan => loan.Id == id)
                .FirstOrDefault();
        }

        public void Save(Loan loan)
        {
            Create(loan);
            SaveChanges();
        }
    }
}
