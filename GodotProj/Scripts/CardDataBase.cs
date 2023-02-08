using System;
using System.Collections.Generic;
using Godot;

public static class CardDataBase
{
	public struct CardData
	{
		public string Name;
		public string Type;
		public string Range;
		public int Damage;		
	}

	private static Dictionary<string, CardData> cards;


	public static CardData GetCardInfo(string cardName)
	{
		return cards[cardName];
	}


	public static string GetCardTexturePath(string nation, string cardName)
	{
		return "res://Assets/Cards/" + nation + "/" + CardDataBase.cards[cardName].Type + "/" + cardName + ".png";
	}


	public static void UpdateCardDataBase()
	{
		cards = new();		

		cards.Add("Card1", new CardData { Name = "Card1", Type = "Squad", Damage = 30 , Range = "Melee"});
		cards.Add("Card2", new CardData { Name = "Card2", Type = "Squad", Damage = 20 , Range = "Melee"});
		cards.Add("Card3", new CardData { Name = "Card3", Type = "Squad", Damage = 30 , Range = "Siege"});
		cards.Add("Card4", new CardData { Name = "Card4", Type = "Squad", Damage = 20 , Range = "Siege"});
		cards.Add("Card5", new CardData { Name = "Card5", Type = "Squad", Damage = 20 , Range = "Melee"});
		cards.Add("Card6", new CardData { Name = "Card6", Type = "Squad", Damage = 20 , Range = "LongRange"});
		cards.Add("Card0", new CardData { Name = "Card0", Type = "Leader", Damage = 20, Range = "" });
	}
}
