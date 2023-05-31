using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Protagonist : Player
{
	protected Control CardsHandContainer;

	public List<int> Hand { get; private set; }
	public List<int> Deck { get; private set; }
	public static Protagonist Instantiate { get; protected set; }


	public Protagonist(int leaderCard, List<int> deck, Control cardRowsContainer, Control cardsHandContainer,
		Control leaderCardContainer, Control discardPileContainer, Label totalCount, Control rowsCountContainer) :
		base(leaderCard, cardRowsContainer, leaderCardContainer, discardPileContainer, totalCount, rowsCountContainer)
	{
		Deck = deck;
		Hand = new();
		CardsHandContainer = cardsHandContainer;
		Instantiate = this;
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
			string err = "There are not enough cards in the deck";
			OS.Alert(String.Join("\n", "Protagonist/GetRandomCardsFromDeck: ", err));
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
			string err = "There are not enough cards in the deck";
			OS.Alert(String.Join("\n", "Protagonist/TakeCardsFromDeck: ", err));
		}

		var remainingСards = Deck.Except(cards).ToList<int>();

		if (Deck.Count - remainingСards.Count != cards.Count)
		{
			string err = "There are not enough cards in the deck";
			OS.Alert(String.Join("\n", "Protagonist/TakeCardsFromDeck: ", err));
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

		Vector2 cardSize = new(CardsHandContainer.Size.X / MaxHandSize, CardsHandContainer.Size.Y);
		float nextCardPosition = (CardsHandContainer.Size.X - cardSize.X * this.Hand.Count) / 2;
		for (int i = 0; i < this.Hand.Count; i++)
		{
			int cardId = this.Hand[i];

			MinCardScene cardInstance = (MinCardScene)GameFieldController.MinCardScene.Instantiate();
			CardsHandContainer.AddChild(cardInstance);

			var card = CardsHandContainer.GetChild<MinCardScene>(i);
			card.SetParams(cardSize, CardDataBase.GetCardTexturePath(cardId), 
				CardDataBase.GetCardInfo(cardId));

			card.Position = new Vector2(nextCardPosition, 0);
			nextCardPosition += cardSize.X;
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
			MinCardScene cardInstance = (MinCardScene)GameFieldController.MinCardScene.Instantiate();
			DiscardPileContainer.AddChild(cardInstance);
		}
	}

	protected override void UpdateDeckSize()
	{
		
	}
}

