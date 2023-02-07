using System;
using System.Collections.Generic;
using System.IO;

public static class CardDataBase
{
	public struct CardData
	{
		public string Name;
		public string Type;
		public int Damage;
	}

	private static Dictionary<string, CardData> cards;
	public static Dictionary<string, CardData> Cards { get { return cards; } }


	public static void UpdateCardDataBase()
	{
		cards = new();		

		cards.Add("Card1", new CardData { Name = "Card1", Type = "Squad", Damage = 30 });
		cards.Add("Card2", new CardData { Name = "Card2", Type = "Squad", Damage = 20 });
		cards.Add("Card3", new CardData { Name = "Card3", Type = "Squad", Damage = 30 });
		cards.Add("Card4", new CardData { Name = "Card4", Type = "Squad", Damage = 20 });
		cards.Add("Card5", new CardData { Name = "Card5", Type = "Squad", Damage = 20 });
		cards.Add("Card6", new CardData { Name = "Card6", Type = "Squad", Damage = 20 });
		cards.Add("Card0", new CardData { Name = "Card0", Type = "Leader", Damage = 20 });
	}
}
