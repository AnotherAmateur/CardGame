using CardGameProj.SeparateClasses;
using Godot;
using System;
using System.Text;

public partial class RoomMenu : Control
{
	private HttpRequest httpRequest;

	public override void _Ready()
	{
		httpRequest = GetNode<HttpRequest>("HTTPRequest");
	}

	private void _on_JoinRoomButton_pressed()
	{
		PackedScene roomListScene = (PackedScene)GD.Load("res://RoomList.tscn");
		RoomList roomListInstance = (RoomList)roomListScene.Instantiate();
		AddChild(roomListInstance);
	}

	private void _on_http_request_request_completed(long result, long response_code, string[] headers, byte[] body)
	{
		if (response_code == 200)
		{
			try
			{
				var json = Json.ParseString(Encoding.UTF8.GetString(body));

				if (json.Obj is null)
				{
					throw new Exception("Unexpected server response (500)");
				}

				States.GameId = (int)json.Obj;
				GetTree().ChangeSceneToPacked((PackedScene)GD.Load("res://CardSelectionMenu.tscn"));
			}
			catch (Exception ex)
			{
				GetNode<Label>("StatusLabel").Text = ex.Message;
			}
		}
		else
		{
			GetNode<Label>("StatusLabel").Text = "Не удалось присоединиться. Код: " + response_code;
		}
	}

	private void _on_create_room_button_pressed()
	{
		PackedScene waiterScene = (PackedScene)GD.Load("res://WaiterScene.tscn");
		WaiterScene waiterInstance = (WaiterScene)waiterScene.Instantiate();
		AddChild(waiterInstance);
	}
}
