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
		socketConnection.Connect();
		socketConnection.JoinGroup();
	}

	public void OnHandleError(string exMessage)
	{
		PackedScene messageBoxScene = (PackedScene)GD.Load("res://message_box.tscn");
		MessageBox messageBox = (MessageBox)messageBoxScene.Instantiate(PackedScene.GenEditState.Instance);
		messageBox.SetUp(exMessage, true);
		AddChild(messageBox);
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
		socketConnection.Disconnect();
		QueueFree();
	}
}
