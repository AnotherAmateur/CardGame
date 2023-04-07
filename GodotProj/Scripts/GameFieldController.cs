using Godot;
using System;
using System.Collections.Generic;

public class GameFieldController : Node2D
{
	public static PackedScene CardScene = (PackedScene)GD.Load("res://CardScene.tscn");
	public static GameFieldController Instance { get; protected set; }
	private Control leaderCardContainerBottom;
	private Control leaderCardContainerTop;

	private Protagonist protagonist;
	private Antagonist antagonist;


	public override void _Ready()
	{
		Instance = this;

		Control cardsHandContainer = GetNode<Control>("Cards");

		Control discardPileContainerTop = GetNode<Control>("DiscardPileContainerTop");
		Control discardPileContainerBottom = GetNode<Control>("DiscardPileContainerBottom");

		Control cardRowsContainerTop = GetNode<Control>("FieldRowsContainer").GetNode<Control>("Top");
		Control cardRowsContainerBottom = GetNode<Control>("FieldRowsContainer").GetNode<Control>("Bottom");

		Label totalCountTop = GetNode<Control>("TotalCount").GetNode<Label>("Top");
		Label totalCountBottom = GetNode<Control>("TotalCount").GetNode<Label>("Bottom");

		Control RowsCountTopContainer = GetNode<Control>("RowsCount").GetNode<Control>("Top");
		Control RowsCountBottomContainer = GetNode<Control>("RowsCount").GetNode<Control>("Bottom");

		leaderCardContainerBottom = GetNode<Control>("LeaderCardContainerBottom");
		leaderCardContainerTop = GetNode<Control>("LeaderCardContainerTop");


		CardDataBase.UpdateCardDataBase();

		protagonist = new(CardSelectionMenu.LeaderCard, CardSelectionMenu.SelectedCards,
			cardRowsContainerBottom, cardsHandContainer, leaderCardContainerBottom,
			discardPileContainerBottom, totalCountTop, RowsCountBottomContainer);
		protagonist.TakeCardsFromDeck(protagonist.GetRandomCardsFromDeck(Player.maxHandSize));
		protagonist.SetLeaderCard(CardSelectionMenu.LeaderCard);

		antagonist = new(CardSelectionMenu.LeaderCard, cardRowsContainerTop, leaderCardContainerTop, 
			discardPileContainerTop, totalCountBottom, RowsCountTopContainer);


		//GetNode<Label>("DeckSizeBottom").Text = player.Deck.Count.ToString();
	}


	public void CardSceneEventHandler(string eventName, string cardName)
	{
		if (eventName == "pressed")
		{
			if (CardDataBase.GetCardInfo(cardName).Type == "Leader")
			{
				leaderCardContainerBottom.GetChild<CardScene>(0).DisableCardButton();

				leaderCardContainerTop.GetChild<CardScene>(0).DisableCardButton();
			}
			else if (protagonist.Hand.Contains(cardName))
			{
				protagonist.PutCardFromHandOnBoard(cardName);

				antagonist.PutCardOnBoard(cardName);
			}
		}
	}


	private void _on_Pass_pressed()
	{
		antagonist.DoPass();
		protagonist.DoPass();
	}

}
