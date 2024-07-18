using Godot;
using System;

public partial class Tramplin : Area2D
{
   
    public override void _Ready()
    {
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        
    }

    public void _on_body_entered(CharacterBody2D body)
    {
        if (!Input.IsActionPressed("TimeRevind"))
        {
            body.Velocity = new Vector2(body.Velocity.X, -2000f);
        }
    }
}
