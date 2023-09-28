using CardGameProj.Scripts;
using Godot;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardGameProj.SeparateClasses
{
    public class AiPlayer
    {
        private Random random;
        private Dictionary<int, int> QtoGTranslator;
        private Dictionary<int, int> GtoQTranslator;
        public List<int> Deck { get; private set; }
        public List<CardDB.CardData> Hand { get; private set; }
        public CardNations Nation { get; private set; }
        public Dictionary<int, int[]> QTable { get; private set; }

        public AiPlayer(CardNations nation)
        {
            random = new();
            Nation = nation;
            ReadQTableFromFile();
            InitTranslators();
            Hand = new();
            Deck = CardDB.GetAllCards.Where(x => x.Value.Nation == Nation && x.Value.Type != CardTypes.Leader)
                .Select(x => x.Key).ToList();
            NewRound();
        }

        private void InitTranslators()
        {
            QtoGTranslator = new();
            GtoQTranslator = new();

            QtoGTranslator.Add(0, (int)ActionTypes.Pass);
            GtoQTranslator.Add((int)ActionTypes.Pass, 0);

            int index = 1;
            foreach (var card in CardDB.GetAllCards.Where(x => x.Value.Nation == Nation).ToArray())
            {
                QtoGTranslator.Add(index, card.Key);
                GtoQTranslator.Add(card.Key, index);
                index++;
            }
        }

        private List<int> GetRandomCardsFromDeck(int count = -1)
        {
            if (count == -1)
            {
                count = Math.Min(Deck.Count, Player.MaxHandSize - Hand.Count);
            }

            List<int> result = new();
            if (count > 0)
            {
                if (count > Deck.Count)
                {
                    throw new Exception();
                }

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

            GFieldController.Instance.UpdateDeckSize(Deck.Count.ToString());
        }

        private void NewRound()
        {
            var cardList = GetRandomCardsFromDeck();
            TakeCardsFromDeck(cardList);
        }

        public int ApplyAction(string rawState)
        {
            int action = GetBestAction(rawState);

            if (action == (int)ActionTypes.Pass)
            {
                NewRound();
            }
            else
            {
                var cardInfo = CardDB.GetCardInfo(action);

                if (cardInfo.Type == CardTypes.Leader)
                {
                    // todo leader card action
                }
                else
                {
                    Hand.Remove(cardInfo);
                }
            }

            return action;
        }

        private int GetBestAction(string rawState)
        {
            int state = GetStateHash(rawState);
            var validActions = GetValidActions();

            if (QTable.ContainsKey(state) is false)
            {
                OS.Alert("rand");
                int randAction = validActions[random.Next(validActions.Count)];
                return QtoGTranslator[randAction];
            }

            double maxQValue = double.MinValue;
            int bestAction;
            List<int> maxValIndxs = new();

            foreach (int action in validActions)
            {
                if (QTable[state][action] > maxQValue)
                {
                    maxQValue = QTable[state][action];
                    bestAction = action;
                    maxValIndxs.Clear();
                    maxValIndxs.Add(bestAction);
                }
                else if (QTable[state][action] == maxQValue)
                {
                    maxValIndxs.Add(action);
                }
            }

            bestAction = maxValIndxs[random.Next(maxValIndxs.Count)];
            return QtoGTranslator[bestAction];
        }

        private List<int> GetValidActions()
        {
            IEnumerable<CardDB.CardData> actions;

            if (Player.SpOnBoard.Count == GFieldController.MaxSpOnBoardCount)
            {
                actions = Hand.Where(x => x.Type != CardTypes.Special || x.Range == CardRanges.OutOfRange);
            }
            else
            {
                actions = Hand.Where(x => Player.SpOnBoard.Select(y => y.Range).Contains(x.Range) is false);
            }

            var result = actions.Select(x => GtoQTranslator[x.Id]).ToList();
            result.Add(GtoQTranslator[(int)ActionTypes.Pass]);

            return result;
        }

        private int GetStateHash(string input)
        {
            int hash = 17;
            foreach (char c in input)
            {
                hash = hash * 31 + c;
            }

            return hash;
        }

        private void ReadQTableFromFile()
        {
            QTable = new();
            string path = $"res://Data/QTableData/{Nation.ToString()}_QTable.txt";
            string data = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read).GetAsText();

            Parallel.ForEach(data.Split('\n', StringSplitOptions.RemoveEmptyEntries), (line) =>
            {
                string[] temp = line.Split(':', StringSplitOptions.RemoveEmptyEntries);
                int state = int.Parse(temp[0]);
                int[] rewards = (temp.Length == 1) ? new int[] { 0 } :
                    rewards = temp[1].Split('/').Select(x => (x != "") ? int.Parse(x) : 0).ToArray();
                lock (QTable)
                {
                    QTable.Add(state, rewards);
                }
            });
        }
    }
}
