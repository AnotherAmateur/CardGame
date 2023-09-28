using AiBot;
using CardGameProj.Scripts;
using System.Text;

public class BotPlaying
{
    private const float Eps = -1;
    public int MatchesCount { get; set; }
    public CardNations Nation1 { get; set; }
    public CardNations Nation2 { get; set; }

    private string qTablePath1;
    private string qTablePath2;

    private Dictionary<int, int> n1_QtoGTranslator;
    private Dictionary<int, int> n2_QtoGTranslator;

    public int Bot1Wins { get; private set; }
    public int Bot2Wins { get; private set; }

    QLPlay bot1;
    QLPlay bot2;

    public BotPlaying(string qTablePath1, string qTablePath2)
    {
        this.n1_QtoGTranslator = new();
        this.n2_QtoGTranslator = new();
        this.qTablePath1 = qTablePath1;
        this.qTablePath2 = qTablePath2;
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
        foreach (var card in CardDB.GetAllCards.Where(x => x.Value.Nation == Nation1 && x.Value.Type != CardTypes.Leader))
        {
            n1_QtoGTranslator.Add(index, card.Key);
            n1_GtoQTranslator.Add(card.Key, index);
            index++;
        }

        index = 1;
        foreach (var card in CardDB.GetAllCards.Where(x => x.Value.Nation == Nation2 && x.Value.Type != CardTypes.Leader))
        {
            n2_QtoGTranslator.Add(index, card.Key);
            n2_GtoQTranslator.Add(card.Key, index);
            index++;
        }

        var task = Task.Factory.StartNew(() =>
        bot1 = new(ReadQTableFromFile(qTablePath1), n1_QtoGTranslator.Count, n1_QtoGTranslator, n1_GtoQTranslator));
        bot2 = new(ReadQTableFromFile(qTablePath2), n2_QtoGTranslator.Count, n2_QtoGTranslator, n2_GtoQTranslator);
        task.Wait();

        for (int i = 0; i < MatchesCount; i++)
        {
            GameController gameController = new(Nation1, Nation2);

            Dictionary<QLPlay, AiPlayer> botPlayerRel = new()
                { { bot1, gameController.FirstPl },
                  { bot2, gameController.SecondPl } };

            do
            {
                var curState = GetStateHash(gameController.GetCurStateString());
                var currentBot = botPlayerRel.Where(x => x.Value == gameController.CurrentPlayer).Select(x => x.Key).First();
                var action = currentBot.GetBestAction(curState, Eps, gameController.GetValidActions());
                gameController.MakeMove(action);

            } while (gameController.CurrentPlayer != null);

            if (gameController.Winner == botPlayerRel[bot1])
            {
                ++Bot1Wins;
            }
            else if (gameController.Winner == botPlayerRel[bot2])
            {
                ++Bot2Wins;
            }
        }
    }

    Dictionary<int, short[]> ReadQTableFromFile(string path)
    {
        const int charMargin = 48;
        Dictionary<int, short[]> qTable = new();

        using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        using (var sr = new StreamReader(fileStream, Encoding.ASCII))
        {
            foreach (string line in sr.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                string[] temp = line.Split('#', StringSplitOptions.RemoveEmptyEntries);
                int state = int.Parse(temp[0]);
                string rewardsString = temp[1];

                short[] rewards = new short[rewardsString.Length];
                for (int i = 0; i < rewards.Length; i++)
                {
                    rewards[i] = (short)((char)rewardsString[i] - charMargin);
                }

                qTable.Add(state, rewards);
            }
        }

        return qTable;
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
}