namespace AiBot
{
    public class QLPlay
    {
        public Dictionary<int, short[]> QTable { get; private set; }
        private float learningRate;
        private float discountFactor;
        private Random random;
        private int actionsCount;
        private bool randInit;
        private float initValue;
        public Dictionary<int, int> QtoGTranslator { get; private set; }
        public Dictionary<int, int> GtoQTranslator { get; private set; }

        public QLPlay(Dictionary<int, short[]> qTable, int actionsCount, Dictionary<int, int> qtoGTranslator, Dictionary<int, int> gtoQTranslator)
        {
            QTable = qTable;
            this.actionsCount = actionsCount;
            QtoGTranslator = qtoGTranslator;
            GtoQTranslator = gtoQTranslator;
            random = new Random();
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
    }
}
