using CardGameProj.Scripts;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;

namespace AiBot
{
    public class AiPlayer
    {
        public List<int> Deck { get; private set; }
        public List<CardDB.CardData> Hand { get; private set; }
        public SortedDictionary<CardRanges, int> TotalsByRows { get; private set; }
        public int Total { get; set; }
        public List<CardDB.CardData> OnBoard { get; private set; }       
        public bool IsPass { get; set; }
        public CardNations Nation { get; private set; }
        private GameController gControl;
        public int PlGamesMargin { get; set; }

        public AiPlayer(CardNations nation, GameController gControl)
        {
            this.gControl = gControl;
            Hand = new();            
            OnBoard = new();           
            Nation = nation;
            Deck = CardDB.GetAllCards.Where(x => x.Value.Nation == Nation && x.Value.Type != CardTypes.Leader)
                .Select(x => x.Key).ToList();
            NewRound();
        }

        public void PutCard(CardDB.CardData card)
        {
            if (Hand.Contains(card) is false)
            {
                throw new Exception();
            }

            if (card.Type == CardTypes.Special)
            {
                gControl.SpOnBoard.Add(card);
            }
            else
            {
                OnBoard.Add(card);
            }

            Hand.Remove(card);
        }

        private List<int> GetRandomCardsFromDeck(int count = -1)
        {
            if (count == -1)
            {
                count = Math.Min(Math.Min(GameController.MaxHandSize, Deck.Count),
                GameController.MaxHandSize - Hand.Count);
            }

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

        private void TakeCardsFromDeck(List<int> cards)
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
                Hand.Add(CardDB.GetCardInfo(item));
            }
        }

        public void InitTotalsByRows()
        {
            TotalsByRows = new();
            TotalsByRows.Add(CardRanges.Row1, 0);
            TotalsByRows.Add(CardRanges.Row2, 0);
            TotalsByRows.Add(CardRanges.Row3, 0);
        }

        public void NewRound()
        {
            InitTotalsByRows();
            OnBoard.Clear();
            gControl.SpOnBoard.Clear();
            Total = 0;

            var cardList = GetRandomCardsFromDeck();
            TakeCardsFromDeck(cardList);
        }

        public void UpdateTotals()
        {
            InitTotalsByRows();

            foreach (CardRanges range in Enum.GetValues(typeof(CardRanges)))
            {
                List<CardDB.CardData> synergyCardsOnRow = new();

                foreach (var card in OnBoard.Where(x => x.Range == range))
                {
                    if (card.Synergy)
                        synergyCardsOnRow.Add(card);
                    TotalsByRows[card.Range] += card.Strength;
                }

                if (synergyCardsOnRow.Count > 1)
                {
                    foreach (var card in synergyCardsOnRow)
                    {
                        TotalsByRows[card.Range] += card.Strength;
                    }
                }
            }

            foreach (var spCard in gControl.SpOnBoard)
            {
                TotalsByRows[spCard.Range] = OnBoard.Where(x => x.Range == spCard.Range).Count();
            }

            Total = TotalsByRows.Values.Sum();
        }
    }
}
