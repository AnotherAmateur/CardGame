using CardGameProj.Scripts;
using CardGameProj.SeparateClasses;
using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

public partial class RoomList : Control, ISocketConn
{
	private HttpRequest httpRequest;
	private VBoxContainer listContainer;
	private SocketConnection socketConnection;
	private Label StatusLabel;
	private Button RefreshButton;
	private float buttonWidth;
	private float buttonHeight;

	public override void _Ready()
	{
		StatusLabel = GetNode<Label>("StatusLabel");
		RefreshButton = GetNode<Button>("RefreshButton");
		listContainer = GetNode<VBoxContainer>("ScrollContainer/VBoxContainer");
		buttonWidth = GetNode<ScrollContainer>("ScrollContainer").Size.X;
		buttonHeight = 50;

		httpRequest = GetNode<HttpRequest>("HTTPRequest");

		RefreshRoomList();
	}

	private void RefreshRoomList()
	{
		RefreshButton.Disabled = true;
		StatusLabel.Text = "";

		foreach (var item in listContainer.GetChildren())
		{
			listContainer.RemoveChild(item);
		}

		string[] headers = new string[] { "Content-Type: application/json" };
		string url = States.Url + "api/Game/getlobbies";		
		httpRequest.Request(url, headers, HttpClient.Method.Get);
	}

	private void _on_HTTPRequest_request_completed(int result, int response_code, string[] headers, byte[] body)
	{
		RefreshButton.Disabled = false;

		if (response_code == 200)
		{
			try
			{
				var json = Json.ParseString(Encoding.UTF8.GetString(body));

				if (json.Obj is null)
				{
					throw new Exception("Unexpected server response (500)");
				}

				List<string> resultList = JsonSerializer.Deserialize<List<string>>(json.Obj.ToString());

				if (resultList is null || resultList.Count == 0)
				{
					StatusLabel.Text = "Нет свободных комнат";
					return;
				}

				foreach (var item in resultList)
				{
					string[] idRatName = item.Split(";", StringSplitOptions.RemoveEmptyEntries);

					var control = new Control();
					var button = new Button();

					button.CustomMinimumSize = new Vector2(buttonWidth, buttonHeight);
					button.Text = "Мастер комнаты:  " + idRatName[2] + "      Рейтинг:  " + idRatName[1];
					button.Name = idRatName[0];
					button.ButtonDown += () => _on_Room_click(button);

					control.CustomMinimumSize = new Vector2(buttonWidth, buttonHeight);
					control.AddChild(button);
					listContainer.AddChild(control);
				}
			}
			catch (Exception ex)
			{
				StatusLabel.Text = ex.Message;
			}
		}
		else
		{
			StatusLabel.Text = "Список комнат недоступен. Код: " + response_code;
		}
	}

	private void _on_Room_click(Button button)
	{
		States.MasterId = int.Parse(button.Name);

		socketConnection = SocketConnection.GetInstance(this);
		socketConnection.ConnectAsync();
		socketConnection.JoinGroupAsync();
	}

	private void _on_CloseButton_pressed()
	{
		QueueFree();
	}

	private void _on_RefreshButton_pressed()
	{
		RefreshRoomList();
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
			States.MasterId = int.Parse(masterId);
			GetTree().ChangeSceneToPacked((PackedScene)GD.Load("res://CardSelectionMenu.tscn"));
		}
	}
}










