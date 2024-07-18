using Godot;
using System;

public partial class Trap : Node2D
{
    public game_manager gm;

    private Area2D spear;
    private ActionsData spearData = new ActionsData();

    private Area2D LeftArea;
    private Area2D RightArea;
    private Timer LeftTimer;
    private Timer RightTimer;
    private AnimationPlayer spearAnimations;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        gm = GetNode<game_manager>("/root/GM");
        LeftArea = GetNode<Area2D>("LeftArea2D");
        RightArea = GetNode<Area2D>("RightArea2D");
        LeftTimer = GetNode<Timer>("LeftArea2D/TimerBeforeAct");
        RightTimer = GetNode<Timer>("RightArea2D/TimerBeforeAct");
        spearAnimations = GetNode<AnimationPlayer>("Spear/SpearAnimations");
        spear = GetNode<Area2D>("Spear");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!Input.IsActionPressed("TimeRevind"))
        {
            if (LeftArea.HasOverlappingBodies() && LeftTimer.IsStopped())
            {
                if (!spearAnimations.IsPlaying() && RightTimer.IsStopped())
                    LeftTimer.Start();
            }
            else if (RightArea.HasOverlappingBodies() && RightTimer.IsStopped())
            {
                if (!spearAnimations.IsPlaying() && LeftTimer.IsStopped())
                    RightTimer.Start();
            }
            spearData.AddNewPosition(spear.Transform);
        }
        else
        {
            spearAnimations.Stop();
            spear.Transform = spearData.TimeRevind();
            spearData.DeleteLastPosition();
        }
        if (Input.IsActionJustReleased("TimeRevind"))
        {
            RightTimer.Stop();
            LeftTimer.Stop();
            spearAnimations.Stop();
        }
    }

    public void _on_spear_body_entered(Node2D body)
    {
        gm.ResetGame();
    }

    public void _on_right_area_2d_body_entered(Node2D body)
    {
        if (!spearAnimations.IsPlaying() && LeftTimer.IsStopped() && !Input.IsActionPressed("TimeRevind"))
            RightTimer.Start();
    }
    public void _on_left_area_2d_body_entered(Node2D body)
    {
        if (!spearAnimations.IsPlaying() && RightTimer.IsStopped() && !Input.IsActionPressed("TimeRevind"))
            LeftTimer.Start();
    }
    public void ONRightTimerOut()
    {
        spearAnimations.Play("GoesRight");
    }
    public void OnLeftTimerOut()
    {
        spearAnimations.Play("GoesLeft");
    }
    public void _on_spear_animation_finished(String animationName)
    {
        RightTimer.Stop();
        LeftTimer.Stop();
    }
}
