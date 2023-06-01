using CardGameProj.Scripts;
using CardGameProj.SeparateClasses;
using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using System;

public class SocketConnection : ISocketConn
{
	private readonly static HubConnection connection;
	private readonly static string url;
	private ISocketConn scene;
	private static SocketConnection instance;

	static SocketConnection()
	{
		url = States.Url + "ws";
		connection = new HubConnectionBuilder().WithUrl(url).Build();	
	}

	private SocketConnection()
	{
		connection.On<string, string, string>("send", OnReceiveMessage);
		connection.On<string>("OnHandleError", OnHandleError);
	}

	public static SocketConnection GetInstance(ISocketConn scene)
	{
		if (instance is null)
		{
			instance = new SocketConnection();
		}

		instance.scene = scene;

		return instance;
	}

	public async void Connect()
	{
		try
		{
			await connection.StartAsync();
		}
		catch (Exception ex)
		{
			OS.Alert("SocketConnection/Connect: " + ex.Message);
		}
	}

	public void OnHandleError(string exMessage)
	{
		scene.OnHandleError(exMessage);
	}

	public async void JoinGroup()
	{
		await connection.SendAsync("JoinGroup", States.MasterId.ToString());
	}

	public void OnReceiveMessage(string action, string masterId, string message)
	{
		scene.OnReceiveMessage(action, masterId, message);
	}

	public async void Disconnect()
	{
		await connection.StopAsync();
		await connection.DisposeAsync();
		instance = null;
	}

	public async void Send(ActionTypes action, int masterId, string message)
	{
		try
		{
			await connection.SendAsync("SendMessage", action.ToString(), masterId.ToString(), message);
		}
		catch (Exception ex)
		{
			OS.Alert("SocketConnection/Send: " + ex.Message);
		}
	}
}
