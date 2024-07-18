using Godot;
using System;

public partial class Switcher : Node2D
{
    private game_manager gm;

    private Transform2D inititalPosition;
    public AnimatedSprite2D sprite2D;
    public Area2D Area2D;
    public bool bodyInRange = false;
    public bool switherOff = true;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        gm = GetNode<game_manager>("/root/GM");
        gm.OnResetGame += Gm_OnResetGame;
    }

    private void Gm_OnResetGame(object sender, EventArgs e)
    {
        sprite2D.Play("SwitchOff");
        switherOff = true;
    }
    public void _on_tree_exited()
    {
        gm.OnResetGame -= Gm_OnResetGame;
    }
    public void _on_area_2d_body_entered(Node2D body)
    {
        bodyInRange = true;
    }

    public void _on_area_2d_body_exited(Node2D body)
    {
        bodyInRange = false;
    }
    public void _on_animated_sprite_2d_animation_finished()
    {
        if (switherOff)
        {
            sprite2D.Play("SwitchOff");
        }
        else
        {
            sprite2D.Play("SwitchOn");
        }
    }
}
