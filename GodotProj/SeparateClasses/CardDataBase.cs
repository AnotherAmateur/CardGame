using CardGameProj.Scripts;
using System;
using System.Collections.Generic;
using Godot;

public static class CardDataBase
{
	static Random rand = new Random();

	public struct CardData
	{
		public int id;
		public CardTypes type;
		public int strength;
		public CardNations nation;
		public CardRanks rank;
		public string text;
		public string category;

		public CardData(int id, CardTypes type, int strength, CardNations nation, string text)
		{
			this.id = id;
			this.type = type;
			this.strength = strength;
			//this.strength = rand.Next(1, 31);
			this.nation = nation;
			this.text = text;

			switch (strength)
			{
				case < 10:
					this.rank = CardRanks.Common;
					break;
				case < 20:
					this.rank = CardRanks.Rare;
					break;
				default:
					this.rank = CardRanks.Legendary;
					break;
			}

			switch (type)
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
					category = nation + ", мудрец";
					break;
				case CardTypes.Special:
					category = "Специальная карта";
					break;
			}
		}
	}

	private static Dictionary<int, CardData> cards;

	public static Dictionary<int, CardData> GetAllCards
	{
		get { return new Dictionary<int, CardData>(cards); }
	}

	public static CardData GetCardInfo(int cardId)
	{
		return cards[cardId];
	}

	public static string GetCardTexturePath(int cardId)
	{
		var card = cards[cardId];
		return "res://Assets/Cards/" + card.nation + "/" + card.type + "/" + cardId + ".png";
	}

	public static string GetFlippedCardTexturePath(CardNations cardNation)
	{
		return "res://Assets/FlippedCards/" + cardNation.ToString() + ".png";
	}

	public static void UpdateCardDataBase()
	{
		int cardId = 0;
		cards = new();

		// AI
		{
			cards.Add(cardId, new CardData(id: cardId++, type: CardTypes.Leader, strength: 20, nation: CardNations.AI, ""));

			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group1,
				strength: 30,
				nation: CardNations.AI,
				text: "Истина - это не то, что ты видишь, а то, что ты осознаешь."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group1,
				strength: 20,
				nation: CardNations.AI,
				text: "Мудрость - это способность видеть красоту в простых вещах."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group1,
				strength: 10,
				nation: CardNations.AI,
				text: "Самая глубокая мудрость приходит из простоты."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group1,
				strength: 20,
				nation: CardNations.AI,
				text: "Мудрый человек видит дальше других, потому что он поднимает глаза выше уровня повседневности."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group1,
				strength: 5,
				nation: CardNations.AI,
				text: "Мудрость - это не только знание, но и способность его применить."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group1,
				strength: 20,
				nation: CardNations.AI,
				text: "Мудрость - это смелость пройти сквозь туман неведения и найти истину."
			));


			cardId = 101;

			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group2,
				strength: 30,
				nation: CardNations.AI,
				text: "Жизнь - это не ожидание, а возможности."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group2,
				strength: 20,
				nation: CardNations.AI,
				text: "Секрет успеха - это постоянное стремление к самосовершенствованию."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group2,
				strength: 10,
				nation: CardNations.AI,
				text: "Никогда не сдавайся, даже если путь к цели кажется трудным. Возможности появляются, когда мы идем вперед."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group2,
				strength: 20,
				nation: CardNations.AI,
				text: "Большие мечты требуют больших усилий. Не останавливайся, пока не достигнешь своей цели."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group2,
				strength: 5,
				nation: CardNations.AI,
				text: "Успех - это не конечная точка, это начало новых высот."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group2,
				strength: 20,
				nation: CardNations.AI,
				text: "Истинная мотивация приходит изнутри. Если ты веришь в себя, никто не сможет остановить тебя."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group2,
				strength: 4,
				nation: CardNations.AI,
				text: "Твои мысли - это семена, которые станут твоей реальностью. Выбирай их с мудростью."
			));

			cardId = 201;

			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group3,
				strength: 30,
				nation: CardNations.AI,
				text: "Поступай с другими так, как ты хотел бы, чтобы они поступали с тобой."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group3,
				strength: 20,
				nation: CardNations.AI,
				text: "Истинное богатство не в материальных вещах, а в доброте и справедливости."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group3,
				strength: 10,
				nation: CardNations.AI,
				text: "Настоящий лидер - это тот, кто ставит интересы других выше своих собственных."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group3,
				strength: 20,
				nation: CardNations.AI,
				text: "Мир начинается с улыбки. Будь добр к каждому, кто пересекает твой путь."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group3,
				strength: 5,
				nation: CardNations.AI,
				text: "Будь тем изменением, которое ты хочешь видеть в мире."
			));

			cardId = 301;

			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Special,
				strength: -5,
				nation: CardNations.AI,
				text: "Уменьшает вес каждой карты на одной линии оппонента на указанное число"
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Special,
				strength: 0,
				nation: CardNations.AI,
				text: "Отменяет все специальные карты"
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Special,
				strength: 5,
				nation: CardNations.AI,
				text: "Увеличивает вес каждой карты на одной линии на указанное число"
			));
		}

		// Confucius
		{
			cardId = 1000;

			cards.Add(cardId, new CardData(id: cardId++, type: CardTypes.Leader, strength: 20, nation: CardNations.Confucius, ""));

			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group1,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Тот, кто знает, что он не знает, — самый умный. Тот, кто не знает, что он не знает, — глупый."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group1,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Лучше видеть один раз, чем слышать сто раз."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group1,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Человек, который хочет переместить гору, должен начать с того, чтобы убрать маленький камешек."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group1,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Истинная мудрость заключается в том, чтобы познать, что ты ничего не знаешь."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group1,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Знание без мышления — пустое; мышление без знания — опасно."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group1,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Учиться и не мыслить — напрасно трудиться."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group1,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Желание знаний — первое признак добродетели."
			));


			cardId = 1101;

			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group2,
				strength: 30,
				nation: CardNations.Confucius,
				text: "Приступая к любому делу, помни о его цели."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group2,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Даже самый длинный путь начинается с первого шага."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group2,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Победа не всегда победа, поражение не всегда поражение."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group2,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Путешествие тысячи миль начинается с одного шага."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group2,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Никогда не отказывайся от своей цели, неважно, насколько далеко ты от нее находишься."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group2,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Самая большая слава не в том, чтобы никогда не падать, а в том, чтобы каждый раз подниматься после падения."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group2,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Неважно, насколько медленно ты идешь, главное — не останавливаться."
			));

			cardId = 1201;

			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group3,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Не делай другому то, чего не хотел бы для себя."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group3,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Справедливость основана на том, чтобы дать каждому то, что ему положено."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group3,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Лучший способ построить доверие — быть доверительным."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group3,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Великий человек предвидит все, что произойдет; маленький человек, что происходит."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group3,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Лучше потерять немного выгоды, чем утратить доверие."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group3,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Что ты не желаешь себе, не делай другим."
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Group3,
				strength: 20,
				nation: CardNations.Confucius,
				text: "Справедливость без милосердия есть жестокость; милосердие без справедливости есть слабость."
			));


			cardId = 1301;

			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Special,
				strength: -5,
				nation: CardNations.Confucius,
				text: "Уменьшает вес каждой карты на одной линии оппонента на указанное число"
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Special,
				strength: 0,
				nation: CardNations.Confucius,
				text: "Отменяет все специальные карты"
			));
			cards.Add(cardId, new CardData
			(
				id: cardId++,
				type: CardTypes.Special,
				strength: 5,
				nation: CardNations.Confucius,
				text: "Увеличивает вес каждой карты на одной линии на указанное число"
			));
		}

	}
}
