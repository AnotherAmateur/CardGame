namespace AiBot
{
    public class QLearning
    {
        public Dictionary<int, float[]> QTable { get; private set; }
        private float learningRate;
        private float discountFactor;
        private Random random;
        private int actionsCount;
        private bool randInit;
        private float initValue;
        public Dictionary<int, int> QtoGTranslator { get; private set; }
        public Dictionary<int, int> GtoQTranslator { get; private set; }


        public QLearning(int actionsCount, float learningRate, float discountFactor,
            Dictionary<int, int> QtoGTranslator, Dictionary<int, int> GtoQTranslator, bool randInit, float initValue)
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

        public QLearning(Dictionary<int, float[]> qTable, int actionsCount, Dictionary<int, int> qtoGTranslator, Dictionary<int, int> gtoQTranslator)
        {
            QTable = qTable;
            this.actionsCount = actionsCount;
            QtoGTranslator = qtoGTranslator;
            GtoQTranslator = gtoQTranslator;
            random = new Random();
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
                float maxQValue = float.MinValue;
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

        public int GetBestAction(int state, double eps, List<int> validActions)
        {
            validActions = validActions.Select(x => GtoQTranslator[x]).ToList();

            if (QTable.ContainsKey(state) is false)
            {
                int randAction = validActions[random.Next(validActions.Count)];
                return QtoGTranslator[randAction];
            }
            else
            {
                float maxQValue = float.MinValue;
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

        public void UpdateQValue(int currentState, int action, int nextState, float reward)
        {
            action = GtoQTranslator[action];

            if (QTable.ContainsKey(nextState) is false)
            {
                QTable.Add(nextState, new float[1]);
            }

            QTable[currentState][action] = QTable[currentState][action] + learningRate *
                (reward + discountFactor * MaxQValue(nextState) - QTable[currentState][action]);
        }

        private float MaxQValue(int state)
        {
            float maxQValue = float.MinValue;

            for (int action = 0; action < QTable[state].Length; action++)
            {
                if (QTable[state][action] > maxQValue)
                {
                    maxQValue = QTable[state][action];
                }
            }

            return maxQValue;
        }

        private float[] InitActRewards()
        {
            float[] actRewArray = new float[actionsCount];

            if (randInit)
            {
                for (int i = 0; i < actRewArray.Length; i++)
                {
                    actRewArray[i] = random.NextSingle();
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
