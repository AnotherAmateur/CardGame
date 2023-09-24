using CardGameProj.Scripts;
using CardGameProj.SeparateClasses;
using Godot;

public partial class WaiterScene : Control, ISocketConn
{
	SocketConnection socketConnection;
	public override void _Ready()
	{
		States.MasterId = States.PlayerId;
		socketConnection = SocketConnection.GetInstance(this);
		socketConnection.ConnectAsync();
		socketConnection.JoinGroupAsync();
	}

	public void OnHandleError(string exMessage)
	{
		var msgBox = MessageBox.Instance;
		msgBox.SetUp(exMessage, true);
		AddChild(msgBox);
	}

	public void OnReceiveMessage(string action, string masterId, string message)
	{
		if (action == ActionTypes.Start.ToString())
		{
			if (States.MasterId != int.Parse(masterId))
			{
				OS.Alert("WaiterScene/OnReceiveMessage");
				return;
			}

			GetTree().ChangeSceneToPacked((PackedScene)GD.Load("res://CardSelectionMenu.tscn"));
		}
	}

	private void _on_close_button_pressed()
	{
		socketConnection.DisconnectAsync();
		QueueFree();
	}
}
