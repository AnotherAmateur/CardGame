using AiBot;
using CardGameProj.Scripts;
using System.Runtime.ConstrainedExecution;

public class BotLearning
{
    private const double InitMaxEps = 1;
    public (string, double) WinState { get; set; }
    public (string, double) LossState { get; set; }
    public (string, double) MatchState { get; set; }

    public int MatchesCount { get; set; }
    public double LearningRate { get; set; }
    public double DiscountFactor { get; set; }
    public bool RandInit { get; set; }
    public double InitValue { get; set; }
    public bool StepRewards { get; set; }
    public CardNations firstBotNation { get; set; }
    public CardNations secondBotNation { get; set; }

    private Dictionary<int, int> n1_QtoGTranslator;
    private Dictionary<int, int> n2_QtoGTranslator;

    QLearning? firstBot;
    QLearning? secondBot;

    public BotLearning()
    {
        this.n1_QtoGTranslator = new();
        this.n2_QtoGTranslator = new();
    }

    public void Start()
    {
        n1_QtoGTranslator.Add(0, (int)ActionTypes.Pass);
        n2_QtoGTranslator.Add(0, (int)ActionTypes.Pass);

        Dictionary<int, int> n1_GtoQTranslator = new();
        Dictionary<int, int> n2_GtoQTranslator = new();
        n1_GtoQTranslator.Add((int)ActionTypes.Pass, 0);
        n2_GtoQTranslator.Add((int)ActionTypes.Pass, 0);

        int index = 1;
        foreach (var card in CardDB.GetAllCards.Where(x => x.Value.Nation == firstBotNation && x.Value.Type != CardTypes.Leader))
        {
            n1_QtoGTranslator.Add(index, card.Key);
            n1_GtoQTranslator.Add(card.Key, index);
            index++;
        }

        index = 1;
        foreach (var card in CardDB.GetAllCards.Where(x => x.Value.Nation == secondBotNation))
        {
            n2_QtoGTranslator.Add(index, card.Key);
            n2_GtoQTranslator.Add(card.Key, index);
            index++;
        }

        firstBot = new(n1_QtoGTranslator.Count, LearningRate, DiscountFactor, n1_QtoGTranslator, n1_GtoQTranslator, RandInit, InitValue);
        secondBot = new(n2_QtoGTranslator.Count, LearningRate, DiscountFactor, n2_QtoGTranslator, n2_GtoQTranslator, RandInit, InitValue);


        for (int i = 0; i < MatchesCount; i++)
        {
            GameController gameController = new(firstBotNation, secondBotNation);
            List<MovesLog> movesLog = new();

            Dictionary<QLearning, AiPlayer> botPlayerRel = new()
                { { firstBot, gameController.FirstPl },
                  { secondBot, gameController.SecondPl } };

            double curEps = Math.Max(InitMaxEps - (i / MatchesCount), 0.1);

            do
            {
                AiPlayer curPlayer = gameController.CurrentPlayer;
                StatesLog stateBefore = gameController.GetCurState(curPlayer);
                int curState = GetStateHash(gameController.GetCurStateString());

                QLearning currentBot = botPlayerRel.Where(x => x.Value == curPlayer).Select(x => x.Key).First();
                int action = currentBot.ChooseAction(curState, curEps, gameController.GetValidActions());
                gameController.MakeMove(action);

                StatesLog stateAfter = gameController.GetCurState(curPlayer);
                movesLog.Add(new(curState, action, currentBot, GetStepReward(stateBefore, stateAfter, action)));

            } while (gameController.CurrentPlayer != null);

            QLearning? winner = null;

            if (gameController.Winner == botPlayerRel[firstBot])
            {
                winner = firstBot;
            }
            else if (gameController.Winner == botPlayerRel[secondBot])
            {
                winner = secondBot;
            }

            UpdateQValuesOffline(movesLog, winner);
        }

        WriteToFile(0);

        firstBot = null;
        secondBot = null;
    }

