using CardGameProj.Scripts;
using System.Net;
using System.Threading.Channels;
using Newtonsoft.Json;

namespace AiBot
{
    public class StartUpConfig
    {

        public static void Main()
        {
            string fileName = "Cards.json";
            CardDataBase.UpdateCardDataBase();
            string json = JsonConvert.SerializeObject(CardDataBase.GetAllCards, Formatting.Indented);
            File.WriteAllText(fileName, json);

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
                    case "q":
                        return;
                    default:
                        Console.WriteLine("ooops");
                        break;
                }
            }
        }


        private static void DoLearning()
        {
            CardDataBase.UpdateCardDataBase();
            List<BotLearning> botLearnings = new();

            botLearnings.Add(new BotLearning());
            botLearnings.Last().WinState = ("WIN", 100);
            botLearnings.Last().LossState = ("LOSS", -100);
            botLearnings.Last().MatchState = ("MATCH", 10);
            botLearnings.Last().DiscountFactor = 0.9;
            botLearnings.Last().LearningRate = 0.5;
            botLearnings.Last().EachStepReward = false;
            botLearnings.Last().InitValue = 0;
            botLearnings.Last().RandInit = false;
            botLearnings.Last().MatchesCount = (int)4e1;
            botLearnings.Last().Nation1 = CardNations.Confucius;
            botLearnings.Last().Nation2 = CardNations.AI;

            botLearnings.Add(new BotLearning());
            botLearnings.Last().WinState = ("WIN", 100);
            botLearnings.Last().LossState = ("LOSS", -100);
            botLearnings.Last().MatchState = ("MATCH", 10);
            botLearnings.Last().DiscountFactor = 0.9;
            botLearnings.Last().LearningRate = 0.5;
            botLearnings.Last().EachStepReward = false;
            botLearnings.Last().InitValue = 0.7;
            botLearnings.Last().RandInit = false;
            botLearnings.Last().MatchesCount = (int)4e1;
            botLearnings.Last().Nation1 = CardNations.Confucius;
            botLearnings.Last().Nation2 = CardNations.AI;

            botLearnings.Add(new BotLearning());
            botLearnings.Last().WinState = ("WIN", 100);
            botLearnings.Last().LossState = ("LOSS", -100);
            botLearnings.Last().MatchState = ("MATCH", 10);
            botLearnings.Last().DiscountFactor = 0.9;
            botLearnings.Last().LearningRate = 0.5;
            botLearnings.Last().EachStepReward = false;
            botLearnings.Last().InitValue = 0.3;
            botLearnings.Last().RandInit = false;
            botLearnings.Last().MatchesCount = (int)4e1;
            botLearnings.Last().Nation1 = CardNations.Confucius;
            botLearnings.Last().Nation2 = CardNations.AI;

            botLearnings.Add(new BotLearning());
            botLearnings.Last().WinState = ("WIN", 100);
            botLearnings.Last().LossState = ("LOSS", -100);
            botLearnings.Last().MatchState = ("MATCH", 10);
            botLearnings.Last().DiscountFactor = 0.9;
            botLearnings.Last().LearningRate = 0.5;
            botLearnings.Last().EachStepReward = false;
            botLearnings.Last().InitValue = 0;
            botLearnings.Last().RandInit = true;
            botLearnings.Last().MatchesCount = (int)4e1;
            botLearnings.Last().Nation1 = CardNations.Confucius;
            botLearnings.Last().Nation2 = CardNations.AI;

            botLearnings.Add(new BotLearning());
            botLearnings.Last().WinState = ("WIN", 1);
            botLearnings.Last().LossState = ("LOSS", -1);
            botLearnings.Last().MatchState = ("MATCH", 0.1);
            botLearnings.Last().DiscountFactor = 0.9;
            botLearnings.Last().LearningRate = 0.5;
            botLearnings.Last().EachStepReward = false;
            botLearnings.Last().InitValue = 0;
            botLearnings.Last().RandInit = false;
            botLearnings.Last().MatchesCount = (int)4e1;
            botLearnings.Last().Nation1 = CardNations.Confucius;
            botLearnings.Last().Nation2 = CardNations.AI;


            Parallel.ForEach(botLearnings, (botLearn) => botLearn.Start());

            Console.WriteLine("Done");
        }

        private static void DoPlaying()
        {

        }
    }
}
