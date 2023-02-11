using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class CardSelectionMenu : Control
{
	public static CardSelectionMenu Instance { get; private set; }
	public static List<string> SelectedCards { get; private set; } = new();
	public static List<string> LeaderCards { get; private set; } = new();
	public static List<string> Nations { get; private set; } = new();
	public static string Nation { get; private set; }
	public static string LeaderCard { get; private set; }

	private PackedScene cardScene = (PackedScene)GD.Load("res://CardScene.tscn");
	private GridContainer allCardsGridContainer;
	private GridContainer selectedCardsGridContainer;


	public override void _Ready()
	{
		Instance = this;
		allCardsGridContainer = GetNode<ScrollContainer>("AllCardScrollContainer").GetNode<GridContainer>("GridContainer");
		selectedCardsGridContainer = GetNode<ScrollContainer>("SelectionCardScrollContainer").GetNode<GridContainer>("GridContainer");


		allCardsGridContainer.AddConstantOverride("hseparation", 195);
		allCardsGridContainer.AddConstantOverride("vseparation", 325);

		selectedCardsGridContainer.AddConstantOverride("hseparation", 195);
		selectedCardsGridContainer.AddConstantOverride("vseparation", 325);

		CardDataBase.UpdateCardDataBase();
		LoadAlCards();
		ChangeLeader();
	}


	private void LoadAlCards()
	{
		SelectedCards = new();

		foreach (var card in CardDataBase.GetAllCards)
		{
			if (card.Value.Type == "Leader")
			{
				Nations.Add(card.Key);
				LeaderCards.Add(card.Key);
			}
			else
			{
				CardScene cardInstance = (CardScene)cardScene.Instance();
				Control t = new();
				t.Name = card.Key;
				allCardsGridContainer.AddChild(t);
				allCardsGridContainer.GetNode<Control>(card.Key).AddChild(cardInstance);
				allCardsGridContainer.GetNode<Control>(card.Key).GetChild<CardScene>(0).SetParams(
					card.Key, new Vector2(155, 285), CardDataBase.GetCardTexturePath(card.Key));
			}
		}

		for (int i = 0; i < allCardsGridContainer.Columns; i++)
		{
			allCardsGridContainer.AddChild(new Control());
			selectedCardsGridContainer.AddChild(new Control());
		}
	}


	private void ChangeLeader()
	{
		int index = LeaderCards.IndexOf(LeaderCard);
		if (LeaderCards.Count == index + 1)
		{
			LeaderCard = LeaderCards.First();
		}
		else
		{
			LeaderCard = LeaderCards[++index];
		}

		var leaderContainer = GetNode<Control>("NationInfoPanel").GetNode("LeaderContainer");

		foreach (Node node in leaderContainer.GetChildren())
		{
			leaderContainer.RemoveChild(node);
		}

		foreach (Node node in selectedCardsGridContainer.GetChildren())
		{
			selectedCardsGridContainer.RemoveChild(node);
		}

		foreach (Node node in allCardsGridContainer.GetChildren())
		{
			allCardsGridContainer.RemoveChild(node);
		}

		LoadAlCards();

		CardScene cardInstance = (CardScene)cardScene.Instance();
		leaderContainer.AddChild(cardInstance);
		leaderContainer.GetChild<CardScene>(0).SetParams(
			LeaderCard, new Vector2(155, 285), CardDataBase.GetCardTexturePath(LeaderCard));
	}


	public void CardSceneEventHandler(string eventName, string cardName)
	{
		if (eventName == "pressed")
		{
			if (CardDataBase.GetCardInfo(cardName).Type == "Leader")
			{
				ChangeLeader();
			}
			else if (SelectedCards.Contains(cardName))
			{
				Control temp = selectedCardsGridContainer.GetNode<Control>(cardName);
				selectedCardsGridContainer.RemoveChild(selectedCardsGridContainer.GetNode<Control>(cardName));
				allCardsGridContainer.AddChild(temp);
				allCardsGridContainer.MoveChild(temp, 0);

				SelectedCards.Remove(cardName);
			}
			else
			{
				Control temp = allCardsGridContainer.GetNode<Control>(cardName);
				allCardsGridContainer.RemoveChild(allCardsGridContainer.GetNode<Control>(cardName));
				selectedCardsGridContainer.AddChild(temp);
				selectedCardsGridContainer.MoveChild(temp, 0);

				SelectedCards.Add(cardName);
			}

			GetNode<Label>("LabelCountInfo").Text = "SELECTED " + SelectedCards.Count() + " CARD(S)";
		}
	}


	private void _on_StartButton_pressed()
	{
		if (SelectedCards.Count() >= 10)
		{
			GetTree().ChangeSceneTo((PackedScene)GD.Load("res://GameFieldScreen.tscn"));
		}
	}
}


