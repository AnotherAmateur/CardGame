using CardGameProj.Scripts;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;

namespace AiBot
{
    public class AiPlayer
    {
        public List<int> Deck { get; private set; }
        public List<CardDataBase.CardData> Hand { get; private set; }
        public Dictionary<CardTypes, int> TotalsByRows { get; private set; }
        public int Total { get; set; }
        public List<CardDataBase.CardData> OnBoard { get; private set; }
        public List<CardDataBase.CardData> SpOnBoard { get; private set; }
        public bool IsPassed { get; set; }
        public CardNations Nation { get; private set; }


        public AiPlayer(CardNations nation)
        {
            Nation = nation;
            Deck = CardDataBase.GetAllCards.Where(x => x.Value.nation == Nation).Select(x => x.Key).ToList();
            var cardList = GetRandomCardsFromDeck(Math.Min(GameController.MaxHandSize, Deck.Count));
            TakeCardsFromDeck(cardList);
        }


        public void PutCard(CardDataBase.CardData card)
        {
            if (Hand.Contains(card) is false)
            {
                throw new Exception();
            }

            if (card.type == CardTypes.Special)
            {
                OnBoard.Add(card);
            }
            else
            {
                OnBoard.Add(card);
            }

            Hand.Remove(card);
        }

        public List<int> GetRandomCardsFromDeck(int count)
        {
            List<int> result = new();
            if (count > 0)
            {
                if (count > Deck.Count)
                {
                    throw new Exception();
                }

                Random random = new();
                HashSet<int> uniqueNumbers = new();
                while (uniqueNumbers.Count < count)
                {
                    int number = random.Next(0, Deck.Count);
                    uniqueNumbers.Add(number);
                }

                foreach (int index in uniqueNumbers)
                {
                    result.Add(Deck[index]);
                }

                return result;
            }

            return result;
        }

        public void TakeCardsFromDeck(List<int> cards)
        {
            if (cards.Count == 0)
            {
                return;
            }

            var remainingСards = Deck.Except(cards).ToList<int>();

            if (Deck.Count - remainingСards.Count != cards.Count)
            {
                throw new Exception();
            }

            Deck = remainingСards;
            foreach (var item in cards)
            {
                Hand.Add(CardDataBase.GetCardInfo(item));
            }
        }

      
    }
}