    void UpdateQValuesOffline(List<MovesLog> botMovesLog, QLearning? winner)
    {
        int nextStateBot1;
        int nextStateBot2;

        double bot1FinalRew;
        double bot2FinalRew;

        switch (winner)
        {
            case var value when value == firstBot:
                nextStateBot1 = GetStateHash(WinState.Item1);
                nextStateBot2 = GetStateHash(LossState.Item1);
                bot1FinalRew = WinState.Item2;
                bot2FinalRew = LossState.Item2;
                break;
            case var value when value == secondBot:
                nextStateBot2 = GetStateHash(WinState.Item1);
                nextStateBot1 = GetStateHash(LossState.Item1);
                bot2FinalRew = WinState.Item2;
                bot1FinalRew = LossState.Item2;
                break;
            case var value when value == null:
                nextStateBot1 = GetStateHash(MatchState.Item1);
                nextStateBot2 = GetStateHash(MatchState.Item1);
                bot1FinalRew = MatchState.Item2;
                bot2FinalRew = MatchState.Item2;
                break;
            default:
                throw new Exception();
        }

        for (int i = botMovesLog.Count - 1; i >= 0; i--)
        {
            switch (botMovesLog[i].Bot)
            {
                case var value when value == firstBot:
                    firstBot.UpdateQValue(botMovesLog[i].State, botMovesLog[i].Action, nextStateBot1, bot1FinalRew + botMovesLog[i].StepReward);
                    bot1FinalRew = 0;
                    nextStateBot1 = botMovesLog[i].State;
                    break;
                case var value when value == secondBot:
                    secondBot.UpdateQValue(botMovesLog[i].State, botMovesLog[i].Action, nextStateBot2, bot2FinalRew + botMovesLog[i].StepReward);
                    bot2FinalRew = 0;
                    nextStateBot2 = botMovesLog[i].State;
                    break;
                default:
                    throw new Exception();
            }
        }
    }

    double GetStepReward(StatesLog stateBefore, StatesLog stateAfter, int action)
    {
        if (StepRewards)
        {
            if ((stateBefore.Round == 3 || stateBefore.SelfGamesRslt < 0) 
                && stateAfter.SelfTotal < stateAfter.EnemyTotal
                && stateBefore.EnemePassed is false)
            {
                if (action == (int)ActionTypes.Pass && stateBefore.HandCount > 0)
                {
                    return -100;
                }
            }

            double totalDifRew = (stateAfter.SelfTotal - stateAfter.EnemyTotal) >
                        (stateBefore.SelfTotal - stateBefore.EnemyTotal) ? 10 : -10;
            double roundRew = stateAfter.Round * 10;
            double winsRew = stateAfter.SelfGamesRslt * 10;

            if (stateAfter.SelfGamesRslt - stateBefore.SelfGamesRslt > 0)
                return totalDifRew * roundRew * winsRew + 50;

            return totalDifRew * roundRew * winsRew;
        }

        return 0;
    }

    void WriteToFile(int index)
    {
        string directory = $"logs_{DateTime.Now.Ticks}";
        System.IO.Directory.CreateDirectory(directory);

        Console.WriteLine($"Writing to directory {directory}...");

        string firstBot_TablePath = $"{directory}/{firstBotNation}_QTable.txt";
        string secondBot_TablePath = $"{directory}/{secondBotNation}_QTable.txt";
        string paramsPath = $"{directory}/Params.txt";

        var task = Task.Factory.StartNew(() =>
        {
            using (var sw = new StreamWriter(firstBot_TablePath))
            {
                foreach (var row in firstBot.QTable)
                {
                    if ((new HashSet<double>(row.Value).Count) == 1)
                        continue;

                    var uniqueValues = row.Value.Distinct().OrderBy(x => x).ToList();
                    var valueToIntegerMapping = uniqueValues.Select((value, index) => new { dValue = value, Integer = index }).ToDictionary(x => x.dValue, x => x.Integer);
                    int[] transformedRews = row.Value.Select(value => valueToIntegerMapping[value]).ToArray();

                    sw.Write($"{row.Key}:{string.Join('/', transformedRews.Select(x => (x != 0) ? x.ToString() : "")) + '\n'}");
                }
            }
        });

        using (var sw = new StreamWriter(secondBot_TablePath))
        {
            foreach (var row in secondBot.QTable)
            {
                if ((new HashSet<double>(row.Value).Count) == 1)
                    continue;

                var uniqueValues = row.Value.Distinct().OrderBy(x => x).ToList();
                var valueToIntegerMapping = uniqueValues.Select((value, index) => new { dValue = value, Integer = index }).ToDictionary(x => x.dValue, x => x.Integer);
                int[] transformedRews = row.Value.Select(value => valueToIntegerMapping[value]).ToArray();

                sw.Write($"{row.Key}:{string.Join('/', transformedRews.Select(x => (x != 0) ? x.ToString() : "")) + '\n'}");
            }
        }

        using (var sw = new StreamWriter(paramsPath))
        {
            var properties = typeof(BotLearning).GetProperties();

            foreach (var property in properties)
            {
                string propertyName = property.Name;
                object propertyValue = property.GetValue(this);

                sw.WriteLine($"{propertyName}:{propertyValue}");
            }
        }

        Console.WriteLine($"Writing to directory {directory} done");
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

    struct MovesLog
    {
        public int State { get; set; }
        public int Action { get; set; }
        public QLearning Bot { get; set; }
        public double StepReward { get; set; }

        public MovesLog(int state, int action, QLearning bot, double stepReward)
        {
            State = state;
            Action = action;
            StepReward = stepReward;
            Bot = bot ?? throw new ArgumentNullException(nameof(bot));
        }
    }
}