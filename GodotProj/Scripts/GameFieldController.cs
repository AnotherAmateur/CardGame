using Godot;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class GameFieldController : Node2D
{
	private PackedScene cardScene = (PackedScene)GD.Load("res://CardScene.tscn");

	public static GameFieldController Instance { get; private set; }
	private static Player player;

	private Control CardsHandContainer;
	private Control CardRowsContainer;
	private static int maxHandSize = 10;

	public override void _Ready()
	{
		Instance = this;
		CardsHandContainer = GetNode<Control>("Cards");
		CardRowsContainer = GetNode<Control>("FieldRowsContainer");		

		player = new(CardSelectionMenu.SelectedCards, CardSelectionMenu.Nation, CardSelectionMenu.LeaderCard);
		player.TakeCardsFromDeck(CardSelectionMenu.SelectedCards);
		CardDataBase.UpdateCardDataBase();

		UpdateHand();
		SetLeaderCard(CardSelectionMenu.LeaderCard);

		GetNode<Label>("DeckSizeBottom").Text = player.Deck.Count.ToString();
	}


	private void UpdateHand()
	{
		foreach (Node2D node in CardsHandContainer.GetChildren())
		{
			CardsHandContainer.RemoveChild(node);
		}

		Vector2 cardSize = new(CardsHandContainer.RectSize.x / maxHandSize, CardsHandContainer.RectSize.y);
		float nextCardPosition = (CardsHandContainer.RectSize.x - cardSize.x * player.Hand.Count) / 2;
		for (int i = 0; i < player.Hand.Count; i++)
		{
			string cardName = player.Hand[i];

			CardScene cardInstance = (CardScene)cardScene.Instance();
			CardsHandContainer.AddChild(cardInstance);

			var card = CardsHandContainer.GetChild<CardScene>(i);
			card.SetParams(cardName, cardSize, CardDataBase.GetCardTexturePath(player.Nation, cardName));

			card.Position = new Vector2(nextCardPosition, 0);
			nextCardPosition += cardSize.x;
		}
	}


	private void SetLeaderCard(string cardName)
	{
		CardScene cardInstance = (CardScene)cardScene.Instance();
		GetNode<Control>("LeaderCard").AddChild(cardInstance);
		GetNode<Control>("LeaderCard").GetChild<CardScene>(0).SetParams(cardName, GetNode<Control>("LeaderCard").RectSize, CardDataBase.GetCardTexturePath(player.Nation, cardName));
	}


	private void UpdateBoard()
	{
		foreach (Node row in CardRowsContainer.GetChildren())
		{
			foreach (Node node in row.GetChildren())
			{
				if (node.Name.Contains("Antagonist") is false)
				{
					row.RemoveChild(node);
				}
			}
		}

		Vector2 rowRectSize = CardRowsContainer.GetChild<Control>(0).RectSize;
		Vector2 cardSize = new(rowRectSize.x / maxHandSize, rowRectSize.y);
		Dictionary<string, List<string>> rangeSortedCards = new();
		foreach (string card in player.OnBoard)
		{
			if (rangeSortedCards.ContainsKey(CardDataBase.GetCardInfo(card).Range) is false)
			{
				rangeSortedCards.Add(CardDataBase.GetCardInfo(card).Range, new());
			}

			rangeSortedCards[CardDataBase.GetCardInfo(card).Range].Add(card);
		}

		foreach (var range in rangeSortedCards)
		{
			Control row = CardRowsContainer.GetNode<Control>("Row" + range.Key);
			float nextXCardPosition = (rowRectSize.x - cardSize.x * range.Value.Count) / 2;
			foreach (string cardName in range.Value)
			{
				CardScene cardInstance = (CardScene)cardScene.Instance();
				row.AddChild(cardInstance);
				cardInstance.Name = cardName;

				var card = row.GetNode<CardScene>(cardName);
				card.SetParams(cardName, cardSize, CardDataBase.GetCardTexturePath(player.Nation, cardName));
				card.Position = new Vector2(nextXCardPosition, 0);
				nextXCardPosition += cardSize.x;
			}
		}
	}


	public void CardSceneEventHandler(string eventName, string cardName)
	{
		if (eventName == "pressed")
		{
			if (CardDataBase.GetCardInfo(cardName).Type == "Leader")
			{
				GetNode<Control>("LeaderCard").GetChild<CardScene>(0).DisableCardButton();
			}
			else if (player.Hand.Contains(cardName))
			{
				player.DiscardCard(cardName);
				UpdateHand();
				UpdateBoard();
			}
		}
	}
}
