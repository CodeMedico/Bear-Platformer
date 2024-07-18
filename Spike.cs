using Godot;
using System;

public partial class Spike : Area2D
{
    protected game_manager gm;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        gm = GetNode<game_manager>("/root/GM");
    }

    public override void _Process(double delta)
    {

    }
    public void _on_body_entered(CharacterBody2D body)
    {
        GD.Print(body.Velocity.Y);
        var bear = (PlayerBear)body;
        if (bear.Velocity.Y > 840)
        {
            gm.ResetGame();
        }
        
    }
}
