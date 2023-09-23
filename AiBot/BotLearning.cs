using AiBot;
using CardGameProj.Scripts;

public class BotLearning
{
    private const double InitMaxEps = 1;
    public (string, double) WinState { get; set; } = ("WIN", 100);
    public (string, double) LossState { get; set; } = ("LOSS", -100);
    public (string, double) MatchState { get; set; } = ("MATCH", 10);

    public int MatchesCount { get; set; } = (int)4e2;
    public double LearningRate { get; set; } = 0.9;
    public double DiscountFactor { get; set; } = 0.75;
    public bool EachStepReward { get; set; }
    public bool RandInit { get; set; }
    public double InitValue { get; set; }
    public CardNations Nation1 { get; set; }
    public CardNations Nation2 { get; set; }

    private Dictionary<int, int> n1_QtoGTranslator;
    private Dictionary<int, int> n2_QtoGTranslator;

    QLearning bot1;
    QLearning botTarget;

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
        foreach (var card in CardDataBase.GetAllCards.Where(x => x.Value.nation == Nation1).ToArray())
        {
            n1_QtoGTranslator.Add(index, card.Key);
            n1_GtoQTranslator.Add(card.Key, index);
            index++;
        }

        index = 1;
        foreach (var card in CardDataBase.GetAllCards.Where(x => x.Value.nation == Nation2).ToArray())
        {
            n2_QtoGTranslator.Add(index, card.Key);
            n2_GtoQTranslator.Add(card.Key, index);
            index++;
        }

        bot1 = new(n1_QtoGTranslator.Count, LearningRate, DiscountFactor, n1_QtoGTranslator, n1_GtoQTranslator, RandInit, InitValue);
        botTarget = new(n2_QtoGTranslator.Count, LearningRate, DiscountFactor, n2_QtoGTranslator, n2_GtoQTranslator, RandInit, InitValue);


        for (int i = 0; i < MatchesCount; i++)
        {
            GameController gameController = new(Nation1, Nation2);
            List<MovesLog> movesLog = new();

            Dictionary<QLearning, AiPlayer> botPlayerRel = new() 
                { { bot1, gameController.Player },
                  { botTarget, gameController.TargetPlayer } };

            double curEps = Math.Max(InitMaxEps - (i / MatchesCount), 0.1);

            do
            {
                var curState = GetStateHash(gameController.GetCurState());
                var currentBot = botPlayerRel.Where(x => x.Value == gameController.CurrentPlayer).Select(x => x.Key).First();
                var action = currentBot.ChooseAction(curState, curEps, gameController.GetValidActions());
                gameController.MakeMove(action);

                movesLog.Add(new(curState, action, currentBot));

            } while (gameController.CurrentPlayer != null);

            QLearning? winner = null;

            if (gameController.Winner == botPlayerRel[bot1])
            {
                winner = bot1;
            }
            else if (gameController.Winner == botPlayerRel[botTarget])
            {
                winner = botTarget;
            }

            UpdateQValuesOffline(gameController.StatesLog, movesLog, winner);
        }

        WriteToFile(0);
    }

    void UpdateQValuesOffline(List<StatesLog> gameStatesLog, List<MovesLog> botMovesLog, QLearning? winner)
    {
        int nextStateBot1;
        int nextStateBot2;

        double bot1Reward;
        double bot2Reward;

        switch (winner)
        {
            case var value when value == bot1:
                nextStateBot1 = GetStateHash(WinState.Item1);
                nextStateBot2 = GetStateHash(LossState.Item1);
                bot1Reward = WinState.Item2;
                bot2Reward = LossState.Item2;
                break;
            case var value when value == botTarget:
                nextStateBot2 = GetStateHash(WinState.Item1);
                nextStateBot1 = GetStateHash(LossState.Item1);
                bot2Reward = WinState.Item2;
                bot1Reward = LossState.Item2;
                break;
            case var value when value == null:
                nextStateBot1 = GetStateHash(MatchState.Item1);
                nextStateBot2 = GetStateHash(MatchState.Item1);
                bot1Reward = MatchState.Item2;
                bot2Reward = MatchState.Item2;
                break;
            default:
                throw new Exception();
        }

        for (int i = botMovesLog.Count - 1; i >= 0; i--)
        {
            switch (botMovesLog[i].Bot)
            {
                case var value when value == bot1:
                    bot1.UpdateQValue(botMovesLog[i].State, botMovesLog[i].Action, nextStateBot1, bot1Reward);
                    bot1Reward = 0;
                    nextStateBot1 = botMovesLog[i].State;
                    break;
                case var value when value == botTarget:
                    botTarget.UpdateQValue(botMovesLog[i].State, botMovesLog[i].Action, nextStateBot2, bot2Reward);
                    bot2Reward = 0;
                    nextStateBot2 = botMovesLog[i].State;
                    break;
                default:
                    throw new Exception();
            }
        }
    }

    void WriteToFile(int index)
    {
        string directory = $"logs_{DateTime.Now.Ticks}";
        System.IO.Directory.CreateDirectory(directory);

        string nation_1_TablePath = $"{directory}/{Nation1}_QTable{index}.txt";        
        string nation_2_TablePath = $"{directory}/{Nation2}_QTable{index}.txt";
        string paramsPath = $"{directory}/Params{index}.txt";

        Task task = new Task(() =>
        {
            using (var sw = new StreamWriter(nation_1_TablePath))
            {
                foreach (var row in botTarget.QTable)
                {
                    sw.WriteLine($"{row.Key}:{string.Join('/', row.Value)}");
                }
            }
        });
        task.Start();

        using (var sw = new StreamWriter(nation_2_TablePath))
        {
            foreach (var row in bot1.QTable)
            {
                sw.WriteLine($"{row.Key}:{string.Join('/', row.Value)}");
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

        public MovesLog(int state, int action, QLearning bot)
        {
            State = state;
            Action = action;
            Bot = bot ?? throw new ArgumentNullException(nameof(bot));
        }
    }
}