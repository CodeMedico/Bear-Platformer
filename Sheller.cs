using Godot;
using System;

public partial class Sheller : Slime
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        base._Process(delta);
	}

	public override void _on_body_entered(CharacterBody2D body)
    {
        if (!Input.IsActionPressed("TimeRevind") && slimeTop.IsColliding() && body.Velocity.Y > 0)
        {
            body.Velocity = new Vector2(0, -2000f);
        }
        else
        {
            gm.ResetGame();
        }
    }
}
