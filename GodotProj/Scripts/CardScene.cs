using Godot;

public class CardScene : Node2D
{

	private int offsetPx = 20;
	public int CardDamage{ get; private set; }


	public override void _Ready()
	{

	}


	private void _on_Card_pressed()
	{
		if (GameFieldController.Instance is null)
		{
			CardSelectionMenu.Instance.CardSceneEventHandler("pressed", Name);
		}
		else
		{
			GameFieldController.Instance.CardSceneEventHandler("pressed", Name);
		}
	}


	public void SetParams(string name, Vector2 rectSize, string texturePath)
	{
		Name = name;
		GetChild<TextureButton>(0).HintTooltip = name;
		GetChild<TextureButton>(0).RectSize = rectSize;
		GetChild<TextureButton>(0).TextureNormal = (Texture)GD.Load(texturePath);
		CardDamage = CardDataBase.GetCardInfo(name).Damage;
	}


	public void SetDamage(int damage) 
	{
		CardDamage = damage;
	}


	public void DisableCardButton()
	{
		GetChild<TextureButton>(0).Disabled = true;
		GetChild<TextureButton>(0).SelfModulate = new Godot.Color("a7a7a7");
	}


	private void _on_Card_mouse_entered()
	{
		Protagonist player = Protagonist.Instance;
		if ((player is null) is false && player.Hand.Contains(Name))
		{
			GetChild<TextureButton>(0).RectPosition = new Vector2(GetChild<TextureButton>(0).RectPosition.x, GetChild<TextureButton>(0).RectPosition.y - offsetPx);
		}
	}


	private void _on_Card_mouse_exited()
	{
		Protagonist player = Protagonist.Instance;
		if ((player is null) is false && player.Hand.Contains(Name))
		{
			GetChild<TextureButton>(0).RectPosition = new Vector2(GetChild<TextureButton>(0).RectPosition.x, GetChild<TextureButton>(0).RectPosition.y + offsetPx);
		}
	}
}






