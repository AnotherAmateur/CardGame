using Godot;
using System;

public class CardScene : Node2D
{
	public override void _Ready()
	{

	}


	private void _on_Card_pressed()
	{
		GameFieldController.Instance.CardSceneEventHandler("pressed", Name);
	}


	public void SetParams(string name, Vector2 rectSize, string texturePath)
	{
		Name = name;
		GetChild<TextureButton>(0).HintTooltip = name;
		GetChild<TextureButton>(0).RectSize = rectSize;
		GetChild<TextureButton>(0).TextureNormal = (Texture)GD.Load(texturePath);
	}


	public void DisableCardButton()
	{
		GetChild<TextureButton>(0).Disabled = true;
		GetChild<TextureButton>(0).SelfModulate = new Godot.Color("a7a7a7");
	}
}


