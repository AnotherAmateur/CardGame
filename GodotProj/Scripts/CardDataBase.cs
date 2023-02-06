using Godot;
using System;
using System.Collections.Generic;

public class CardDataBase
{
	public struct CardData
	{
		public string Name;
		public string Type;
		public int Damage;
	}

	Dictionary<string, CardData> cards;
	public Dictionary<string, CardData> Cards { get { return cards; } }

	public CardDataBase()
	{
		cards = new();
		cards.Add("card1", new CardData { Name = "card1", Type = "leader", Damage = 30 });
		cards.Add("card2", new CardData { Name = "card2", Type = "leader", Damage = 20 });
	}
}
