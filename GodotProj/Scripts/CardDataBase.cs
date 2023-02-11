using System.Collections.Generic;

public static class CardDataBase
{
	public struct CardData
	{
		public string Name;
		public string Type;
		public string Range;
		public int Damage;
		public string Nation;
	}

	private static Dictionary<string, CardData> cards;

	public static Dictionary<string, CardData> GetAllCards
	{
		get { return cards; }
	}


	public static CardData GetCardInfo(string cardName)
	{
		return cards[cardName];
	}


	public static string GetCardTexturePath(string cardName)
	{
		return "res://Assets/Cards/" + CardDataBase.cards[cardName].Nation + "/" + CardDataBase.cards[cardName].Type + "/" + cardName + ".png";
	}


	public static void UpdateCardDataBase()
	{
		cards = new();

		cards.Add("Card1", new CardData { Name = "Card1", Type = "Squad", Damage = 30, Nation = "Nation1", Range = "1" });
		cards.Add("Card2", new CardData { Name = "Card2", Type = "Squad", Damage = 20, Nation = "Nation1", Range = "1" });
		cards.Add("Card3", new CardData { Name = "Card3", Type = "Squad", Damage = 30, Nation = "Nation1", Range = "1" });
		cards.Add("Card4", new CardData { Name = "Card4", Type = "Squad", Damage = 20, Nation = "Nation1", Range = "1" });
		cards.Add("Card5", new CardData { Name = "Card5", Type = "Squad", Damage = 20, Nation = "Nation1", Range = "1" });
		cards.Add("Card6", new CardData { Name = "Card6", Type = "Squad", Damage = 20, Nation = "Nation1", Range = "2" });
		cards.Add("1", new CardData { Name = "1", Type = "Squad", Damage = 20, Nation = "Nation1", Range = "2" });
		cards.Add("2", new CardData { Name = "2", Type = "Squad", Damage = 20, Nation = "Nation1", Range = "2" });
		cards.Add("3", new CardData { Name = "3", Type = "Squad", Damage = 20, Nation = "Nation1", Range = "2" });
		cards.Add("Card0", new CardData { Name = "Card0", Type = "Leader", Nation = "Nation1", Damage = 20, Range = null });

		//cards.Add("Card10", new CardData { Name = "Card10", Type = "Squad", Damage = 30, Nation = "Nation2", Range = "Melee" });
		//cards.Add("Card20", new CardData { Name = "Card20", Type = "Squad", Damage = 20, Nation = "Nation2", Range = "Melee" });
		//cards.Add("Card30", new CardData { Name = "Card30", Type = "Squad", Damage = 30, Nation = "Nation2", Range = "Siege" });
		//cards.Add("Card40", new CardData { Name = "Card40", Type = "Squad", Damage = 20, Nation = "Nation2", Range = "Siege" });
		//cards.Add("Card50", new CardData { Name = "Card50", Type = "Squad", Damage = 20, Nation = "Nation2", Range = "Melee" });
		//cards.Add("Card60", new CardData { Name = "Card60", Type = "Squad", Damage = 20, Nation = "Nation2", Range = "LongRange" });
		//cards.Add("1", new CardData { Name = "1", Type = "Squad", Damage = 20, Nation = "Nation2", Range = "LongRange" });
		//cards.Add("2", new CardData { Name = "2", Type = "Squad", Damage = 20, Nation = "Nation2", Range = "LongRange" });
		//cards.Add("3", new CardData { Name = "3", Type = "Squad", Damage = 20, Nation = "Nation2", Range = "LongRange" });
		cards.Add("4", new CardData { Name = "4", Type = "Squad", Damage = 20, Nation = "Nation2", Range = "3" });
		cards.Add("5", new CardData { Name = "5", Type = "Squad", Damage = 20, Nation = "Nation2", Range = "3" });
		cards.Add("6", new CardData { Name = "6", Type = "Squad", Damage = 20, Nation = "Nation2", Range = "3" });
		cards.Add("Card", new CardData { Name = "Card", Type = "Leader", Nation = "Nation2", Damage = 20, Range = null });
	}
}
