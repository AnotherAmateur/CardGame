using CardGameProj.Scripts;
using Godot;
using System.Collections.Generic;

public abstract class Player
{
	protected Control CardRowsContainer;
	protected Control LeaderCardContainer;
	protected Control DiscardPileContainer;
	protected Control RowsCountContainer;
	protected Label TotalCount;
	protected int totalCount;
	public const int MaxHandSize = 10;
	protected int DiscardPileFlippedcardId;
	protected bool isPass;


	public List<int> DiscardPile { get; protected set; }
	public List<int> OnBoard { get; protected set; }
	public int LeaderCard { get; protected set; }


	public Player(int leaderCard, Control cardRowsContainer, Control leaderCardContainer,
		Control discardPileContainer, Label totalCount, Control rowsCountContainer)
	{
		DiscardPile = new();
		OnBoard = new();
		LeaderCard = leaderCard;
		CardRowsContainer = cardRowsContainer;
		LeaderCardContainer = leaderCardContainer;
		DiscardPileContainer = discardPileContainer;
		TotalCount = totalCount;

		//UpdateTotalCount();
		RowsCountContainer = rowsCountContainer;
	}


	protected void UpdateTotalCount()
	{
		totalCount = 0;
		foreach (Control row in CardRowsContainer.GetChildren())
		{
			foreach (SlaveCardScene card in row.GetChildren())
			{
				totalCount += card.CardDamage;
			}
		}

		TotalCount.Text = totalCount.ToString();
	}


	protected void UpdateRowsCount()
	{
		int oneSideRowsNumber = 3;
		for (int i = 1; i <= oneSideRowsNumber; i++)
		{
			int sum = 0;
			foreach (SlaveCardScene card in CardRowsContainer.GetNode<Control>("Row" + i).GetChildren())
			{
				sum += card.CardDamage;
			}

			RowsCountContainer.GetNode<Label>("Row" + i).Text = sum.ToString();
		}
	}


	public void SetLeaderCard(int cardId)
	{
		SlaveCardScene cardInstance = (SlaveCardScene)GameFieldController.SlaveCardScene.Instance();
		LeaderCardContainer.AddChild(cardInstance);
		LeaderCardContainer.GetChild<SlaveCardScene>(0).SetParams(cardId.ToString(),
			LeaderCardContainer.RectSize, CardDataBase.GetCardTexturePath(cardId),
			CardDataBase.GetCardInfo(LeaderCard).text);
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


	public void UpdateBoard()
	{
		ClearBoard();
		//UpdateDiscardPileFlippedCard();

		Vector2 rowRectSize = CardRowsContainer.GetChild<Control>(0).RectSize;
		Vector2 cardSize = new(rowRectSize.x / MaxHandSize, rowRectSize.y);
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
			float nextXCardPosition = (rowRectSize.x - cardSize.x * range.Value.Count) / 2;
			foreach (int cardId in range.Value)
			{
				SlaveCardScene cardInstance = (SlaveCardScene)GameFieldController.SlaveCardScene.Instance();
				row.AddChild(cardInstance);
				cardInstance.Name = cardId.ToString();

				var card = row.GetNode<SlaveCardScene>(cardId.ToString());
				card.SetParams(cardId.ToString(), cardSize,
					CardDataBase.GetCardTexturePath(cardId),
					CardDataBase.GetCardInfo(LeaderCard).text);
				card.Position = new Vector2(nextXCardPosition, 0);
				nextXCardPosition += cardSize.x;
			}
		}

		UpdateTotalCount();
		UpdateRowsCount();
	}


	public void DoPass()
	{
		isPass = true;

		if (Antagonist.Instance.isPass == Protagonist.Instance.isPass)
		{
			Antagonist.Instance.OnBoard = new();
			Protagonist.Instance.OnBoard = new();
			Protagonist.Instance.UpdateBoard();
			Antagonist.Instance.UpdateBoard();
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

