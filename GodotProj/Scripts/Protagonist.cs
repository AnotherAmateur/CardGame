using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Protagonist : Player
{
	protected Control CardsHandContainer;

	public List<int> Hand { get; private set; }
	public List<int> Deck { get; private set; }
	public static Protagonist Instance { get; protected set; }


	public Protagonist(int leaderCard, List<int> deck, Control cardRowsContainer, Control cardsHandContainer,
		Control leaderCardContainer, Control discardPileContainer, Label totalCount, Control rowsCountContainer) :
		base(leaderCard, cardRowsContainer, leaderCardContainer, discardPileContainer, totalCount, rowsCountContainer)
	{
		Deck = deck;
		Hand = new();
		CardsHandContainer = cardsHandContainer;
		Instance = this;
	}
	

	public void PutCardFromHandOnBoard(int cardId)
	{
		if (Hand.Contains(cardId))
		{
			Hand.Remove(cardId);
			OnBoard.Add(cardId);
		}
		else
		{
			throw new Exception("There is no such a card");
		}

		UpdateHand();
		UpdateBoard();
	}


	public List<int> GetRandomCardsFromDeck(int count)
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
			int number = random.RandiRange(0, Deck.Count - 1);
			uniqueNumbers.Add(number);
		}

		List<int> result = new();
		foreach (int index in uniqueNumbers)
		{
			result.Add(Deck[index]);
		}

		return result;
	}


	public void TakeCardsFromDeck(List<int> cards)
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

		Vector2 cardSize = new(CardsHandContainer.RectSize.x / MaxHandSize, CardsHandContainer.RectSize.y);
		float nextCardPosition = (CardsHandContainer.RectSize.x - cardSize.x * this.Hand.Count) / 2;
		for (int i = 0; i < this.Hand.Count; i++)
		{
			int cardId = this.Hand[i];

			SlaveCardScene cardInstance = (SlaveCardScene)GameFieldController.SlaveCardScene.Instance();
			CardsHandContainer.AddChild(cardInstance);

			var card = CardsHandContainer.GetChild<SlaveCardScene>(i);
			card.SetParams(cardId.ToString(), cardSize, 
				CardDataBase.GetCardTexturePath(cardId), 
				CardDataBase.GetCardInfo(LeaderCard).text);

			card.Position = new Vector2(nextCardPosition, 0);
			nextCardPosition += cardSize.x;
		}
	}


	protected override void UpdateDiscardPileFlippedCard()
	{
		if (DiscardPile.Count == 0)
		{
			foreach (Node node in DiscardPileContainer.GetChildren())
			{
				DiscardPileContainer.RemoveChild(node);
			}
		}
		else if (DiscardPile.Contains(DiscardPileFlippedcardId) is false)
		{
			Godot.RandomNumberGenerator random = new();
			random.Randomize();
			int index = random.RandiRange(0, DiscardPile.Count - 1);
			DiscardPileFlippedcardId = DiscardPile[index];
			SlaveCardScene cardInstance = (SlaveCardScene)GameFieldController.SlaveCardScene.Instance();
			DiscardPileContainer.AddChild(cardInstance);
		}
	}

	protected override void UpdateDeckSize()
	{
		
	}
}

