using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class Player
{
	private List<string> deck;

	public List<string> Deck
	{
		get { return deck; }
	}

	private List<string> hand;

	public List<string> Hand
	{
		get { return hand; }
	}

	private List<string> discardPile;

	public List<string> DiscardPile
	{
		get { return discardPile; }
	}


	public Player(List<string> deck)
	{
		this.deck = deck;
	}


	public List<string> GetRandomCardsFromDeck(int count)
	{
		if (count > deck.Count)
		{
			throw new Exception("There are not enough cards in the deck");
		}

		Random random = new();
		HashSet<int> uniqueNumbers = new();
		while (uniqueNumbers.Count < count)
		{
			int number = random.Next(deck.Count);
			uniqueNumbers.Add(number);
		}

		List<string> result = new();
		foreach (int index in uniqueNumbers)
		{
			result.Add(deck[index]);
		}

		return result;
	}


	public void TakeCardsFromDeck(List<string> cards)
	{
		if (cards.Count > deck.Count)
		{
			throw new Exception("There are not enough cards in the deck");
		}

		var remainingСards = deck.Except(cards).ToList();

		if (deck.Count - remainingСards.Count != cards.Count)
		{
			throw new Exception("The given cards don`t belong the deck");
		}

		deck = remainingСards;
		hand.AddRange(cards);
	}


	public void TakeCardFromDiscardPile(string card)
	{
		if (discardPile.Contains(card) is false)
		{
			throw new Exception("Discard pile doesn`t contain the given card");
		}

		discardPile.Remove(card);
		hand.Add(card);
	}


	public void DiscardCards(List<string> cards)
	{
	}
}

