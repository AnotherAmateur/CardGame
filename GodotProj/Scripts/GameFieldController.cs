using CardGameProj.Scripts;
using CardGameProj.SeparateClasses;
using Godot;
using System;
using System.Diagnostics;
using System.Text;

public partial class GameFieldController : Node2D, ISocketConn
{
	public static PackedScene MinCardScene = (PackedScene)GD.Load("res://MinCardScene.tscn");
	private PackedScene cardScene = (PackedScene)GD.Load("res://SlaveCardScene.tscn");
	public static GameFieldController Instantiate { get; private set; }
	protected Control leaderCardContainerBottom;
	protected Control leaderCardContainerTop;
	private Control largeCardContainer;

	private Protagonist protagonist;
	private Antagonist antagonist;

	private SocketConnection socketConnection;

	private GameFieldController() { }

	public override void _Ready()
	{
		Instantiate = this;
		socketConnection = SocketConnection.GetInstance(this);

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

		largeCardContainer = GetNode<Control>("LargeCardContainer");

		CardDataBase.UpdateCardDataBase();

		protagonist = new(CardSelectionMenu.LeaderCard, CardSelectionMenu.SelectedCards,
			cardRowsContainerBottom, cardsHandContainer, leaderCardContainerBottom,
			discardPileContainerBottom, totalCountTop, RowsCountBottomContainer);

		protagonist.TakeCardsFromDeck(protagonist.GetRandomCardsFromDeck(
			Math.Min(Player.MaxHandSize, CardSelectionMenu.SelectedCards.Count)));

		antagonist = new(States.AntagonistLeaderCardId, cardRowsContainerTop, leaderCardContainerTop,
			discardPileContainerTop, totalCountBottom, RowsCountTopContainer);

		//GetNode<Label>("DeckSizeBottom").Text = player.Deck.Count.ToString();
	}


	public void CardSceneEventHandler(CardEvents cardEvent, int cardId)
	{
		if (cardEvent is CardEvents.LeftCllick)
		{
			if (CardDataBase.GetCardInfo(cardId).type == CardTypes.Leader)
			{

			}
			else if (protagonist.Hand.Contains(cardId))
			{
				protagonist.PutCardFromHandOnBoard(cardId);

				socketConnection.Send(ActionTypes.CardMove, States.MasterId, cardId.ToString());




			}
		}
		else if (cardEvent is CardEvents.RightClick)
		{
			foreach (var childNode in largeCardContainer.GetChildren())
			{
				largeCardContainer.RemoveChild(childNode);
			}
			
			SlaveCardScene cardInstance = (SlaveCardScene)cardScene.Instantiate(PackedScene.GenEditState.Instance);
			largeCardContainer.AddChild(cardInstance);
			cardInstance.SetParams(largeCardContainer.Size, CardDataBase.GetCardTexturePath(cardId),
				CardDataBase.GetCardInfo(cardId), disconnectSignals: true);
		}
	}



	private void _on_Pass_pressed()
	{
		antagonist.DoPass();






	}

	public void OnHandleError(string exMessage)
	{
		OS.Alert(String.Join("\n", "GameFieldController/OnReceiveMessage: ", exMessage));
	}

	public void OnReceiveMessage(string action, string masterId, string message)
	{
		if (action == ActionTypes.CardMove.ToString())
		{
			antagonist.PutCardOnBoard(int.Parse(message));
		}
		else if (action == ActionTypes.Pass.ToString())
		{

		}
		else
		{
			OS.Alert(String.Join("\n", "GameFieldController/OnReceiveMessage: ", action, masterId, message));
		}
	}
}


