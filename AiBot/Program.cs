using AiBot;
using CardGameProj.Scripts;


const int matchesCount = 100;
const double LearningRate = 0.9;
const double DiscountFactor = 0.75;
double initMaxEps = 1;

Dictionary<int, int> confTranslator = new();
Dictionary<int, int> aiTranslator = new();
confTranslator.Add(0, (int)ActionTypes.Pass);
aiTranslator.Add(0, (int)ActionTypes.Pass);

int index = 1;
foreach (var card in CardDataBase.GetAllCards.Where(x => x.Value.nation == CardNations.Confucius).ToArray())
{
    confTranslator.Add(index, card.Key);
    index++;
}

index = 1;
foreach (var card in CardDataBase.GetAllCards.Where(x => x.Value.nation == CardNations.AI).ToArray())
{
    aiTranslator.Add(index, card.Key);
    index++;
}

QLearning bot1 = new(confTranslator.Count, LearningRate, DiscountFactor, confTranslator);
QLearning bot2 = new(aiTranslator.Count, LearningRate, DiscountFactor, aiTranslator);


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
        var curState = GetCustomHash(gameController.GetCurState());
        var currentBot = botPlayerRel.Where(x => x.Value == gameController.CurrentPlayer).Select(x => x.Key).First();       
        var action = currentBot.ChooseAction(curState, curEps);
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

    WriteToFile(i);
}


void UpdateQValuesOffline(List<StatesLog> gameStatesLog, List<MovesLog> botMovesLog, QLearning? winner)
{

}

void WriteToFile(int index)
{
    string aiNationTablePath = $"AI_QTable.txt{index}";
    string confNationTablePath = $"Confucius_QTable.txt{index}";

    using (var sw = new StreamWriter(aiNationTablePath))
    {
        foreach (var row in bot2.QTable)
        {
            sw.WriteLine(string.Join(' ', row));
        }
    }

    using (var sw = new StreamWriter(confNationTablePath))
    {
        foreach (var row in bot1.QTable)
        {
            sw.WriteLine(string.Join(' ', row));
        }
    }
}

int GetCustomHash(string input)
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
