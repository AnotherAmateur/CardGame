using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class Player
{
	protected Control CardRowsContainer;
	protected Control LeaderCardContainer;
	protected Control DiscardPileContainer;
	protected Label TotalCount;
	public const int maxHandSize = 10;
	protected string DiscardPileFlippedCardName;
	

	public List<string> DiscardPile { get; protected set; }
	public List<string> OnBoard { get; protected set; }
	public string LiderCard { get; protected set; }


	public Player(string leaderCard, Control cardRowsContainer, Control leaderCardContainer,
		Control discardPileContainer, Label totalCount)
	{
		DiscardPile = new();
		OnBoard = new();
		LiderCard = leaderCard;
		CardRowsContainer = cardRowsContainer;
		LeaderCardContainer = leaderCardContainer;
		DiscardPileContainer = discardPileContainer;
		TotalCount = totalCount;
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
		UpdateDiscardPileFlippedCard();

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


public class Protagonist : Player
{
	protected Control CardsHandContainer;

	public List<string> Hand { get; private set; }
	public List<string> Deck { get; private set; }
	public static Protagonist Instance { get; protected set; }


	public Protagonist(string leaderCard, List<string> deck, Control cardRowsContainer, Control cardsHandContainer,
		Control leaderCardContainer, Control discardPileContainer, Label totalCount) :
		base(leaderCard, cardRowsContainer, leaderCardContainer, discardPileContainer, totalCount)
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
			int number = random.RandiRange(0, Deck.Count - 1);
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


	protected override void UpdateDiscardPileFlippedCard()
	{
		if (DiscardPile.Count == 0)
		{
			foreach (Node node in DiscardPileContainer.GetChildren())
			{
				DiscardPileContainer.RemoveChild(node);
			}
		}
		else if (DiscardPile.Contains(DiscardPileFlippedCardName) is false)
		{
			Godot.RandomNumberGenerator random = new();
			random.Randomize();
			int index = random.RandiRange(0, DiscardPile.Count - 1);
			DiscardPileFlippedCardName = DiscardPile[index];
			CardScene cardInstance = (CardScene)GameFieldController.CardScene.Instance();
			DiscardPileContainer.AddChild(cardInstance);
		}
	}

	protected override void UpdateDeckSize()
	{
		throw new NotImplementedException();
	}
}



public class Antagonist : Player
{
	public Antagonist(string leaderCard, Control cardRowsContainer, Control leaderCardContainer, Control discardPileContainer, 
		Label totalCount) : base(leaderCard, cardRowsContainer, leaderCardContainer, discardPileContainer, totalCount)
	{
		HTTPRequastInit();
	}


	private void HTTPRequastInit()
	{
		var t = ServerImitation.HTTPRequestInit();
		SetLeaderCard(t["leaderCard"]);
	}


	public void PutCardOnBoard(string cardName)
	{
		ServerImitation.HTTPRequestGetMove();

		OnBoard.Add(cardName);
		UpdateBoard();
	}

	protected override void UpdateDiscardPileFlippedCard()
	{
		throw new NotImplementedException();
	}

	protected override void UpdateDeckSize()
	{
		throw new NotImplementedException();
	}
}


public static class ServerImitation
{
	public static Dictionary<string, string> HTTPRequestInit()
	{
		Dictionary<string, string> response = new();
		response.Add("leaderCard", CardSelectionMenu.LeaderCard);
		response.Add("deckCount", CardSelectionMenu.SelectedCards.Count.ToString());

		return response;
	}


	public static Dictionary<string, string> HTTPRequestGetMove()
	{
		Dictionary<string, string> response = new();
		response.Add("cardName", CardSelectionMenu.LeaderCard);
		response.Add("deckCount", CardSelectionMenu.SelectedCards.Count.ToString());

		return response;
	}

}

