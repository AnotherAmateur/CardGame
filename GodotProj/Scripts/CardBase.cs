using Godot;
using System;

public class CardBase : MarginContainer
{
	public override void _Ready()
	{
		CardDataBase cardDataBase = new();

		RectSize = new Vector2(120, 200);
		GetNode<Sprite>("Card").Scale *= RectSize / GetNode<Sprite>("Card").Texture.GetSize();

		GetNode<Label>("AttackInfo").Text = cardDataBase.Cards["card1"].Damage.ToString();
	}
}
