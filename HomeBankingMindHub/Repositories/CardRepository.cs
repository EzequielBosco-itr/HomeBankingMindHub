using HomeBankingMindHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace HomeBankingMindHub.Repositories
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Card> GetCardsByEmail(string clientEmail)
        {
            return FindByCondition(card => card.Client.Email == clientEmail).ToList();
        }

        public int GetCardsCountByClient(long clientId)
        {
            return FindByCondition(card => card.Client.Id == clientId).Count();
        }

        public void Save(Card card)
        {
            Create(card);
            SaveChanges();
        }

        public bool ExistsByCardNumber(string cardNumber)
        {
            return FindByCondition(card => card.Number == cardNumber).Any();
        }

        public bool ExistsByTypeAndColor(long clientId, string type, string color)
        {
            return FindByCondition(card => card.ClientId == clientId && card.Type == type && card.Color == color).Any();
        }
    }
}
