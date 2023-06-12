using CardGameProj.Scripts;
using CardGameProj.SeparateClasses;
using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class CardSelectionMenu : Control, ISocketConn
{
	public static CardSelectionMenu Instantiate { get; private set; }
	public static List<int> SelectedCards { get; private set; } = new();
	public static List<int> LeaderCards { get; private set; } = new();
	public static List<CardNations> Nations { get; private set; } = new();
	public static CardNations Nation { get; private set; }
	public int LeaderCard
	{
		get { return States.ProtagonistLeaderCardId; }
		private set { States.ProtagonistLeaderCardId = value; }
	}

	private PackedScene cardScene = (PackedScene)GD.Load("res://SlaveCardScene.tscn");
	private GridContainer allCardsGridContainer;
	private GridContainer selectedCardsGridContainer;

	private int minHandSize;
	private Vector2 cardSize;
	private bool isReadyHere;
	private bool isReadyThere;

	SocketConnection socketConnection;

	public override void _Ready()
	{
		socketConnection = SocketConnection.GetInstance(this);

		Instantiate = this;
		minHandSize = 1;
		allCardsGridContainer = GetNode<ScrollContainer>("AllCardScrollContainer").GetNode<GridContainer>("GridContainer");
		selectedCardsGridContainer = GetNode<ScrollContainer>("SelectionCardScrollContainer").GetNode<GridContainer>("GridContainer");

		var cardContainerSize = GetNode<ScrollContainer>("AllCardScrollContainer").Size;

		cardSize = new Vector2(185f, 277.5f);

		CardDataBase.UpdateCardDataBase();
		GetNode<Label>("VBoxContainer/HBoxContainer/TotalCards").Text = CardDataBase.GetAllCards.Count.ToString();
		LoadAllCards();
		ChangeLeader();
	}

	private void LoadAllCards()
	{
		int nationCardsCount = 0;

		foreach (var card in CardDataBase.GetAllCards.Where(x =>
			x.Value.nation == CardDataBase.GetCardInfo(LeaderCard).nation || x.Value.type == CardTypes.Leader))
		{
			if (card.Value.type == CardTypes.Leader)
			{
				if (LeaderCards.Contains(card.Key) is false)
				{
					Nations.Add(card.Value.nation);
					LeaderCards.Add(card.Key);
				}
			}
			else
			{
				SlaveCardScene cardInstance = (SlaveCardScene)cardScene.Instantiate(PackedScene.GenEditState.Instance);
				Control t = new();
				t.Name = card.Key.ToString();
				allCardsGridContainer.AddChild(t);
				allCardsGridContainer.GetNode<Control>(card.Key.ToString()).AddChild(cardInstance);

				string texturePath = CardDataBase.GetCardTexturePath(card.Key);

				allCardsGridContainer.GetNode<Control>(card.Key.ToString()).GetChild<SlaveCardScene>(0)
					.SetParams(cardSize, texturePath, CardDataBase.GetCardInfo(card.Key));

				++nationCardsCount;
			}
		}

		GetNode<Label>("VBoxContainer/HBoxContainer2/TotalNationCards").Text = nationCardsCount.ToString();

		for (int i = 0; i < allCardsGridContainer.Columns; i++)
		{
			allCardsGridContainer.AddChild(new Control());
			selectedCardsGridContainer.AddChild(new Control());
		}
	}

	private void ChangeLeader()
	{
		SelectedCards = new();

		int index = LeaderCards.IndexOf(LeaderCard);
		if (LeaderCards.Count == index + 1)
		{
			LeaderCard = LeaderCards.First();
		}
		else
		{
			LeaderCard = LeaderCards[++index];
		}

		var leaderContainer = GetNode<Control>("NationInfoPanel").GetNode("LeaderContainer");

		foreach (Node node in leaderContainer.GetChildren())
		{
			leaderContainer.RemoveChild(node);
		}

		foreach (Node node in selectedCardsGridContainer.GetChildren())
		{
			selectedCardsGridContainer.RemoveChild(node);
		}

		foreach (Node node in allCardsGridContainer.GetChildren())
		{
			allCardsGridContainer.RemoveChild(node);
		}

		LoadAllCards();

		SlaveCardScene cardInstance = (SlaveCardScene)cardScene.Instantiate();
		leaderContainer.AddChild(cardInstance);
		leaderContainer.GetChild<SlaveCardScene>(0).SetParams(cardSize,
			CardDataBase.GetCardTexturePath(LeaderCard), CardDataBase.GetCardInfo(LeaderCard));
	}

	public void CardSceneEventHandler(CardEvents cardEvent, int cardId)
	{
		if (cardEvent is CardEvents.LeftCllick)
		{
			if (CardDataBase.GetCardInfo(cardId).type == CardTypes.Leader)
			{
				ChangeLeader();
			}
			else if (SelectedCards.Contains(cardId))
			{
				Control temp = selectedCardsGridContainer.GetNode<Control>(cardId.ToString());
				selectedCardsGridContainer.RemoveChild(selectedCardsGridContainer.GetNode<Control>(cardId.ToString()));
				allCardsGridContainer.AddChild(temp);
				allCardsGridContainer.MoveChild(temp, 0);

				SelectedCards.Remove(cardId);
			}
			else
			{
				Control temp = allCardsGridContainer.GetNode<Control>(cardId.ToString());
				allCardsGridContainer.RemoveChild(allCardsGridContainer.GetNode<Control>(cardId.ToString()));
				selectedCardsGridContainer.AddChild(temp);
				selectedCardsGridContainer.MoveChild(temp, 0);

				SelectedCards.Add(cardId);
			}

			GetNode<Label>("LabelsContainer/HBoxContainer/LabelCountInfo").Text = SelectedCards.Count().ToString();
		}
	}

	private void _on_StartButton_pressed()
	{
		if (SelectedCards.Count() >= minHandSize)
		{
			isReadyHere = true;
			socketConnection.Send(ActionTypes.Ready, States.MasterId, LeaderCard.ToString());

			if (isReadyThere == true)
			{
				GetTree().ChangeSceneToPacked((PackedScene)GD.Load("res://GameFieldScreen.tscn"));
			}
			else
			{
				PackedScene messageBoxScene = (PackedScene)GD.Load("res://message_box.tscn");
				MessageBox messageBox = (MessageBox)messageBoxScene.Instantiate(PackedScene.GenEditState.Instance);
				messageBox.SetUp("Рука зафиксирована \n Ожидание готовности оппонента", false);
				AddChild(messageBox);
			}
		}
	}

	public void OnHandleError(string exMessage)
	{
		OS.Alert("CardSelectionMenu/OnHandleError");
	}

	public void OnReceiveMessage(string action, string masterId, string message)
	{
		if (action == ActionTypes.Ready.ToString())
		{
			isReadyThere = true;
			States.AntagonistLeaderCardId = int.Parse(message);

			if (isReadyHere == true)
			{
				GetTree().ChangeSceneToPacked((PackedScene)GD.Load("res://GameFieldScreen.tscn"));
			}
		}
	}

	private void LoadCards(CardTypes cardType)
	{
		foreach (Node node in selectedCardsGridContainer.GetChildren())
		{
			selectedCardsGridContainer.RemoveChild(node);
		}

		foreach (Node node in allCardsGridContainer.GetChildren())
		{
			allCardsGridContainer.RemoveChild(node);
		}

		IEnumerable<KeyValuePair<int, CardDataBase.CardData>> cards;
		if (cardType == CardTypes.Leader)
		{
			cards = CardDataBase.GetAllCards.Where(x =>
			x.Value.nation == CardDataBase.GetCardInfo(LeaderCard).nation && x.Value.type != cardType);
		}
		else
		{
			cards = CardDataBase.GetAllCards.Where(x =>
		x.Value.nation == CardDataBase.GetCardInfo(LeaderCard).nation && x.Value.type == cardType);
		}

		foreach (var card in cards.Where(x => SelectedCards.Contains(x.Key) is false))
		{
			SlaveCardScene cardInstance = (SlaveCardScene)cardScene.Instantiate(PackedScene.GenEditState.Instance);
			Control t = new();
			t.Name = card.Key.ToString();
			allCardsGridContainer.AddChild(t);
			allCardsGridContainer.GetNode<Control>(card.Key.ToString()).AddChild(cardInstance);

			string texturePath = CardDataBase.GetCardTexturePath(card.Key);

			allCardsGridContainer.GetNode<Control>(card.Key.ToString()).GetChild<SlaveCardScene>(0)
				.SetParams(cardSize, texturePath, CardDataBase.GetCardInfo(card.Key));
		}

		foreach (var card in cards.Where(x => SelectedCards.Contains(x.Key) is true))
		{
			SlaveCardScene cardInstance = (SlaveCardScene)cardScene.Instantiate(PackedScene.GenEditState.Instance);
			Control t = new();
			t.Name = card.Key.ToString();
			selectedCardsGridContainer.AddChild(t);
			selectedCardsGridContainer.GetNode<Control>(card.Key.ToString()).AddChild(cardInstance);

			string texturePath = CardDataBase.GetCardTexturePath(card.Key);

			selectedCardsGridContainer.GetNode<Control>(card.Key.ToString()).GetChild<SlaveCardScene>(0)
				.SetParams(cardSize, texturePath, CardDataBase.GetCardInfo(card.Key));
		}

		for (int i = 0; i < allCardsGridContainer.Columns; i++)
		{
			allCardsGridContainer.AddChild(new Control());
			selectedCardsGridContainer.AddChild(new Control());
		}
	}

	private void _on_all_cards_btn_pressed()
	{
		LoadCards(CardTypes.Leader);
	}

	private void _on_group_1_btn_pressed()
	{
		LoadCards(CardTypes.Group1);
	}

	private void _on_group_2_btn_pressed()
	{
		LoadCards(CardTypes.Group2);
	}

	private void _on_group_3_btn_pressed()
	{
		LoadCards(CardTypes.Group3);
	}

	private void _on_special_cards_btn_pressed()
	{
		LoadCards(CardTypes.Special);
	}

	private void _on_take_all_cards_btn_pressed()
	{
		foreach (Control cardHolder in allCardsGridContainer.GetChildren())
		{
			foreach (SlaveCardScene card in cardHolder.GetChildren())
			{
				card._on_Card_pressed();
			}
		}
	}

	private void _on_drop_all_cards_btn_pressed()
	{
		foreach (Control cardHolder in selectedCardsGridContainer.GetChildren())
		{
			foreach (SlaveCardScene card in cardHolder.GetChildren())
			{
				card._on_Card_pressed();
			}
		}
	}
}



