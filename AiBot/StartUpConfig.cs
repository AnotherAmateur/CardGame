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
            while (true)
            {
                string? input = Console.ReadLine();

                switch (input)
                {
                    case "play":
                        DoPlaying();
                        break;
                    case "learn":
                        DoLearning();
                        break;
                    default:
                        Console.WriteLine("ooops");
                        break;
                }
            }
        }

        private static void DoLearning()
        {
            CardDB.UpdateCardDataBase();

            string confiFilePath = "BotConfigs.json";
            string jsonData = File.ReadAllText(confiFilePath);
            if (jsonData is null)
                throw new Exception($"Reading {confiFilePath} failed");

            var botConfigs = JsonConvert.DeserializeObject<List<ConfStruct>>(jsonData);
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
                botLearning.Nation1 = config.Nation1;
                botLearning.Nation2 = config.Nation2;

                botLearning.Start();
                Console.WriteLine("++");
            }

            //Parallel.ForEach(botLearnings, (botLearn) => botLearn.Start());

            Console.WriteLine("Done");
        }

        private static void DoPlaying()
        {
            int tablesCount = 6;

            for (int i = 0; i < tablesCount; i++)
            {
                for (int j = 0; j < tablesCount; j++)
                {
                    Console.WriteLine($"i: {i}, j: {j}");

                    string qTableBot1 = $"i/{CardNations.Confucius}_QTable0.txt";
                    string qTableBot2 = $"j/{CardNations.AI}_QTable0.txt";

                    var botPlaying = new BotPlaying(qTableBot1, qTableBot2);
                    botPlaying.Nation1 = CardNations.Confucius;
                    botPlaying.Nation2 = CardNations.AI;
                    botPlaying.MatchesCount = 10000;

                    botPlaying.Start();

                    Console.WriteLine($"Bot {botPlaying.Nation1} wins: {botPlaying.Bot1Wins}");
                    Console.WriteLine($"Bot {botPlaying.Nation2} wins: {botPlaying.Bot2Wins}");
                }
            }
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
        }
    }
}
