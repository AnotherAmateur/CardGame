using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp1
{
	internal class Program
	{
		public enum CardTypes { Group1, Group2, Group3, Leader, Special }
		public enum CardNations { AI, Confucius };
		public enum CardRanks { Common, Rare, Legendary };

		public struct CardData
		{
			public int id;
			public CardTypes type;
			public int strength;
			public CardNations nation;
			public CardRanks rank;
			public string text;

			public CardData(int id, CardTypes type, int strength, CardNations nation, string text)
			{
				this.id = id;
				this.type = type;
				this.strength = strength;
				this.nation = nation;
				this.text = text;
				this.rank = CardRanks.Rare;

			}
		}

		private static Dictionary<int, CardData> cards;

		public static Dictionary<int, CardData> GetAllCards
		{
			get { return cards; }
		}


		public static CardData GetCardInfo(int cardId)
		{
			return cards[cardId];
		}


		public static string GetCardTexturePath(int cardId)
		{
			var card = cards[cardId];
			return $"res://Assets/Cards/{card.nation}/{card.type}/{card.type}{cardId}.png";
		}


		public static void UpdateCardDataBase()
		{
			int id = 0;
			cards = new Dictionary<int, CardData>();

			cards.Add(id, new CardData { id = id++, type = CardTypes.Leader, nation = CardNations.AI, strength = 20, });
			cards.Add(id, new CardData { id = id++, type = CardTypes.Group1, strength = 30, nation = CardNations.AI, text = "Истина - это не то, что ты видишь, а то, что ты осознаешь." });
			cards.Add(id, new CardData { id = id++, type = CardTypes.Group1, strength = 20, nation = CardNations.AI, text = "Мудрость - это способность видеть красоту в простых вещах." });
			cards.Add(id, new CardData { id = id++, type = CardTypes.Group1, strength = 10, nation = CardNations.AI, text = "Самая глубокая мудрость приходит из простоты." });
			cards.Add(id, new CardData { id = id++, type = CardTypes.Group1, strength = 20, nation = CardNations.AI, text = "Мудрый человек видит дальше других, потому что он поднимает глаза выше уровня повседневности." });
			cards.Add(id, new CardData { id = id++, type = CardTypes.Group1, strength = 5, nation = CardNations.AI, text = "Мудрость - это не только знание, но и способность его применить." });
			cards.Add(id, new CardData { id = id++, type = CardTypes.Group1, strength = 20, nation = CardNations.AI, text = "Мудрость - это смелость пройти сквозь туман неведения и найти истину." });
			cards.Add(id, new CardData { id = id++, type = CardTypes.Group1, strength = 4, nation = CardNations.AI, text = "Самый мудрый человек - это тот, кто знает, что он ничего не знает." });
		}
		static void Main(string[] args)
		{
			UpdateCardDataBase();
			int cardId = 0;
			var card = GetCardInfo(cardId);
            Console.WriteLine($"res://Assets/Cards/{card.nation}/{card.type}/{cardId}.png");
			Console.Read();
        }
	}
}
