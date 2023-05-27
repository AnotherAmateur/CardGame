using Godot;
using System;

public class RoomMenu : Control
{

	public override void _Ready()
	{
		//	GetTree().ChangeSceneTo((PackedScene)GD.Load("res://RoomMenu.tscn"));
	}


	private void _on_JoinRoomButton_pressed()
	{
		PackedScene roomListScene = (PackedScene)GD.Load("res://RoomList.tscn");
		RoomList roomListInstance = (RoomList)roomListScene.Instance();
		AddChild(roomListInstance);
	}


	private void _on_CreateRoomButton_pressed()
	{
		
	}
}
