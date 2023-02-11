using Godot;
using System;
using System.Collections.Generic;

public class GameFieldController : Node2D
{
	public static PackedScene CardScene = (PackedScene)GD.Load("res://CardScene.tscn");
	public static GameFieldController Instance { get; protected set; }

	private Protagonist protagonist;
	private Antagonist antagonist;


	public override void _Ready()
	{
		Instance = this;
		Control cardsHandContainer = GetNode<Control>("Cards");
		Control cardRowsContainerTop = GetNode<Control>("FieldRowsContainer").GetNode<Control>("Top");
		Control cardRowsContainerBottom = GetNode<Control>("FieldRowsContainer").GetNode<Control>("Bottom");
		Control leaderCardContainerBottom = GetNode<Control>("LeaderCardContainerBottom");
		Control leaderCardContainerTop = GetNode<Control>("LeaderCardContainerTop");

		CardDataBase.UpdateCardDataBase();

		protagonist = new(CardSelectionMenu.LeaderCard, CardSelectionMenu.SelectedCards,
			cardRowsContainerBottom, cardsHandContainer, leaderCardContainerBottom);
		protagonist.TakeCardsFromDeck(protagonist.GetRandomCardsFromDeck(2));//Player.maxHandSize));
		protagonist.SetLeaderCard(CardSelectionMenu.LeaderCard);


		//GetNode<Label>("DeckSizeBottom").Text = player.Deck.Count.ToString();
	}


	public void CardSceneEventHandler(string eventName, string cardName)
	{
		if (eventName == "pressed")
		{
			if (CardDataBase.GetCardInfo(cardName).Type == "Leader")
			{
				GetNode<Control>("LeaderCardContainerBottom").GetChild<CardScene>(0).DisableCardButton();
			}
			else if (protagonist.Hand.Contains(cardName))
			{
				protagonist.PutCardFromHandOnBoard(cardName);
			}
		}
	}

}
