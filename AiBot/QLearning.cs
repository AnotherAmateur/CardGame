using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AiBot
{
    public class QLearning
    {
        public Dictionary<int, double[]> QTable { get; private set; }
        private double learningRate;
        private double discountFactor;
        private Random random;
        private int actionsCount;
        private Dictionary<int, int> QtoGTranslator;
        private Dictionary<int, int> GtoQTranslator;


        public QLearning(int actionsCount, double learningRate, double discountFactor, 
            Dictionary<int, int> QtoGTranslator, Dictionary<int, int> GtoQTranslator)
        {
            this.learningRate = learningRate;
            this.discountFactor = discountFactor;
            this.actionsCount = actionsCount;
            this.QtoGTranslator = QtoGTranslator;
            this.GtoQTranslator = GtoQTranslator;

            QTable = new();
            random = new Random();
        }

        public int ChooseAction(int state, double eps, List<int> validActions)
        {
            validActions = validActions.Select(x => GtoQTranslator[x]).ToList();

            if (QTable.ContainsKey(state) is false)
            {
                QTable.Add(state, new double[actionsCount]);
            }

            if (random.NextDouble() < eps)
            {
                int randAction = validActions[random.Next(validActions.Count)];
                return QtoGTranslator[randAction];
            }
            else
            {
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

                bestAction = maxValIndxs[random.Next(maxValIndxs.Count())];
                return QtoGTranslator[bestAction];
            }
        }

        public void UpdateQValue(int currentState, int action, int nextState, double reward)
        {
            action = GtoQTranslator[action];

            if (QTable.ContainsKey(nextState) is false)
            {
                QTable.Add(nextState, new double[1]);
            }

            QTable[currentState][action] = QTable[currentState][action] + learningRate *
                (reward + discountFactor * MaxQValue(nextState) - QTable[currentState][action]);
        }

        private double MaxQValue(int state)
        {
            double maxQValue = double.MinValue;

            for (int action = 0; action < QTable[state].Length; action++)
            {
                if (QTable[state][action] > maxQValue)
                {
                    maxQValue = QTable[state][action];
                }
            }

            return maxQValue;
        }
    }
}
