using CardGameProj.Scripts;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;

namespace AiBot
{
    public class StartUpConfig
    {
        public static void Main()
        {
            CardDB.UpdateCardDataBase();

            while (true)
            {
                string? input = Console.ReadLine();

                switch (input)
                {
                    case "play":
                        Console.WriteLine("Tables Matches Processors");
                        int[] tablesMatches = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => int.Parse(x)).ToArray();
                        DoPlaying(tablesMatches[0], tablesMatches[1], tablesMatches[2]);
                        break;
                    case "learn":
                        Console.WriteLine("Processors");
                        int procCount = int.Parse(Console.ReadLine());
                        DoLearning(procCount);
                        break;
                    default:
                        Console.WriteLine("ooops");
                        break;
                }
            }
        }

        private static void DoLearning(int procCount)
        {
            string confiFilePath = "BotConfigs.json";
            string jsonData = File.ReadAllText(confiFilePath);
            if (jsonData is null)
                throw new Exception($"Reading {confiFilePath} failed");

            var botConfigs = JsonConvert.DeserializeObject<List<ConfStruct>>(jsonData);

            Parallel.ForEach(botConfigs, new ParallelOptions { MaxDegreeOfParallelism = procCount }, (config) =>
            {
                var botLearning = new BotLearning();
                botLearning.FixedEps = config.FixedEps;
                botLearning.WinState = config.WinState;
                botLearning.LossState = config.LossState;
                botLearning.MatchState = config.MatchState;
                botLearning.DiscountFactor = config.DiscountFactor;
                botLearning.LearningRate = config.LearningRate;
                botLearning.InitValue = config.InitValue;
                botLearning.RandInit = config.RandInit;
                botLearning.MatchesCount = config.MatchesCount;
                botLearning.firstBotNation = config.Nation1;
                botLearning.secondBotNation = config.Nation2;
                botLearning.StepRewards = config.StepRewards;

                botLearning.Start();
                Console.WriteLine("++");
            });

            Console.WriteLine("Done");
            Console.Beep(10000, 1500);
        }

        private static void DoPlaying(int tablesCount, int mathesCount, int processors)
        {
            Dictionary<string, int[]> gamesResults = new();

            Parallel.For(0, tablesCount, new ParallelOptions { MaxDegreeOfParallelism = processors }, (i) =>
            {
                for (int j = 0; j < tablesCount; j++)
                {               
                    string qTableBot1 = $"{i}/{CardNations.Confucius}_QTable.txt";
                    string qTableBot2 = $"{j}/{CardNations.AI}_QTable.txt";

                    var botPlaying = new BotPlaying(qTableBot1, qTableBot2);
                    botPlaying.Nation1 = CardNations.Confucius;
                    botPlaying.Nation2 = CardNations.AI;
                    botPlaying.MatchesCount = mathesCount;

                    botPlaying.Start();

                    Console.WriteLine($"i: {i}, j: {j}");
                    Console.WriteLine($"Bot {botPlaying.Nation1} wins: {botPlaying.Bot1Wins}");
                    Console.WriteLine($"Bot {botPlaying.Nation2} wins: {botPlaying.Bot2Wins}");

                    string confKey = CardNations.Confucius.ToString();
                    string aiKey = CardNations.AI.ToString();

                    lock (botPlaying) {
                        if (gamesResults.ContainsKey(confKey) is false)
                            gamesResults.Add(confKey, new int[tablesCount]);

                        if (gamesResults.ContainsKey(aiKey) is false)
                            gamesResults.Add(aiKey, new int[tablesCount]);

                        gamesResults[confKey][i] += botPlaying.Bot1Wins;
                        gamesResults[aiKey][j] += botPlaying.Bot2Wins;
                    }
                }
            });

            string outputPath = "GamesResults.txt";
            using (var sw = new StreamWriter(outputPath))
            {
                foreach (var item in gamesResults)
                {
                    sw.WriteLine($"{item.Key}: {string.Join(' ', item.Value.Select((value, index) => $"{index}:{value}"))}");
                }
            }

            Console.WriteLine("Done");
            Console.Beep(10000, 1500);
        }

        private class ConfStruct
        {
            public float FixedEps;
            public (string, float) WinState;
            public (string, float) LossState;
            public (string, float) MatchState;

            public int MatchesCount;
            public float LearningRate;
            public float DiscountFactor;
            public bool RandInit;
            public float InitValue;
            public CardNations Nation1;
            public CardNations Nation2;
            public bool StepRewards;
        }
    }
}
