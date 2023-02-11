using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class Player
{
	protected Control CardRowsContainer;
	protected Control LeaderCardContainer;
	public const int maxHandSize = 10;
	//protected Control DiscardPileContainer;


	public List<string> DiscardPile { get; protected set; }
	public List<string> OnBoard { get; protected set; }
	public string LiderCard { get; protected set; }
	public static Player Instance { get; protected set; }


	public Player(string leaderCard, Control cardRowsContainer, Control leaderCardContainer)
	{
		DiscardPile = new();
		OnBoard = new();
		LiderCard = leaderCard;
		CardRowsContainer = cardRowsContainer;
		LeaderCardContainer = leaderCardContainer;
	}


	public void SetLeaderCard(string cardName)
	{
		CardScene cardInstance = (CardScene)GameFieldController.CardScene.Instance();
		LeaderCardContainer.AddChild(cardInstance);
		LeaderCardContainer.GetChild<CardScene>(0).SetParams(cardName, LeaderCardContainer.RectSize, CardDataBase.GetCardTexturePath(cardName));
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

		Vector2 rowRectSize = CardRowsContainer.GetChild<Control>(0).RectSize;
		Vector2 cardSize = new(rowRectSize.x / maxHandSize, rowRectSize.y);
		Dictionary<string, List<string>> rangeSortedCards = new();

		foreach (string card in this.OnBoard)
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
				CardScene cardInstance = (CardScene)GameFieldController.CardScene.Instance();
				row.AddChild(cardInstance);
				cardInstance.Name = cardName;

				var card = row.GetNode<CardScene>(cardName);
				card.SetParams(cardName, cardSize, CardDataBase.GetCardTexturePath(cardName));
				card.Position = new Vector2(nextXCardPosition, 0);
				nextXCardPosition += cardSize.x;
			}
		}
	}



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


public class Protagonist : Player
{
	protected Control CardsHandContainer;

	public List<string> Hand { get; private set; }
	public List<string> Deck { get; private set; }


	public Protagonist(string leaderCard, List<string> deck, Control cardRowsContainer, Control cardsHandContainer,
		Control leaderCardContainer) : base(leaderCard, cardRowsContainer, leaderCardContainer)
	{
		Deck = deck;
		Hand = new();
		CardsHandContainer = cardsHandContainer;
		Instance = this;
	}


	public void PutCardFromHandOnBoard(string cardName)
	{
		if (Hand.Contains(cardName))
		{
			Hand.Remove(cardName);
			OnBoard.Add(cardName);
		}
		else
		{
			throw new Exception("There is no such a card");
		}

		UpdateHand();
		UpdateBoard();
	}


	public List<string> GetRandomCardsFromDeck(int count)
	{
		if (count > Deck.Count)
		{
			throw new Exception("There are not enough cards in the deck");
		}

		Godot.RandomNumberGenerator random = new();
		HashSet<int> uniqueNumbers = new();
		while (uniqueNumbers.Count < count)
		{
			random.Randomize();
			int number = random.RandiRange(0, Deck.Count);
			uniqueNumbers.Add(number);
		}

		List<string> result = new();
		foreach (int index in uniqueNumbers)
		{
			result.Add(Deck[index]);
		}

		return result;
	}


	public void TakeCardsFromDeck(List<string> cards)
	{
		if (cards.Count > Deck.Count)
		{
			throw new Exception("There are not enough cards in the deck");
		}

		var remainingСards = Deck.Except(cards).ToList();

		if (Deck.Count - remainingСards.Count != cards.Count)
		{
			throw new Exception("The given cards don`t belong the deck");
		}

		Deck = remainingСards;
		Hand.AddRange(cards);

		UpdateHand();
	}


	public void UpdateHand()
	{
		foreach (Node node in CardsHandContainer.GetChildren())
		{
			CardsHandContainer.RemoveChild(node);
		}

		Vector2 cardSize = new(CardsHandContainer.RectSize.x / maxHandSize, CardsHandContainer.RectSize.y);
		float nextCardPosition = (CardsHandContainer.RectSize.x - cardSize.x * this.Hand.Count) / 2;
		for (int i = 0; i < this.Hand.Count; i++)
		{
			string cardName = this.Hand[i];

			CardScene cardInstance = (CardScene)GameFieldController.CardScene.Instance();
			CardsHandContainer.AddChild(cardInstance);

			var card = CardsHandContainer.GetChild<CardScene>(i);
			card.SetParams(cardName, cardSize, CardDataBase.GetCardTexturePath(cardName));

			card.Position = new Vector2(nextCardPosition, 0);
			nextCardPosition += cardSize.x;
		}
	}


	protected void BlockHand()
	{

	}
}



public class Antagonist : Player
{
	public Antagonist(string leaderCard, Control cardRowsContainer, Control leaderCardContainer) :
		base(leaderCard, cardRowsContainer, leaderCardContainer)
	{
		Instance = this;
	}
}
