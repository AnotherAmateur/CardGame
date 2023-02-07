using Godot;
using System.Collections.Generic;

public class GameFieldController : Node2D
{
	private PackedScene cardScene = (PackedScene)GD.Load("res://CardScene.tscn");

	public static GameFieldController Instance { get; private set; }
	private static Player player;

	private List<string> selectedCards = new();
	private string selectedNation = "Nation1";
	private Control CardsControlNode;
	private int maxHandSize = 10;

	public override void _Ready()
	{
		Instance = this;
		CardsControlNode = GetNode<Control>("Cards");

		selectedCards.Add("Card1");
		selectedCards.Add("Card2");
		selectedCards.Add("Card3");
		selectedCards.Add("Card4");
		selectedCards.Add("Card5");
		selectedCards.Add("Card6");
		selectedCards.Add("Card0");

		player = new(selectedCards, selectedNation);
		player.TakeCardsFromDeck(selectedCards);
		CardDataBase.UpdateCardDataBase();
		UpdateHand();

		GetNode<Label>("DeckSizeBottom").Text = player.Deck.Count.ToString();
	}


	private void UpdateHand()
	{
		foreach (Node2D node in CardsControlNode.GetChildren())
		{
			CardsControlNode.RemoveChild(node);
		}

		Vector2 cardSize = new(CardsControlNode.RectSize.x / maxHandSize, CardsControlNode.RectSize.y);
		float nextCardPosition = (CardsControlNode.RectSize.x - cardSize.x * player.Hand.Count) / 2;
		for (int i = 0; i < player.Hand.Count; i++)
		{
			string cardName = player.Hand[i];
			string texturePath = "res://Assets/Cards/" + player.Nation + "/" + CardDataBase.Cards[cardName].Type + "/" + cardName + ".png";

			CardScene cardInstance = (CardScene)cardScene.Instance();
			CardsControlNode.AddChild(cardInstance);

			var card = CardsControlNode.GetChild<CardScene>(i);
			card.SetParams(cardName, cardSize, texturePath);

			card.Position = new Vector2(nextCardPosition, 0);
			nextCardPosition += cardSize.x;
		}
	}


	public void CardSceneEventHandler(string eventName, string cardName)
	{
		if (eventName == "pressed")
		{
			player.DiscardCard(cardName);
			UpdateHand();
		}
	}



}
