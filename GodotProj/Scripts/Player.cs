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
			foreach (MinCardScene card in CardRowsContainer.GetNode<Control>("Row" + i)
				.GetChildren().Where(it => it.Name != "Label"))
			{
				sum += card.CardDamage;
			}

			RowsCountContainer.GetNode<Label>("Row" + i).Text = sum.ToString();
		}
	}

	public void SetLeaderCard(int cardId)
	{
		MinCardScene cardInstance = (MinCardScene)GameFieldController.MinCardScene.Instantiate();
		LeaderCardContainer.AddChild(cardInstance);
		cardInstance.SetParams(LeaderCardContainer.Size,
			CardDataBase.GetCardTexturePath(cardId), CardDataBase.GetCardInfo(LeaderCard));
		cardInstance.Name = "leaderCardInst";
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
	}

	protected void UpdateBoard()
	{
		ClearBoard();
		//UpdateDiscardPileFlippedCard();

		Vector2 rowRectSize = CardRowsContainer.GetChild<Control>(0).Size;
		rowRectSize = new Vector2(rowRectSize.X - 50, rowRectSize.Y);
		Vector2 cardSize = new(rowRectSize.X / MaxHandSize, rowRectSize.Y);
		Dictionary<CardTypes, List<int>> rangeSortedCards = new();

		foreach (int card in this.OnBoard)
		{
			if (rangeSortedCards.ContainsKey(CardDataBase.GetCardInfo(card).type) is false)
			{
				rangeSortedCards.Add(CardDataBase.GetCardInfo(card).type, new());
			}

			rangeSortedCards[CardDataBase.GetCardInfo(card).type].Add(card);
		}

		int i = 1;
		foreach (var range in rangeSortedCards)
		{
			Control row = CardRowsContainer.GetNode<Control>("Row" + i++);
			float nextXCardPosition = (rowRectSize.X - cardSize.X * range.Value.Count) / 2;
			foreach (int cardId in range.Value)
			{
				MinCardScene cardInstance = (MinCardScene)GameFieldController.MinCardScene.Instantiate();
				row.AddChild(cardInstance);
				cardInstance.Name = cardId.ToString();

				var card = row.GetNode<MinCardScene>(cardId.ToString());
				card.SetParams(cardSize, CardDataBase.GetCardTexturePath(cardId), CardDataBase.GetCardInfo(cardId));
				card.Position = new Vector2(nextXCardPosition + 10, 0);
				nextXCardPosition += cardSize.X;
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
			Antagonist.Instance.IsPass = false;
			Protagonist.Instance.IsPass = false;

			switch (Antagonist.Instance.TotalCount.CompareTo(Protagonist.Instance.TotalCount))
			{
				case < 0:
					Protagonist.Instance.RoundVBoxContainer.GetNode<CheckBox>("CheckBox" + round.ToString())
						.ButtonPressed = true;
					++gameResult;
					break;
				case > 0:
					Antagonist.Instance.RoundVBoxContainer.GetNode<CheckBox>("CheckBox" + round.ToString())
						.ButtonPressed = true;
					--gameResult;
					break;
				case 0:
					Protagonist.Instance.RoundVBoxContainer.GetNode<CheckBox>("CheckBox" + round.ToString())
						.ButtonPressed = true;
					Antagonist.Instance.RoundVBoxContainer.GetNode<CheckBox>("CheckBox" + round.ToString())
						.ButtonPressed = true;
					break;
			}

			if (Math.Abs(gameResult) == 2 || gameResult != 0 && round == 3)
			{
				GameFieldController.Instance.MatchCompleted((gameResult > 0) ? Protagonist.Instance : Antagonist.Instance);
			}
			else if (gameResult == 0 && round == 3)
			{
				GameFieldController.Instance.MatchCompleted(null);
			}

			Antagonist.Instance.OnBoard = new();
			Protagonist.Instance.OnBoard = new();
			Protagonist.Instance.UpdateBoard();
			Antagonist.Instance.UpdateBoard();

			++round;
		}
	}

	abstract protected void UpdateDiscardPileFlippedCard();
	abstract protected void UpdateDeckSize();


	//public void TakeCardFromDiscardPile(string card)
	//{
	//	if (DiscardPile.Contains(card) is false)
	//	{
	//		throw new Exception("Discard pile doesn`t contain the given card");
	//	}

	//	DiscardPile.Remove(card);
	//	Hand.Add(card);
	//}
}

