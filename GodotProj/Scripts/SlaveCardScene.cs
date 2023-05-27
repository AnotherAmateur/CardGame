using Godot;
using System.Xml.Linq;

public class SlaveCardScene : Node2D
{

	private int yOffsetPx = 20;
	public int CardDamage{ get; private set; }
	private Vector2 initRectSize = new Vector2(512f, 768f);
	//private Vector2 initScale = new Vector2(1.5f, 1.5f);


	public override void _Ready()
	{
	}


	private void _on_Card_pressed()
	{
		if (GameFieldController.Instance is null)
		{
			CardSelectionMenu.Instance.CardSceneEventHandler("pressed", int.Parse(Name));
		}
		else
		{
			GameFieldController.Instance.CardSceneEventHandler("pressed", int.Parse(Name));
		}
	}


	public void SetParams(string name, Vector2 rectSize, string texturePath, string text)
	{
		Name = name;
		GetNode<TextureButton>("Card").HintTooltip = text;
		GetNode<TextureButton>("Card").TextureNormal = (Texture)GD.Load(texturePath);
		float scaleFactorY = rectSize.y / initRectSize.y;
		float scaleFactorX = rectSize.x / initRectSize.x;
		this.Scale = new Vector2(scaleFactorX, scaleFactorY);

		CardDamage = CardDataBase.GetCardInfo(int.Parse(name)).strength;
		GetNode<Label>("LabelsContainer/VBoxContainer/HBoxContainer/Strength").Text = CardDamage.ToString();
	}


	public void SetDamage(int damage) 
	{
		CardDamage = damage;
	}


	public void DisableCardButton()
	{
		GetNode<TextureButton>("Card").Disabled = true;
		GetNode<TextureButton>("Card").SelfModulate = new Godot.Color("a7a7a7");
	}


	private void _on_Card_mouse_entered()
	{
		Protagonist player = Protagonist.Instance;
		if ((player is null) is false && player.Hand.Contains(int.Parse(Name)))
		{
			var initPosition = new Vector2(this.Position.x, this.Position.y - yOffsetPx);
			this.Position = initPosition;
		}
	}


	private void _on_Card_mouse_exited()
	{
		Protagonist player = Protagonist.Instance;
		if ((player is null) is false && player.Hand.Contains(int.Parse(Name)))
		{
			var initPosition = new Vector2(this.Position.x, this.Position.y + yOffsetPx);
			this.Position = initPosition;
		}
	}
}






