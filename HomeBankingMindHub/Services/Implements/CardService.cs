using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Utils;

namespace HomeBankingMindHub.Services.Implements
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;

        public CardService(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public IEnumerable<CardDTO> GetCardsByEmail(string email)
        {
            var cards = _cardRepository.GetCardsByEmail(email);
            var cardsDTO = new List<CardDTO>();

            foreach (Card card in cards)
            {
                var newCardDTO = new CardDTO(card);

                cardsDTO.Add(newCardDTO);
            }

            return cardsDTO;
        }

            public int GetCardsCountByClient(long clientId)
        {
            int clientCardsCount = _cardRepository.GetCardsCountByClient(clientId);
            if (clientCardsCount >= 6)
            {
                throw new Exception("Error. El cliente ya tiene el máximo de tarjetas registradas (6).");
            }
            return clientCardsCount;
        }

        public bool ExistsByTypeAndColor(long clientId, string type, string color)
        {
            var existsCard = _cardRepository.ExistsByTypeAndColor(clientId, type, color);
            if (existsCard)
            {
                throw new Exception("Error. El cliente ya tiene una tarjeta con el mismo tipo y color.");
            };

            return existsCard;
        }

        public string CreateCardNumber()
        {
            int randomCardNumber = RandomNumberUtils.GenerateRandomNumber(1000, 9999);
            int randomCardNumber1 = RandomNumberUtils.GenerateRandomNumber(1000, 9999);
            int randomCardNumber2 = RandomNumberUtils.GenerateRandomNumber(1000, 9999);
            int randomCardNumber3 = RandomNumberUtils.GenerateRandomNumber(1000, 9999);

            string cardNumber = $"{randomCardNumber}{randomCardNumber1}{randomCardNumber2}{randomCardNumber3}";

            // Verifico si el número de tarjeta es único
            while (_cardRepository.ExistsByCardNumber(cardNumber))
            {
                // Si ya existe, genero un nuevo número y vuelvo a verificar
                randomCardNumber = RandomNumberUtils.GenerateRandomNumber(1000, 9999);
                randomCardNumber1 = RandomNumberUtils.GenerateRandomNumber(1000, 9999);
                randomCardNumber2 = RandomNumberUtils.GenerateRandomNumber(1000, 9999);
                randomCardNumber3 = RandomNumberUtils.GenerateRandomNumber(1000, 9999);
                cardNumber = $"{randomCardNumber}{randomCardNumber1}{randomCardNumber2}{randomCardNumber3}";
            }

            // Agrego formato de tarjeta
            string cardNumberFormat = $"{randomCardNumber}-{randomCardNumber1}-{randomCardNumber2}-{randomCardNumber3}";

            return cardNumberFormat;
        }

        public int CreateCardCvv()
        {
            long randomCardCvv = RandomNumberUtils.GenerateRandomNumber(100, 999);

            int cardCvv = (int)randomCardCvv;

            return cardCvv;
        }

        public Card CreateCard(long clientId, string firstName, string lastName, string type, string color, string cardNumber, int cardCvv)
        {
            var newCard = new Card
            {
                ClientId = clientId,
                CardHolder = $"{firstName} {lastName}",
                Type = type,
                Color = color,
                Number = cardNumber,
                Cvv = cardCvv
            };
            return newCard;
        }

        public void SaveCard(Card card)
        {
            _cardRepository.Save(card);
        }
    }
}
