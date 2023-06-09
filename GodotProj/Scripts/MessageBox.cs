using Godot;
using System;

public partial class MessageBox : Control
{
	private bool goToRoomMenu;

	public override void _Ready() { }

	public void SetUp(string msg, bool btn, bool goToRoomMenu = false)
	{
		this.goToRoomMenu = goToRoomMenu;
		var label = GetNode<Label>("ColorRect/Label");

		if (btn is false)
		{
			var fullRectSize = GetNode<ColorRect>("ColorRect").Size;
			label.Size = fullRectSize;
			label.Position = new Vector2(0, 0);
		}

		label.Text = msg;
		GetNode<Button>("ColorRect/CloseButton").Visible = btn;
	}

	private void _on_close_button_pressed()
	{
		if (goToRoomMenu)
		{
			GetTree().ChangeSceneToPacked((PackedScene)GD.Load("res://RoomMenu.tscn"));
		}

		QueueFree();
	}
}

