using CardGameProj.Scripts;
using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

public partial class Protagonist : Player
{
	protected Control CardsHandContainer;
	public List<int> Hand { get; private set; }
	public List<int> Deck { get; private set; }
	public static Protagonist Instance { get; protected set; }


	public Protagonist(int leaderCard, List<int> deck, Control cardRowsContainer, Control cardsHandContainer,
		Control leaderCardContainer, Control discardPileContainer, Label totalCount, Control rowsCountContainer, HBoxContainer roundVBoxContainer) :
		base(leaderCard, cardRowsContainer, leaderCardContainer, discardPileContainer, totalCount, rowsCountContainer, roundVBoxContainer)
	{
		Deck = deck;
		Hand = new();
		CardsHandContainer = cardsHandContainer;
		Instance = this;
	}

	public new void DoPass()
	{
		base.DoPass();
	}

	public void PutCardFromHandOnBoard(CardDataBase.CardData cardInfo)
	{		
		if (cardInfo.type == CardTypes.Special)
		{
			PutSpecialCard(cardInfo);
		}
		else
		{
			OnBoard.Add(cardInfo.id);
			UpdateBoard();
		}
		
		Hand.Remove(cardInfo.id);
		UpdateHand();
	}

	public List<int> GetRandomCardsFromDeck(int count)
	{
		List<int> result = new();
		if (count > 0)
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
			
			foreach (int index in uniqueNumbers)
			{
				result.Add(Deck[index]);
			}

			return result;
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
		GameFieldController.Instance.UpdateDeckSize();
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
}

