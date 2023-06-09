using CardGameProj.Scripts;
using CardGameProj.SeparateClasses;
using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;

public partial class GameFieldController : Node2D, ISocketConn
{
	public static PackedScene MinCardScene = (PackedScene)GD.Load("res://MinCardScene.tscn");
	private PackedScene cardScene = (PackedScene)GD.Load("res://SlaveCardScene.tscn");
	public static GameFieldController Instance { get; private set; }
	protected Control leaderCardContainerBottom;
	protected Control leaderCardContainerTop;
	private Control largeCardContainer;
	private Protagonist protagonist;
	private Antagonist antagonist;
	private SocketConnection socketConnection;
	private GameFieldController() { }
	private Button passBtn;

	public override void _Ready()
	{
		Instance = this;
		socketConnection = SocketConnection.GetInstance(this);

		InitializeColorRects();

		passBtn = GetNode<Button>("ToPass");

		Control cardsHandContainer = GetNode<Control>("Cards");

		Control discardPileContainerTop = GetNode<Control>("DiscardPileContainerTop");
		Control discardPileContainerBottom = GetNode<Control>("DiscardPileContainerBottom");

		Control cardRowsContainerTop = GetNode<Control>("FieldRowsContainer").GetNode<Control>("Top");
		Control cardRowsContainerBottom = GetNode<Control>("FieldRowsContainer").GetNode<Control>("Bottom");

		Label totalCountTop = GetNode<Label>("TotalCount/VBoxContainer/Top");
		Label totalCountBottom = GetNode<Label>("TotalCount/VBoxContainer/Bottom");

		Control RowsCountTopContainer = GetNode<Control>("RowsCount").GetNode<Control>("Top");
		Control RowsCountBottomContainer = GetNode<Control>("RowsCount").GetNode<Control>("Bottom");

		leaderCardContainerBottom = GetNode<Control>("LeaderCardContainerBottom");
		leaderCardContainerTop = GetNode<Control>("LeaderCardContainerTop");

		largeCardContainer = GetNode<Control>("LargeCardContainer");

		var roundVBoxTop = GetNode<HBoxContainer>("TotalCount/VBoxContainer/TopRoundCount");
		var roundVBoxBottom = GetNode<HBoxContainer>("TotalCount/VBoxContainer/BottomRoundCount");

		CardDataBase.UpdateCardDataBase();

		antagonist = new(States.AntagonistLeaderCardId, cardRowsContainerTop, leaderCardContainerTop,
			discardPileContainerTop, totalCountTop, RowsCountTopContainer, roundVBoxTop);

		protagonist = new(CardSelectionMenu.LeaderCard, CardSelectionMenu.SelectedCards,
			cardRowsContainerBottom, cardsHandContainer, leaderCardContainerBottom,
			discardPileContainerBottom, totalCountBottom, RowsCountBottomContainer, roundVBoxBottom);

		protagonist.TakeCardsFromDeck(protagonist.GetRandomCardsFromDeck(
			Math.Min(Player.MaxHandSize, CardSelectionMenu.SelectedCards.Count)));

		//GetNode<Label>("DeckSizeBottom").Text = player.Deck.Count.ToString();
	}

	public void CardSceneEventHandler(CardEvents cardEvent, int cardId)
	{
		if (cardEvent is CardEvents.LeftCllick && passBtn.Disabled is false)
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
		passBtn.Disabled = (antagonist.IsPass) ? false : true;
		socketConnection.Send(ActionTypes.Pass, States.MasterId, String.Empty);
		protagonist.DoPass();	
	}

	public void OnHandleError(string exMessage)
	{
		//OS.Alert(String.Join("\n", "GameFieldController/OnReceiveMessage: ", exMessage));
	}

	public void OnReceiveMessage(string action, string masterId, string message)
	{
		if (action == ActionTypes.CardMove.ToString())
		{
			antagonist.PutCardOnBoard(int.Parse(message));
		}
		else if (action == ActionTypes.Pass.ToString())
		{
			antagonist.DoPass();
			passBtn.Disabled = false;
		}
		else
		{
			OS.Alert(String.Join("\n", "GameFieldController/OnReceiveMessage: ", action, masterId, message));
		}
	}

	public void MatchCompleted(Player winner)
	{
		string declareWinner = "";

		if (winner == protagonist)
		{
			declareWinner = "Победа за вами!";
		}
		else if (winner == antagonist)
		{
			declareWinner = "Победа за оппонентом!";
		}
		else
		{
			declareWinner = "Ничья!";
		}

		socketConnection.Disconnect();

		PackedScene messageBoxScene = (PackedScene)GD.Load("res://message_box.tscn");
		MessageBox messageBox = (MessageBox)messageBoxScene.Instantiate(PackedScene.GenEditState.Instance);
		messageBox.SetUp(declareWinner, true, true);
		GetNode("Shape").AddChild(messageBox);
	}

	private void InitializeColorRects()
	{
		var color = new Godot.Color("#988153");

		var controls = new List<Control>();
		controls.Add(GetNode<Control>("LeaderCardContainerTop"));
		controls.Add(GetNode<Control>("LeaderCardContainerBottom"));
		controls.Add(GetNode<Control>("Cards"));
		controls.Add(GetNode<Control>("LargeCardContainer"));

		float rowHeight = 127f;
		float containerOffsetX = 524f;
		float topMargin = GetNode<Control>("FieldRowsContainer").Position.Y;
		var rowSize = new Vector2(GetNode<VBoxContainer>("FieldRowsContainer/Top").Size.X, rowHeight);
		for (int i = 1; i <= 3; i++)
		{
			var offset = new Vector2(containerOffsetX, topMargin + (i - 1) * (4 + rowHeight));
			var item = GetNode<Control>("FieldRowsContainer/Top/Row" + i.ToString());
			var t = new ColorRect();
			t.Size = rowSize;
			t.Position = new Vector2(item.Position.X + offset.X, item.Position.Y + offset.Y);
			t.Color = color;
			t.MouseFilter = Control.MouseFilterEnum.Ignore;
			AddChild(t);

			item = GetNode<Control>("FieldRowsContainer/Bottom/Row" + i.ToString());
			offset = new Vector2(containerOffsetX, topMargin + 397 + (i - 1) * (4 + rowHeight));
			t = new ColorRect();
			t.Size = rowSize;
			t.Position = new Vector2(item.Position.X + offset.X, item.Position.Y + offset.Y);
			t.Color = color;
			t.MouseFilter = Control.MouseFilterEnum.Ignore;
			AddChild(t);
		}

		const int margin = 16;
		foreach (var item in controls)
		{
			var t = new ColorRect();
			t.Size = new Vector2(item.Size.X + margin, item.Size.Y + margin);
			t.Position = new Vector2(item.Position.X - margin / 2, item.Position.Y - margin / 2);
			t.Color = color;
			t.MouseFilter = Control.MouseFilterEnum.Ignore;
			AddChild(t);
		}
	}
}


