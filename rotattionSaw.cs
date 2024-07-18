using Godot;
using System;

public partial class rotattionSaw : Node2D
{
	private AnimationPlayer animation;
	
	public override void _Ready()
	{
		animation = GetNode<AnimationPlayer>("Node2D/AnimationPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("TimeRevind"))
		{
			animation.PlayBackwards("fly");
		}
		else if (!Input.IsActionPressed("TimeRevind"))
		{
			animation.Play("fly");
		}
	}
}
