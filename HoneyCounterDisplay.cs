using Godot;
using System;

public partial class HoneyCounterDisplay : BoxContainer
{
	[Export] private PlayerBear characterBody2D;
	private game_manager gm;
	private SceneTransition st;
	private Label honeyCountLabel;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gm = GetNode<game_manager>("/root/GM");
		st = GetNode<SceneTransition>("/root/SceneTransition");
		honeyCountLabel = GetNode<Label>("Label");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		honeyCountLabel.Text = "x "+characterBody2D.honeyCount.ToString()+"/" +gm.honeyCount.ToString();
		if(characterBody2D.honeyCount == gm.honeyCount)
		{
			gm.currentLevel++;
			gm.honeyCount = 0;
			st.SceneTransitionMethod("res://Level" + gm.currentLevel.ToString() + ".tscn");
		}
	}
}
