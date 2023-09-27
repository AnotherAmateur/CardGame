using CardGameProj.Scripts;
using Newtonsoft.Json;
using System;
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
                        int[] tablesMatches = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => int.Parse(x)).ToArray();
                        DoPlaying(tablesMatches[0], tablesMatches[1]);
                        break;
                    case "learn":
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
            List<BotLearning> botLearnings = new();
            foreach (var config in botConfigs)
            {
                var botLearning = new BotLearning();
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

                botLearnings.Add(botLearning);
            }

            System.Threading.Tasks.ParallelOptions opt = new System.Threading.Tasks.ParallelOptions();
            opt.MaxDegreeOfParallelism = procCount;
            Parallel.ForEach(botLearnings, opt, (botLearn) =>
            {
                botLearn.Start();
                Console.WriteLine("++");

            });

            Console.WriteLine("Done");
            Console.Beep(5000, 2000);
        }

        private static void DoPlaying(int tablesCount, int mathesCount)
        {
            for (int i = 0; i < tablesCount; i++)
            {
                for (int j = 0; j < tablesCount; j++)
                {
                    Console.WriteLine($"i: {i}, j: {j}");

                    string qTableBot1 = $"{i}/{CardNations.Confucius}_QTable.txt";
                    string qTableBot2 = $"{j}/{CardNations.AI}_QTable.txt";

                    var botPlaying = new BotPlaying(qTableBot1, qTableBot2);
                    botPlaying.Nation1 = CardNations.Confucius;
                    botPlaying.Nation2 = CardNations.AI;
                    botPlaying.MatchesCount = mathesCount;

                    botPlaying.Start();

                    Console.WriteLine($"Bot {botPlaying.Nation1} wins: {botPlaying.Bot1Wins}");
                    Console.WriteLine($"Bot {botPlaying.Nation2} wins: {botPlaying.Bot2Wins}");
                }
            }

            Console.WriteLine("Done");
            Console.Beep(5000, 2000);
        }

        private class ConfStruct
        {
            public (string, double) WinState;
            public (string, double) LossState;
            public (string, double) MatchState;

            public int MatchesCount;
            public double LearningRate;
            public double DiscountFactor;
            public bool RandInit;
            public double InitValue;
            public CardNations Nation1;
            public CardNations Nation2;
            public bool StepRewards;
        }
    }
}
