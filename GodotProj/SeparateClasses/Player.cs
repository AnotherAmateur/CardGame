using CardGameProj.Scripts;
using CardGameProj.SeparateClasses;
using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public abstract class Player
{
	private Label TotalCountLabel;
	protected Control CardRowsContainer;
	protected Control LeaderCardContainer;
	protected Control DiscardPileContainer;
	protected Control RowsCountContainer;
	private HBoxContainer RoundVBoxContainer;
	private VBoxContainer SpecialCardsContainer;
	private Control TemporalSpCardContainer;

	private static int round;
	public const int MaxHandSize = 10;
	protected int DiscardPileFlippedcardId;
	public bool IsPass { get; protected set; }
	private static int gameResult;

	public List<int> DiscardPile { get; protected set; }
	public List<int> OnBoard { get; protected set; }
	public int LeaderCard { get; protected set; }
	public int TotalCount
	{
		get { return int.Parse(TotalCountLabel.Text); }
		private set { TotalCountLabel.Text = value.ToString(); }
	}


	public Player(int leaderCard, Control cardRowsContainer, Control leaderCardContainer,
		Control discardPileContainer, Label totalCount, Control rowsCountContainer, HBoxContainer roundVBoxContainer)
	{
		DiscardPile = new();
		OnBoard = new();
		LeaderCard = leaderCard;
		CardRowsContainer = cardRowsContainer;
		LeaderCardContainer = leaderCardContainer;
		DiscardPileContainer = discardPileContainer;
		TotalCountLabel = totalCount;
		RoundVBoxContainer = roundVBoxContainer;
		round = 1;
		gameResult = 0;
		SpecialCardsContainer = GameFieldController.Instance.SpecialCardsContainer;
		TemporalSpCardContainer = GameFieldController.Instance.TemporalSpCardContainer;

		RowsCountContainer = rowsCountContainer;

		SetLeaderCard(leaderCard);

		UpdateRowsCount();
		UpdateTotalCount();
	}

	private void UpdateTotalCount()
	{
		int totalCount = 0;
		foreach (Label rowCount in RowsCountContainer.GetChildren())
		{
			totalCount += int.Parse((rowCount.Text == "") ? "0" : rowCount.Text);
		}

		TotalCount = totalCount;
	}

	private void UpdateRowsCount()
	{
		const int oneSideRowsNumber = 3;
		for (int i = 1; i <= oneSideRowsNumber; i++)
		{
			int sum = 0;
			int rowCardCount = 0;
			foreach (MinCardScene card in CardRowsContainer.GetNode<Control>("Row" + i)
				.GetChildren().Where(it => it.Name != "Label"))
			{
				sum += card.CardDamage;
				++rowCardCount;
			}


			int addStrength = 0;
			var t = SpecialCardsContainer.GetNode<Control>((this is Protagonist) ? "BottomRow" + i : "TopRow" + i);
			foreach (MinCardScene spCard in t.GetChildren())
			{
				addStrength += spCard.CardDamage * rowCardCount;
			}

			RowsCountContainer.GetNode<Label>("Row" + i).Text = (sum + addStrength).ToString();
		}
	}

	public void SetLeaderCard(int cardId)
	{
		MinCardScene cardInstance = (MinCardScene)GameFieldController.MinCardScene.Instantiate();
		LeaderCardContainer.AddChild(cardInstance);
		cardInstance.SetParams(LeaderCardContainer.Size,
			CardDataBase.GetCardTexturePath(cardId), CardDataBase.GetCardInfo(LeaderCard));
		cardInstance.Name = cardId.ToString();
	}

	public void ClearBoard()
	{
		foreach (Node row in CardRowsContainer.GetChildren())
		{
			foreach (Node node in row.GetChildren())
			{
				row.RemoveChild(node);
			}
		}

		foreach (Node row in SpecialCardsContainer.GetChildren())
		{
			foreach (MinCardScene node in row.GetChildren())
			{
				if (OnBoard.Contains(int.Parse(node.Name)))
				{
					row.RemoveChild(node);
				}
			}
		}
	}

	public void UpdateBoard()
	{
		ClearBoard();

		Vector2 rowRectSize = CardRowsContainer.GetChild<Control>(0).Size;
		rowRectSize = new Vector2(rowRectSize.X - 50, rowRectSize.Y);
		Vector2 cardSize = new(rowRectSize.X / MaxHandSize, rowRectSize.Y);
		Dictionary<CardTypes, List<int>> rangeSortedCards = new();

		foreach (int card in OnBoard)
		{
			var cardinfo = CardDataBase.GetCardInfo(card);
			if (rangeSortedCards.ContainsKey(cardinfo.type) is false)
			{
				rangeSortedCards.Add(cardinfo.type, new());
			}

			rangeSortedCards[cardinfo.type].Add(card);
		}

		int i = 1;
		int cardMarginRight = 5;
		foreach (var range in rangeSortedCards)
		{
			string path = (this is Antagonist) ? "Top" : "Bottom";

			Control row = CardRowsContainer.GetNode<Control>("Row" + i++);
			float nextXCardPosition = (rowRectSize.X - cardSize.X * range.Value.Count) / 2;
			foreach (int cardId in range.Value)
			{
				MinCardScene cardInstance = (MinCardScene)GameFieldController.MinCardScene.Instantiate();
				row.AddChild(cardInstance);
				cardInstance.Name = cardId.ToString();

				var card = row.GetNode<MinCardScene>(cardId.ToString());
				card.SetParams(cardSize, CardDataBase.GetCardTexturePath(cardId), CardDataBase.GetCardInfo(cardId));
				card.Position = new Vector2(nextXCardPosition, 0);
				nextXCardPosition += cardSize.X + cardMarginRight;
			}
		}

		UpdateRowsCount();
		UpdateTotalCount();
	}

	public void DoPass()
	{
		IsPass = true;

		if (Antagonist.Instance.IsPass == Protagonist.Instance.IsPass)
		{
			var protagonist = Protagonist.Instance;
			var antagonist = Antagonist.Instance;

			antagonist.IsPass = false;
			protagonist.IsPass = false;

			switch (antagonist.TotalCount.CompareTo(protagonist.TotalCount))
			{
				case < 0:
					protagonist.RoundVBoxContainer.GetNode<CheckBox>("CheckBox" + round.ToString())
						.ButtonPressed = true;
					++gameResult;
					break;
				case > 0:
					antagonist.RoundVBoxContainer.GetNode<CheckBox>("CheckBox" + round.ToString())
						.ButtonPressed = true;
					--gameResult;
					break;
				case 0:
					protagonist.RoundVBoxContainer.GetNode<CheckBox>("CheckBox" + round.ToString())
						.ButtonPressed = true;
					antagonist.RoundVBoxContainer.GetNode<CheckBox>("CheckBox" + round.ToString())
						.ButtonPressed = true;
					break;
			}

			if (Math.Abs(gameResult) == 2 || gameResult != 0 && round == 3)
			{
				GameFieldController.Instance.MatchCompleted((gameResult > 0) ? protagonist : antagonist);
			}
			else if (gameResult == 0 && round == 3)
			{
				GameFieldController.Instance.MatchCompleted(null);
			}

			if (round < 3)
			{
				antagonist.OnBoard = new();
				protagonist.OnBoard = new();
				protagonist.UpdateBoard();
				antagonist.UpdateBoard();
				++round;

				int cardCount = 0;
				var cardList = protagonist.GetRandomCardsFromDeck(
					Math.Min(MaxHandSize - protagonist.Hand.Count, protagonist.Deck.Count));
				if (cardList.Count > 0)
				{
					protagonist.TakeCardsFromDeck(cardList);
					GameFieldController.Instance.UpdateDeckTextures();
				}			
			}
		}
	}

	protected void PutSpecialCard(CardDataBase.CardData cardInfo)
	{
		MinCardScene cardInstance = (MinCardScene)GameFieldController.MinCardScene.Instantiate();
		cardInstance.Name = cardInfo.id.ToString();
		cardInstance.SetParams(SpecialCardsContainer.GetChild<Control>(0).Size,
			CardDataBase.GetCardTexturePath(cardInfo.id), cardInfo);

		switch (cardInfo.strength)
		{
			case < 0:
				SpecialCardsContainer.GetNode((this is Protagonist) ? "TopRow1" : "BottomRow1").AddChild(cardInstance);
				break;
			case > 0:
				SpecialCardsContainer.GetNode((this is Protagonist) ? "BottomRow2" : "TopRow2").AddChild(cardInstance);
				break;
			case 0:
				TemporalSpCardContainer.AddChild(cardInstance);

				cardInstance.SetParams(TemporalSpCardContainer.Size,
					CardDataBase.GetCardTexturePath(cardInfo.id), cardInfo);

				foreach (Node row in SpecialCardsContainer.GetChildren())
				{
					foreach (MinCardScene node in row.GetChildren())
					{
						row.RemoveChild(node);
					}
				}

				System.Threading.Timer timer = null;
				timer = new System.Threading.Timer((obj) =>
					{
						RemoveTemporalSpCard();
						timer.Dispose();
					},
					null, 3000, System.Threading.Timeout.Infinite);
				break;
		}

		Protagonist.Instance.UpdateRowsCount();
		Protagonist.Instance.UpdateTotalCount();
		Antagonist.Instance.UpdateRowsCount();
		Antagonist.Instance.UpdateTotalCount();
	}

	private void RemoveTemporalSpCard()
	{
		foreach (var node in TemporalSpCardContainer.GetChildren())
		{
			TemporalSpCardContainer.RemoveChild(node);
		}
	}
}

