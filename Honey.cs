using Godot;
using System;
using System.Threading.Tasks;

public partial class Honey : Area2D
{
    public game_manager gm;
    private Vector2 initialPosition;
    private AnimationPlayer animation;
    public void _on_body_entered(PlayerBear body)
    {
        GD.Print("collect Honey");
        body.HoneyColleted();
        Position = new Vector2(2000, 2000);
        Visible = false;
    }
    public override void _Ready()
    {
        initialPosition = Position;
        animation = GetNode<AnimationPlayer>("AnimationPlayer");
        gm = GetNode<game_manager>("/root/GM");
        gm.OnResetGame += Gm_OnResetGame;
        gm.AddHoney();
    }
    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("TimeRevind"))
        {
            animation.PlayBackwards();
        }
        else
        {
            animation.Play();
        }
    }

    private async void Gm_OnResetGame(object sender, EventArgs e)
    {
        await Task.Delay(300);
        Position = initialPosition;
        Visible= true;
    }

    public void _on_tree_exited()
    {
        gm.OnResetGame -= Gm_OnResetGame;
    }
}
