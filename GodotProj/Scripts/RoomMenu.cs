using CardGameProj.SeparateClasses;
using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

public partial class RoomMenu : Control
{
	private HttpRequest httpRequest;

	public override void _Ready()
	{
		States.PVE = false;
		httpRequest = GetNode<HttpRequest>("HTTPRequest");
		GetPlayerInfo();       
	}

	private void GetPlayerInfo()
	{
		string json = Json.Stringify(States.PlayerId);
		string[] headers = new string[] { "Content-Type: application/json" };
		string url = States.Url + "api/Game/getplayer";
		httpRequest.Request(url, headers, HttpClient.Method.Post, json);
	}

	private void _on_JoinRoomButton_pressed()
	{
		PackedScene roomListScene = (PackedScene)GD.Load("res://RoomList.tscn");
		RoomList roomListInstance = (RoomList)roomListScene.Instantiate();
		AddChild(roomListInstance);
	}

	private void _on_http_request_request_completed(long _, long response_code, string[] headers, byte[] body)
	{
		if (response_code == 200)
		{
			try
			{
				string resultString = System.Text.Encoding.UTF8.GetString(body);
				string[] playerInfo = resultString.Split(";", StringSplitOptions.RemoveEmptyEntries);
				GetNode<Label>("PlayerInfoPanel/HBoxContainer/LoginLabel").Text = playerInfo[0];
				GetNode<Label>("PlayerInfoPanel/HBoxContainer/RatingLabel").Text = playerInfo[1];
			}
			catch (Exception ex)
			{
				OS.Alert("Ошибка \n" + ex.Message);
				GetTree().Quit();
			}
		}
		else
		{
			OS.Alert("Ошибка. Код: " + response_code);
			GetTree().Quit();
		}
	}

	private void _on_create_room_button_pressed()
	{
		PackedScene waiterScene = (PackedScene)GD.Load("res://WaiterScene.tscn");
		WaiterScene waiterInstance = (WaiterScene)waiterScene.Instantiate();
		AddChild(waiterInstance);
	}

	private void _on_pve_btn_pressed()
	{
		States.PVE = true;
		GetTree().ChangeSceneToPacked((PackedScene)GD.Load("res://CardSelectionMenu.tscn"));
	}
}
