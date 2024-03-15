using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ICardService
    {
        IEnumerable<CardDTO> GetCardsByEmail(string email);
        int GetCardsCountByClient(long clientId);
        bool ExistsByTypeAndColor(long clientId, string type, string color);
        string CreateCardNumber();
        int CreateCardCvv();
        Card CreateCard(long clientId, string firstName, string lastName, string type, string color, string cardNumber, int cardCvv);
        void SaveCard(Card card);
    }
}
