using CardGameProj.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
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
            QTable = ReadQTableFromFile();
            Hand = new();
            Nation = nation;
            Deck = CardDataBase.GetAllCards.Where(x => x.Value.nation == Nation && x.Value.type != CardTypes.Leader)
                .Select(x => x.Key).ToList();
            var cardList = GetRandomCardsFromDeck();
            TakeCardsFromDeck(cardList);
        }

        public void PutCard(CardDataBase.CardData card)
        {
            if (Hand.Contains(card) is false)
            {
                throw new Exception();
            }

            Hand.Remove(card);
        }

        private List<int> GetRandomCardsFromDeck(int count = -1)
        {
            if (count == -1)
            {
                count = Math.Min(Math.Min(Player.MaxHandSize, Deck.Count),
                Player.MaxHandSize - Hand.Count);
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
                Hand.Add(CardDataBase.GetCardInfo(item));
            }
        }

        public void NewRound()
        {
            var cardList = GetRandomCardsFromDeck();
            TakeCardsFromDeck(cardList);
        }

        public int ChooseAction(string rawState)
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

        public List<int> GetValidActions()
        {
            var actions = Hand.Select(x => x.id).ToList();
            actions.Add((int)ActionTypes.Pass);

            return actions;
        }

        int GetStateHash(string input)
        {
            int hash = 17;
            foreach (char c in input)
            {
                hash = hash * 31 + c;
            }

            return hash;
        }

        Dictionary<int, double[]> ReadQTableFromFile()
        {
            Dictionary<int, double[]> qTable = new();
            string path = $"{Nation.ToString().ToLower()}.QTable";

            using (var sr = new StreamReader(path))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] temp = line.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    int state = int.Parse(temp[0]);
                    double[] rewards = temp[1].Split('/', StringSplitOptions.RemoveEmptyEntries).Select(x => double.Parse(x)).ToArray();
                    qTable.Add(state, rewards);
                }
            }

            return qTable;
        }
    }
}
