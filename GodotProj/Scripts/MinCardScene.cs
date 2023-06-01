using CardGameProj.Scripts;
using CardGameProj.SeparateClasses;
using Godot;

public partial class MinCardScene : Node2D
{
	private const int yOffsetPx = 20;
	public int CardDamage { get; private set; }
	private Vector2 initRectSize = new Vector2(States.InitCardSize.Item1, States.InitCardSize.Item2);

	public override void _Ready()
	{
	}


	public void SetParams(Vector2 rectSize, string texturePath, CardDataBase.CardData card)
	{
		Name = card.id.ToString();
		GetNode<TextureButton>("Card").TooltipText = $"Вес: {card.strength}\nСпецифика: {card.type}\n{card.text}";
		GetNode<TextureButton>("Card").TextureNormal = (Texture2D)GD.Load(texturePath);
		float scaleFactorY = rectSize.Y / initRectSize.Y;
		float scaleFactorX = rectSize.X / initRectSize.X;
		//float scaleFactorX = scaleFactorY;
		this.Scale = new Vector2(scaleFactorX, scaleFactorY);

		CardDamage = CardDataBase.GetCardInfo(card.id).strength;
		GetNode<Label>("LabelsContainer/VBoxContainer/HBoxContainer/Strength").Text = CardDamage.ToString();
		GetNode<Label>("LabelsContainer/VBoxContainer/HBoxContainer2/Paragraph").Text = card.type.ToString();
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


	private void _on_card_mouse_entered()
	{
		Protagonist player = Protagonist.Instantiate;
		if ((player is null) is false && player.Hand.Contains(int.Parse(Name)))
		{
			var initPosition = new Vector2(this.Position.X, this.Position.Y - yOffsetPx);
			this.Position = initPosition;
		}
	}


	private void _on_card_mouse_exited()
	{
		Protagonist player = Protagonist.Instantiate;
		if ((player is null) is false && player.Hand.Contains(int.Parse(Name)))
		{
			var initPosition = new Vector2(this.Position.X, this.Position.Y + yOffsetPx);
			this.Position = initPosition;
		}
	}


	private void _on_card_gui_input(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed)
		{
			CardEvents cardEvent = CardEvents.LeftCllick;

			if (eventMouseButton.ButtonMask == MouseButtonMask.Right)
			{
				cardEvent = CardEvents.RightClick;
			}

			GameFieldController.Instantiate.CardSceneEventHandler(cardEvent, int.Parse(Name));
		}
	}
}


