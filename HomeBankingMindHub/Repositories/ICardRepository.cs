using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public interface ICardRepository
    {

        IEnumerable<Card> GetCardsByEmail(string clientEmail);

        int GetCardsCountByClient(long clientId);

        void Save(Card card);

        bool ExistsByCardNumber(string cardNumber);

        bool ExistsByTypeAndColor(long clientId, string type, string color);
    }
}
