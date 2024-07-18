using Godot;
using System;

public partial class PendulumSaw : Node2D
{
    private AnimationPlayer sawAnim;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        sawAnim = GetNode<AnimationPlayer>("AnimationPlayer");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
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
}
