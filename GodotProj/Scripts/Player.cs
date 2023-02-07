using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class Player
{
	public List<string> Deck { get; private set; }

	public List<string> Hand { get; private set; }

	public List<string> DiscardPile { get; private set; }

	public string Nation { get; private set; }

	public List<string> OnBoard { get; private set; }


	public Player(List<string> deck, string nation)
	{
		Deck = deck;
		Nation = nation;
		Hand = new();
		DiscardPile = new();
		OnBoard = new();
	}


	public List<string> GetRandomCardsFromDeck(int count)
	{
		if (count > Deck.Count)
		{
			throw new Exception("There are not enough cards in the deck");
		}

		Random random = new();
		HashSet<int> uniqueNumbers = new();
		while (uniqueNumbers.Count < count)
		{
			int number = random.Next(Deck.Count);
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
	}


	public void TakeCardFromDiscardPile(string card)
	{
		if (DiscardPile.Contains(card) is false)
		{
			throw new Exception("Discard pile doesn`t contain the given card");
		}

		DiscardPile.Remove(card);
		Hand.Add(card);
	}


	public void DiscardCard(string card)
	{
		if (Hand.Contains(card))
		{
			Hand.Remove(card);
			OnBoard.Add(card);
		}
		else if (OnBoard.Contains(card))
		{
			DiscardPile.Add(card);
			OnBoard.Remove(card);
		}
		else
		{
			throw new Exception("There is no such a card");
		}		
	}
}
