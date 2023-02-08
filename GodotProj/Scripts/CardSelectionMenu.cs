using Godot;
using System;
using System.Collections.Generic;

public class CardSelectionMenu : Control
{
	public static List<string> SelectedCards { get; private set; } = new();
	public static string Nation { get; private set; }
	public static string LeaderCard { get; private set; }


	private PackedScene cardScene = (PackedScene)GD.Load("res://CardScene.tscn");


	public override void _Ready()
	{
		SelectedCards.Add("Card1");
		SelectedCards.Add("Card2");
		SelectedCards.Add("Card3");
		SelectedCards.Add("Card4");
		SelectedCards.Add("Card5");
		SelectedCards.Add("Card6");

		CardDataBase.UpdateCardDataBase();


		GridContainer t = GetNode<ScrollContainer>("AllCardScrollContainer").GetNode<VBoxContainer>("VBoxContainer").GetNode<GridContainer>("GridContainer");

		for (int i = 0; i < 13; i++)
		{
			CardScene cardInstance = (CardScene)cardScene.Instance();
			t.AddChild(new Control());
			t.GetChild(i).AddChild(cardInstance);			
			cardInstance.Name = "Card1";
			t.GetChild(i).GetChild<CardScene>(0).SetParams("Card1", new Vector2(155, 285), CardDataBase.GetCardTexturePath("Nation1", "Card1"));
			t.AddConstantOverride("hseparation", 195);
			t.AddConstantOverride("vseparation", 325);
		}

		LeaderCard = "Card0";
		Nation = "Nation1";
	}


	private void _on_StartButton_pressed()
	{
		GetTree().ChangeSceneTo((PackedScene)GD.Load("res://GameFieldScreen.tscn"));
	}
}


