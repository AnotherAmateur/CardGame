﻿using AiBot;
using CardGameProj.Scripts;

public class BotPlaying
{
    private const double Eps = -1;
    public int MatchesCount { get; set; }
    public CardNations Nation1 { get; set; }
    public CardNations Nation2 { get; set; }

    private string qTablePath1;
    private string qTablePath2;

    private Dictionary<int, int> n1_QtoGTranslator;
    private Dictionary<int, int> n2_QtoGTranslator;

    public int Bot1Wins { get; private set; }
    public int Bot2Wins { get; private set; }

    QLearning bot1;
    QLearning bot2;

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

        bot1 = new(ReadQTableFromFile(qTablePath1), n1_QtoGTranslator.Count, n1_QtoGTranslator, n1_GtoQTranslator);
        bot2 = new(ReadQTableFromFile(qTablePath2), n2_QtoGTranslator.Count, n2_QtoGTranslator, n2_GtoQTranslator);


        for (int i = 0; i < MatchesCount; i++)
        {
            GameController gameController = new(Nation1, Nation2);

            Dictionary<QLearning, AiPlayer> botPlayerRel = new()
                { { bot1, gameController.player },
                  { bot2, gameController.targetPlayer } };

            do
            {
                var curState = GetStateHash(gameController.GetCurState());
                var currentBot = botPlayerRel.Where(x => x.Value == gameController.CurrentPlayer).Select(x => x.Key).First();
                var action = currentBot.ChooseAction(curState, Eps, gameController.GetValidActions());
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

    Dictionary<int, double[]> ReadQTableFromFile(string path)
    {
        Dictionary<int, double[]> qTable = new();

        using (var sr = new StreamReader(path))
        {
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] temp = line.Split(':', StringSplitOptions.RemoveEmptyEntries);
                int state = int.Parse(temp[0]);
                double[] rewards = temp[1].Split('/', StringSplitOptions.RemoveEmptyEntries).Select(x => double.Parse(x)).ToArray();
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