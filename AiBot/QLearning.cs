using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiBot
{
    public class QLearning
    {
        public Dictionary<int, double[]> QTable { get; private set; }
        private double learningRate;
        private double discountFactor;
        private Random random;
        private int actionsCount;
        private Dictionary<int, int> translator;

        public QLearning(int actionsCount, double learningRate, double discountFactor, Dictionary<int, int> translator)
        {
            this.learningRate = learningRate;
            this.discountFactor = discountFactor;
            this.actionsCount = actionsCount;
            this.translator = translator;

            QTable = new();
            random = new Random();            
        }

        public int ChooseAction(int state, double eps)
        {

            if (QTable.ContainsKey(state) is false)
            {
                QTable.Add(state, new double[actionsCount]);
            }

            if (random.NextDouble() < eps)
            {
                return random.Next(actionsCount);
            }
            else
            {
                double maxQValue = double.MinValue;
                int bestAction = 0;

                for (int action = 0; action < actionsCount; action++)
                {
                    if (QTable[state][action] > maxQValue)
                    {
                        maxQValue = QTable[state][action];
                        bestAction = action;
                    }
                }

                return translator[bestAction];
            }
        }

        public void UpdateQValue(int currentState, int action, int nextState, double reward)
        {
            QTable[currentState][action] = QTable[currentState][action] + learningRate *
                (reward + discountFactor * MaxQValue(nextState) - QTable[currentState][action]);
        }
        
        private double MaxQValue(int state)
        {
            double maxQValue = double.MinValue;

            for (int action = 0; action < actionsCount; action++)
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
