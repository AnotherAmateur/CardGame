using Godot;
using System;

public partial class MessageBox : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{		
	}

	public void SetMessage(string msg)
	{
		GetNode<Label>("ColorRect/Label").Text = msg;
	}
}
