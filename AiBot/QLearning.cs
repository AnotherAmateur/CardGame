namespace AiBot
{
    public class QLearning
    {
        public Dictionary<int, double[]> QTable { get; private set; }
        private double learningRate;
        private double discountFactor;
        private Random random;
        private int actionsCount;
        private bool randInit;
        private double initValue;
        public Dictionary<int, int> QtoGTranslator { get; private set; }
        public Dictionary<int, int> GtoQTranslator { get; private set; }


        public QLearning(int actionsCount, double learningRate, double discountFactor,
            Dictionary<int, int> QtoGTranslator, Dictionary<int, int> GtoQTranslator, bool randInit, double initValue)
        {
            this.learningRate = learningRate;
            this.discountFactor = discountFactor;
            this.actionsCount = actionsCount;
            this.QtoGTranslator = QtoGTranslator;
            this.GtoQTranslator = GtoQTranslator;
            this.randInit = randInit;
            this.initValue = initValue;

            QTable = new();
            random = new Random();
        }

        public QLearning(Dictionary<int, double[]> qTable, int actionsCount, Dictionary<int, int> qtoGTranslator, Dictionary<int, int> gtoQTranslator)
        {
            QTable = qTable;
            this.actionsCount = actionsCount;
            QtoGTranslator = qtoGTranslator;
            GtoQTranslator = gtoQTranslator;
        }

        public int ChooseAction(int state, double eps, List<int> validActions)
        {
            validActions = validActions.Select(x => GtoQTranslator[x]).ToList();

            if (QTable.ContainsKey(state) is false)
            {
                QTable.Add(state, InitActRewards());
            }

            if (random.NextDouble() < eps)
            {
                int randAction = validActions[random.Next(validActions.Count)];
                return QtoGTranslator[randAction];
            }
            else
            {
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

        public void SetQTable(Dictionary<int, double[]> qTable)
        {
            QTable = qTable;
        }

        private double[] InitActRewards()
        {
            double[] actRewArray = new double[actionsCount];

            if (randInit)
            {
                for (int i = 0; i < actRewArray.Length; i++)
                {
                    actRewArray[i] = random.NextDouble();
                }
            }
            else
            {
                for (int i = 0; i < actRewArray.Length; i++)
                {
                    actRewArray[i] = initValue;
                }
            }

            return actRewArray;
        }
    }
}
