using CardGameProj.Scripts;
using CardGameProj.SeparateClasses;
using Godot;

public partial class MinCardScene : Node2D
{
	private const int yOffsetPx = 20;
	public int CardDamage { get; private set; }
	private Vector2 initRectSize = new Vector2(States.InitCardSize.Item1, States.InitCardSize.Item2);

	public override void _Ready() { }

	public void SetParams(Vector2 rectSize, string texturePath, CardDB.CardData card)
	{
		Name = card.Id.ToString();
		GetNode<TextureButton>("Card").TooltipText = $"Вес: {card.Strength}\nСпецифика: {card.Category}\n{card.Text}";
		GetNode<TextureButton>("Card").TextureNormal = (Texture2D)GD.Load(texturePath);
		float scaleFactorY = rectSize.Y / initRectSize.Y;
		float scaleFactorX = rectSize.X / initRectSize.X;
		this.Scale = new Vector2(scaleFactorX, scaleFactorY);

		CardDamage = card.Strength;
		GetNode<Label>("LabelsContainer/VBoxContainer/HBoxContainer/Strength").Text = CardDamage.ToString();
		GetNode<Label>("LabelsContainer/VBoxContainer/Paragraph").Text = 
			(card.Type == CardTypes.Special) ? card.Text : card.Category.ToString();
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
		Protagonist player = Protagonist.Instance;
		if ((player is null) is false && player.Hand.Contains(int.Parse(Name)))
		{
			var initPosition = new Vector2(this.Position.X, this.Position.Y - yOffsetPx);
			this.Position = initPosition;
		}
	}

	private void _on_card_mouse_exited()
	{
		Protagonist player = Protagonist.Instance;
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

			GFieldController.Instance.CardSceneEventHandler(cardEvent, int.Parse(Name));
		}
	}
}


