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
            if (GFieldController.Instance is null)
			{
                CardSelectionMenu.Instantiate.CardSceneEventHandler(CardEvents.LeftCllick,
					int.Parse(Name));
			}
			else
			{
                GFieldController.Instance.CardSceneEventHandler(CardEvents.LeftCllick,
					int.Parse(Name));
			}
		}
	}

	public void SetParams(Vector2 rectSize, string texturePath, CardDB.CardData card,
		bool disconnectSignals = false)
	{
		this.disconnectSignals = disconnectSignals;
		Name = card.Id.ToString();
		GetNode<TextureButton>("Card").TooltipText = $"Вес: {card.Strength}\nСпецифика: {card.Category}\n{card.Text}";
		GetNode<TextureButton>("Card").TextureNormal = (Texture2D)GD.Load(texturePath);
		float scaleFactorY = rectSize.Y / initRectSize.Y;
		float scaleFactorX = rectSize.X / initRectSize.X;
		this.Scale = new Vector2(scaleFactorX, scaleFactorY);

		CardDamage = CardDB.GetCardInfo(card.Id).Strength;
		GetNode<Label>("LabelsContainer/VBoxContainer/HBoxContainer/Strength").Text = CardDamage.ToString();
		GetNode<Label>("LabelsContainer/VBoxContainer/HBoxContainer2/Paragraph").Text = card.Category.ToString();
		GetNode<Label>("LabelsContainer/MainText").Text = card.Text;

        if (card.Synergy is false)
            GetNode<Sprite2D>("SynergyIcon").Visible = false;
    }

	public void SetDamage(int damage)
	{
		CardDamage = damage;
	}
}