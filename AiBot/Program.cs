using AiBot;
using CardGameProj.Scripts;

(string, double) WinState = ("WIN", 100);
(string, double) LossState = ("LOSS", -100);
(string, double) MatchState = ("MATCH", 10);

const int matchesCount = (int)9e4;
const double LearningRate = 0.9;
const double DiscountFactor = 0.75;
double initMaxEps = 1;


Dictionary<int, int> confQtoGTranslator = new();
Dictionary<int, int> aiQtoGTranslator = new();
confQtoGTranslator.Add(0, (int)ActionTypes.Pass);
aiQtoGTranslator.Add(0, (int)ActionTypes.Pass);

Dictionary<int, int> confGtoQTranslator = new();
Dictionary<int, int> aiGtoQTranslator = new();
confGtoQTranslator.Add((int)ActionTypes.Pass, 0);
aiGtoQTranslator.Add((int)ActionTypes.Pass, 0);

CardDataBase.UpdateCardDataBase();
int index = 1;
foreach (var card in CardDataBase.GetAllCards.Where(x => x.Value.nation == CardNations.Confucius).ToArray())
{
    confQtoGTranslator.Add(index, card.Key);
    confGtoQTranslator.Add(card.Key, index);
    index++;
}

index = 1;
foreach (var card in CardDataBase.GetAllCards.Where(x => x.Value.nation == CardNations.AI).ToArray())
{
    aiQtoGTranslator.Add(index, card.Key);
    aiGtoQTranslator.Add(card.Key, index);
    index++;
}

QLearning bot1 = new(confQtoGTranslator.Count, LearningRate, DiscountFactor, confQtoGTranslator, confGtoQTranslator);
QLearning bot2 = new(aiQtoGTranslator.Count, LearningRate, DiscountFactor, aiQtoGTranslator, aiGtoQTranslator);


for (int i = 0; i < matchesCount; i++)
{
    GameController gameController = new(CardNations.Confucius, CardNations.AI);
    List<MovesLog> movesLog = new();

    Dictionary<QLearning, AiPlayer> botPlayerRel = new() {
        { bot1, gameController.player1 },
        { bot2, gameController.player2 } };

    double curEps = Math.Max(initMaxEps - (i / matchesCount), 0.1);

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
    else if (gameController.Winner == botPlayerRel[bot2])
    {
        winner = bot2;
    }

    UpdateQValuesOffline(gameController.StatesLog, movesLog, winner);
}

WriteToFile(1);



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
        case var value when value == bot2:
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
  

    for (int i = gameStatesLog.Count - 1; i >= 0; i--)
    {
        switch (botMovesLog[i].Bot)
        {
            case var value when value == bot1:
                bot1.UpdateQValue(botMovesLog[i].State, botMovesLog[i].Action, nextStateBot1, bot1Reward);
                bot1Reward = 0;
                nextStateBot1 = botMovesLog[i].State;
                break;
            case var value when value == bot2:
                bot2.UpdateQValue(botMovesLog[i].State, botMovesLog[i].Action, nextStateBot2, bot2Reward);
                bot2Reward = 0;
                nextStateBot2 = botMovesLog[i].State;
                break;
        }
    }
}

void WriteToFile(int index)
{
    string directory = "logs";
    System.IO.Directory.CreateDirectory(directory);

    string aiNationTablePath = $"{directory}/AI_QTable{index}.txt";
    string confNationTablePath = $"{directory}/Confucius_QTable{index}.txt";

    using (var sw = new StreamWriter(aiNationTablePath))
    {
        foreach (var row in bot2.QTable)
        {
            sw.WriteLine($"{row.Key}:{string.Join('/', row.Value)}");
        }
    }

    using (var sw = new StreamWriter(confNationTablePath))
    {
        foreach (var row in bot1.QTable)
        {
            sw.WriteLine($"{row.Key}:{string.Join('/', row.Value)}");
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
