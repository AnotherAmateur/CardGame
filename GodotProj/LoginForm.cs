using Godot;
using Godot.Collections;
using System;
using System.Reflection.Metadata;
using System.Text;

public class LoginForm : Control
{
	private HTTPRequest httpRequest;
	public static int PlayerId { get; private set; }
	public static string Login { get; private set; }


	public override void _Ready()
	{
		OS.SetWindowSize(new Vector2(1280, 720));
		httpRequest = GetNode<HTTPRequest>("HTTPRequest");
	}


	private void _on_Reg_pressed()
	{
		GetNode<Button>("Reg").Disabled = true;
		GetNode<Button>("Auth").Disabled = true;

		Login = GetNode<LineEdit>("Login").Text;
		string password = GetNode<LineEdit>("Password").Text;

		Dictionary<string, object> postData = new Dictionary<string, object>()
		{
			{ "Login", Login },
			{ "Password", password }
		};

		var postJson = JSON.Print(postData);
		string[] headers = new string[] { "Content-Type: application/json" };
		string url = "https://localhost:7135/api/Game/registration";
		httpRequest.Request(url, headers, false, HTTPClient.Method.Post, postJson);
	}


	private void _on_Auth_pressed()
	{
		GetNode<Button>("Reg").Disabled = true;
		GetNode<Button>("Auth").Disabled = true;

		Login = GetNode<LineEdit>("Login").Text;
		string password = GetNode<LineEdit>("Password").Text;

		Dictionary<string, object> postData = new Dictionary<string, object>()
		{
			{ "Login", Login },
			{ "Password", password }
		};

		var postJson = JSON.Print(postData);
		string[] headers = new string[] { "Content-Type: application/json" };
		string url = "https://localhost:7135/api/Game/login";
		httpRequest.Request(url, headers, false, HTTPClient.Method.Post, postJson);
	}


	private void _on_HTTPRequest_request_completed(int result, int response_code, string[] headers, byte[] body)
	{
		GetNode<Button>("Reg").Disabled = false;
		GetNode<Button>("Auth").Disabled = false;

		if (response_code == 200)
		{
			try
			{
				JSONParseResult json = JSON.Parse(Encoding.UTF8.GetString(body));
				if (json.Error != Error.Ok)
				{
					throw new Exception("Unexpected server response (500)");
				}

				PlayerId = int.Parse(json.Result.ToString());

				GetTree().ChangeSceneTo((PackedScene)GD.Load("res://CardSelectionMenu.tscn"));
			}
			catch (Exception ex)
			{
				GetNode<Label>("AuthStatus").Text = ex.Message;
			}
		}
		else
		{
			GetNode<Label>("AuthStatus").Text = "Ошибка входа/регистрации. Код: " + response_code;
		}
	}


	private void _on_Button_pressed()
	{
		Login = "admin";
		PlayerId = 1;
		GetTree().ChangeSceneTo((PackedScene)GD.Load("res://RoomMenu.tscn"));
	}
}
