using Godot;
using System;
using System.Threading.Tasks;

public partial class DeathTransition : CanvasLayer
{
    protected game_manager gm;
    private AnimationPlayer deathAnimation;
    public override void _Ready()
	{
        gm = GetNode<game_manager>("/root/GM");
        gm.OnResetGame += Gm_OnResetGame;
        deathAnimation = GetNode<AnimationPlayer>("ColorRect/AnimationPlayer");
    }

    private void Gm_OnResetGame(object sender, EventArgs e)
    {
        deathAnimation.Play("disolve");
    }
    public void _on_tree_exiting()
    {
        gm.OnResetGame -= Gm_OnResetGame;
    }
}
