using Godot;
using Godot.Collections;
using System;
using System.Text;

public class LoginForm : Control
{
	private HTTPRequest httpRequest;
	public int PlayerId { get; private set; }
	public string Login { get; private set; }


	public override void _Ready()
	{	
		OS.SetWindowSize(new Vector2(960, 540));
		httpRequest = GetNode<HTTPRequest>("HTTPRequest");
	}


	private void _on_Reg_pressed()
	{
		string login = GetNode<LineEdit>("Login").Text;
		string password = GetNode<LineEdit>("Password").Text;

		Dictionary<string, object> postData = new Dictionary<string, object>()
		{
			{ "Login", login },
			{ "Password", password }
		};

		var postJson = JSON.Print(postData);
		string[] headers = new string[] { "Content-Type: application/json" };
		string url = "https://localhost:7135/api/Game/registration";
		httpRequest.Request(url, headers, false, HTTPClient.Method.Post, postJson);
	}


	private void _on_Auth_pressed()
	{
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
		if (response_code == 200)
		{
			try
			{
				JSONParseResult json = JSON.Parse(Encoding.UTF8.GetString(body));
				PlayerId = (int)json.Result;

				GetTree().ChangeSceneTo((PackedScene)GD.Load("res://CardSelectionMenu.tscn"));
			}
			catch (Exception)
			{
				GetNode<Label>("AuthStatus").Text = "Код: 500";
			}		
		}
		else
		{
			GetNode<Label>("AuthStatus").Text = "Что-то пошло не так";
		}
	}
}
