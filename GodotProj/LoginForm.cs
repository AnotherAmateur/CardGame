using Godot;
using Godot.Collections;
using System;
using System.Reflection.Metadata;
using System.Text;
using System.Diagnostics;
using CardGameProj.SeparateClasses;
using System.Text.Json;

public partial class LoginForm : Control
{
	private HttpRequest httpRequest;

	public override void _Ready()
	{
		httpRequest = GetNode<HttpRequest>("HTTPRequest");
	}

	private void _on_Reg_pressed()
	{
		if (GetNode<LineEdit>("Login").Text.Length < 1 || GetNode<LineEdit>("Password").Text.Length < 1)
		{
			PackedScene messageBoxScene = (PackedScene)GD.Load("res://message_box.tscn");
			MessageBox messageBox = (MessageBox)messageBoxScene.Instantiate(PackedScene.GenEditState.Instance);
			messageBox.SetUp("Пароль и логин должны содержать не менее 1 символа", true);
			AddChild(messageBox);

			return;
		}

		GetNode<Button>("Reg").Disabled = true;
		GetNode<Button>("Auth").Disabled = true;

		States.Login = GetNode<LineEdit>("Login").Text;
		string password = GetNode<LineEdit>("Password").Text;

		Dictionary<string, string> postData = new Dictionary<string, string>()
		{
			{ "login", States.Login },
			{ "password", password }
		};

		string json = Json.Stringify(postData);

		string[] headers = new string[] { "Content-Type: application/json" };
		string url = States.Url + "api/Game/registration";
		httpRequest.Request(url, headers, HttpClient.Method.Post, json);
	}

	private void _on_Auth_pressed()
	{
		if (GetNode<LineEdit>("Login").Text.Length < 1 || GetNode<LineEdit>("Password").Text.Length < 1)
		{
			PackedScene messageBoxScene = (PackedScene)GD.Load("res://message_box.tscn");
			MessageBox messageBox = (MessageBox)messageBoxScene.Instantiate(PackedScene.GenEditState.Instance);
			messageBox.SetUp("Пароль и логин должны содержать не менее 1 символа", true);
			AddChild(messageBox);

			return;
		}

		GetNode<Button>("Reg").Disabled = true;
		GetNode<Button>("Auth").Disabled = true;

		States.Login = GetNode<LineEdit>("Login").Text;
		string password = GetNode<LineEdit>("Password").Text;

		Dictionary<string, string> postData = new Dictionary<string, string>()
		{
			{ "login", States.Login },
			{ "password", password }
		};

		string json = Json.Stringify(postData);

		string[] headers = new string[] { "Content-Type: application/json" };
		string url = States.Url + "api/Game/login";
		httpRequest.Request(url, headers, HttpClient.Method.Post, json);
	}

	private void _on_http_request_request_completed(long result, long response_code, string[] headers, byte[] body)
	{
		GetNode<Button>("Reg").Disabled = false;
		GetNode<Button>("Auth").Disabled = false;

		if (response_code == 200)
		{
			try
			{
				var json = Json.ParseString(Encoding.UTF8.GetString(body));

				if (json.Obj is null)
				{
					throw new Exception("Unexpected server response (500)");
				}

				States.PlayerId = JsonSerializer.Deserialize<int>(json.Obj.ToString());
				if (States.PlayerId < 1)
				{
					OS.Alert("Response parsing error");
					return;
				}

				GetTree().ChangeSceneToPacked((PackedScene)GD.Load("res://RoomMenu.tscn"));
			}
			catch (Exception ex)
			{
				GetNode<Label>("AuthStatus").Text = ex.Message;
			}
		}
		else
		{
			GetNode<Label>("AuthStatus").Text = "Ошибка. Код: " + response_code;
		}
	}

	private void _on_check_button_toggled(bool button_pressed)
	{
		if (button_pressed is true)
		{
			States.Url = "http://localhost:7136/";
		}
		else
		{
			States.Url = "http://bloghda-001-site1.htempurl.com/";
		}
	}
}




