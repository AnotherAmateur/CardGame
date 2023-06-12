using CardGameProj.Scripts;
using CardGameProj.SeparateClasses;
using Godot;

public partial class SlaveCardScene : Node2D
{

	private int yOffsetPx = 20;
	public int CardDamage { get; private set; }
	private Vector2 initRectSize = new Vector2(States.InitCardSize.Item1, States.InitCardSize.Item2);
	private bool disconnectSignals;

	public override void _Ready() { }

	public void _on_Card_pressed()
	{
		if (disconnectSignals is false)
		{
			if (GameFieldController.Instance is null)
			{
				CardSelectionMenu.Instantiate.CardSceneEventHandler(CardEvents.LeftCllick,
					int.Parse(Name));
			}
			else
			{
				GameFieldController.Instance.CardSceneEventHandler(CardEvents.LeftCllick,
					int.Parse(Name));
			}
		}
	}

	public void SetParams(Vector2 rectSize, string texturePath, CardDataBase.CardData card,
		bool disconnectSignals = false)
	{
		this.disconnectSignals = disconnectSignals;
		Name = card.id.ToString();
		GetNode<TextureButton>("Card").TooltipText = $"Вес: {card.strength}\nСпецифика: {card.category}\n{card.text}";
		GetNode<TextureButton>("Card").TextureNormal = (Texture2D)GD.Load(texturePath);
		float scaleFactorY = rectSize.Y / initRectSize.Y;
		float scaleFactorX = rectSize.X / initRectSize.X;
		this.Scale = new Vector2(scaleFactorX, scaleFactorY);

		CardDamage = CardDataBase.GetCardInfo(card.id).strength;
		GetNode<Label>("LabelsContainer/VBoxContainer/HBoxContainer/Strength").Text = CardDamage.ToString();
		GetNode<Label>("LabelsContainer/VBoxContainer/HBoxContainer2/Paragraph").Text = card.category.ToString();
		GetNode<Label>("LabelsContainer/MainText").Text = card.text;
	}

	public void SetDamage(int damage)
	{
		CardDamage = damage;
	}
}







