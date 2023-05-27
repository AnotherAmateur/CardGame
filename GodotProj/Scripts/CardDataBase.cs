using CardGameProj.Scripts; using System; using System.Collections.Generic; using Godot;  public static class CardDataBase { 	public struct CardData 	{ 		public int id; 		public CardTypes type; 		public int strength; 		public CardNations nation; 		public CardRanks rank; 		public string text; 		public string category;  		public CardData(int id, CardTypes type, int strength, CardNations nation, string text) 		{ 			this.id = id; 			this.type = type; 			this.strength = strength; 			this.nation = nation; 			this.text = text;  			switch (strength) 			{ 				case < 10: 					this.rank = CardRanks.Common; 					break; 				case < 20: 					this.rank = CardRanks.Rare; 					break; 				default: 					this.rank = CardRanks.Legendary; 					break; 			}  			switch (type)
			{
				case CardTypes.Group1:
					category = "Мудрость и философия";
					break;
				case CardTypes.Group2:
					category = "Личностное развитие и мотивация";
					break;
				case CardTypes.Group3:
					category = "Социальная справедливость и этика";
					break;
				case CardTypes.Leader:
					category = $"{nation}, мудрец";
					break;
				case CardTypes.Special:
					category = "Специальная карта";
					break;
			}
		} 	}  	private static Dictionary<int, CardData> cards;  	public static Dictionary<int, CardData> GetAllCards 	{ 		get { return new Dictionary<int, CardData>(cards); } 	}   	public static CardData GetCardInfo(int cardId) 	{ 		return cards[cardId]; 	}   	public static string GetCardTexturePath(int cardId) 	{
		var card = cards[cardId];
		return "res://Assets/Cards/" + card.nation + "/" + card.type + "/" + cardId + ".png"; 	}   	public static void UpdateCardDataBase() 	{ 		int id = 0; 		cards = new();  		// nation 1 		{
			cards.Add(id, new CardData { id = id++, type = CardTypes.Leader, nation = CardNations.AI, strength = 20, });

			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group1,
				strength = 30,
				nation = CardNations.AI,
				text = "Истина - это не то, что ты видишь, а то, что ты осознаешь."
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group1,
				strength = 20,
				nation = CardNations.AI,
				text = "Мудрость - это способность видеть красоту в простых вещах."
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group1,
				strength = 10,
				nation = CardNations.AI,
				text = "Самая глубокая мудрость приходит из простоты."
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group1,
				strength = 20,
				nation = CardNations.AI,
				text = "Мудрый человек видит дальше других, потому что он поднимает глаза выше уровня повседневности."
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group1,
				strength = 5,
				nation = CardNations.AI,
				text = "Мудрость - это не только знание, но и способность его применить."
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group1,
				strength = 20,
				nation = CardNations.AI,
				text = "Мудрость - это смелость пройти сквозь туман неведения и найти истину."
			});


			id = 101;

			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group2,
				strength = 30,
				nation = CardNations.AI,
				text = "Жизнь - это не ожидание, а возможности."
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group2,
				strength = 20,
				nation = CardNations.AI,
				text = "Секрет успеха - это постоянное стремление к самосовершенствованию."
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group2,
				strength = 10,
				nation = CardNations.AI,
				text = "Никогда не сдавайся, даже если путь к цели кажется трудным. Возможности появляются, когда мы идем вперед."
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group2,
				strength = 20,
				nation = CardNations.AI,
				text = "Большие мечты требуют больших усилий. Не останавливайся, пока не достигнешь своей цели."
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group2,
				strength = 5,
				nation = CardNations.AI,
				text = "Успех - это не конечная точка, это начало новых высот."
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group2,
				strength = 20,
				nation = CardNations.AI,
				text = "Истинная мотивация приходит изнутри. Если ты веришь в себя, никто не сможет остановить тебя."
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group2,
				strength = 4,
				nation = CardNations.AI,
				text = "Твои мысли - это семена, которые станут твоей реальностью. Выбирай их с мудростью."
			});

			id = 201;

			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group3,
				strength = 30,
				nation = CardNations.AI,
				text = "Поступай с другими так, как ты хотел бы, чтобы они поступали с тобой."
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group3,
				strength = 20,
				nation = CardNations.AI,
				text = "Истинное богатство не в материальных вещах, а в доброте и справедливости."
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group3,
				strength = 10,
				nation = CardNations.AI,
				text = "Настоящий лидер - это тот, кто ставит интересы других выше своих собственных."
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group3,
				strength = 20,
				nation = CardNations.AI,
				text = "Мир начинается с улыбки. Будь добр к каждому, кто пересекает твой путь."
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group3,
				strength = 5,
				nation = CardNations.AI,
				text = "Будь тем изменением, которое ты хочешь видеть в мире."
			});

			id = 301;

			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Special,
				strength = 30,
				nation = CardNations.AI,
				text = ""
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Special,
				strength = 20,
				nation = CardNations.AI,
				text = ""
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Special,
				strength = 10,
				nation = CardNations.AI,
				text = ""
			});
		}

		// nation 2
		{
			id = 1000;

			cards.Add(id, new CardData { id = id++, type = CardTypes.Leader, nation = CardNations.Confucius, strength = 20, });

			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group1,
				strength = 30,
				nation = CardNations.Confucius,
				text = "Поступай"
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Group1,
				strength = 20,
				nation = CardNations.Confucius,
				text = "Истинное"
			});

			id = 1301;

			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Special,
				strength = 30,
				nation = CardNations.Confucius,
				text = ""
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Special,
				strength = 20,
				nation = CardNations.Confucius,
				text = ""
			});
			cards.Add(id, new CardData
			{
				id = id++,
				type = CardTypes.Special,
				strength = 10,
				nation = CardNations.Confucius,
				text = ""
			});
		}
 	} } 