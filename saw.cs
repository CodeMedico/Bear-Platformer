using Godot;
using System;

public partial class saw : Area2D
{
    private game_manager gm;
    private AnimationPlayer sawAnim;

    public override void _Ready()
    {
        gm = GetNode<game_manager>("/root/GM");
        sawAnim = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("TimeRevind"))
        {
            sawAnim.PlayBackwards();
        }
        else
        {
            sawAnim.Play();
        }
    }
    public void OnBodyEntered(Node2D body)
    {
        gm.ResetGame();
    }
}
