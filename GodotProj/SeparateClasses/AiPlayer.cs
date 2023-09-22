using CardGameProj.Scripts;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardGameProj.SeparateClasses
{
    public class AiPlayer
    {
        private Dictionary<int, int> QtoGTranslator;
        private Dictionary<int, int> GtoQTranslator;
        public List<int> Deck { get; private set; }
        public List<CardDataBase.CardData> Hand { get; private set; }
        public CardNations Nation { get; private set; }
        public Dictionary<int, double[]> QTable { get; private set; }

        public AiPlayer(CardNations nation)
        {
            Nation = nation;
            ReadQTableFromFile();
            InitTranslators();
            Hand = new();          
            Deck = CardDataBase.GetAllCards.Where(x => x.Value.nation == Nation && x.Value.type != CardTypes.Leader)
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
            foreach (var card in CardDataBase.GetAllCards.Where(x => x.Value.nation == Nation).ToArray())
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
                    int number = Random.Shared.Next(0, Deck.Count);
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
                Hand.Add(CardDataBase.GetCardInfo(item));
            }

            GameFieldController.Instance.UpdateDeckSize(Deck.Count.ToString());
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
                var cardInfo = CardDataBase.GetCardInfo(action);
                switch (cardInfo.type)
                {
                    case CardTypes.Group1:
                    case CardTypes.Group2:
                    case CardTypes.Group3:
                        Hand.Remove(cardInfo);
                        break;
                    case CardTypes.Leader:
                        break;
                    case CardTypes.Special:
                        break;
                    default:
                        break;
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
                int randAction = validActions[Random.Shared.Next(validActions.Count)];
                return QtoGTranslator[randAction];
            }

            double maxQValue = double.MinValue;
            int bestAction = int.MinValue;
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

            bestAction = maxValIndxs[Random.Shared.Next(maxValIndxs.Count())];
            return QtoGTranslator[bestAction];
        }

        private List<int> GetValidActions()
        {
            var actions = Hand.Select(x => x.id).ToList();
            actions.Add((int)ActionTypes.Pass);

            return actions.Select(x => GtoQTranslator[x]).ToList();
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
            string path = $"res://Data/QTableData/{Nation.ToString().ToLower()}_QTable.txt";
            string data = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read).GetAsText();

            foreach (string line in data.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                string[] temp = line.Split(':', StringSplitOptions.RemoveEmptyEntries);
                int state = int.Parse(temp[0]);
                double[] rewards = temp[1].Split('/', StringSplitOptions.RemoveEmptyEntries).Select(x => double.Parse(x)).ToArray();
                QTable.Add(state, rewards);
            }
        }
    }
}