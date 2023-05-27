using CardGameProj.Scripts;
using Godot;
using System;

public class GameFieldController : Node2D
{
	public static PackedScene SlaveCardScene = (PackedScene)GD.Load("res://SlaveCardScene.tscn");
	public static GameFieldController Instance { get; private set; }
	protected Control leaderCardContainerBottom;
	protected Control leaderCardContainerTop;

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

		protagonist.TakeCardsFromDeck(protagonist.GetRandomCardsFromDeck(CardSelectionMenu.SelectedCards.Count));		
		//protagonist.TakeCardsFromDeck(protagonist.GetRandomCardsFromDeck(Player.MaxHandSize));

		protagonist.SetLeaderCard(CardSelectionMenu.LeaderCard);

		antagonist = new(CardSelectionMenu.LeaderCard, cardRowsContainerTop, leaderCardContainerTop, 
			discardPileContainerTop, totalCountBottom, RowsCountTopContainer);


		//GetNode<Label>("DeckSizeBottom").Text = player.Deck.Count.ToString();
	}


	public void CardSceneEventHandler(string eventName, int cardId)
	{
		if (eventName == "pressed")
		{
			if (CardDataBase.GetCardInfo(cardId).type == CardTypes.Leader)
			{
				foreach (var item in leaderCardContainerBottom.GetChildren())
				{
					GD.Print("Xui" + item.ToString());
				}
				

				leaderCardContainerBottom.GetChild<SlaveCardScene>(0).DisableCardButton();

				//leaderCardContainerTop.GetChild<SlaveCardScene>(0).DisableCardButton();
			}
			else if (protagonist.Hand.Contains(cardId))
			{
				protagonist.PutCardFromHandOnBoard(cardId);


				var text = JSON.Print("12sd");

				HTTPRequest httpRequest = GetNode<HTTPRequest>("HTTPRequest");

				string[] headers = new string[] { "Content-Type: application/json" };


				try
				{
					string url = "https://localhost:7135/api/Game";
					httpRequest.Request(url, headers, false, HTTPClient.Method.Post, text);
					
				}
				catch (Exception ex)
				{
					GD.Print(ex.Message);
				}
				

				antagonist.PutCardOnBoard(cardId);
			}
		}
	}


	private void _on_Pass_pressed()
	{
		antagonist.DoPass();
		protagonist.DoPass();
	}

private void _on_HTTPRequest_request_completed(int result, int response_code, string[] headers, byte[] body)
{
	
	if (response_code == 200)
	{
		JSONParseResult json = JSON.Parse(System.Text.Encoding.UTF8.GetString(body));
		GD.Print("we got it" +json.Result);
	}
	else
	{
	   GD.Print("No");
	}
}
}





