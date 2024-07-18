using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public partial class Portal : Area2D
{
    [Export] public Portal PairPortal;
    public Camera2D Camera;
    public Timer PortalDelay;
    public bool PortalInactive = false;
    public AnimationPlayer animation;
    public override void _Ready()
    {
        PortalDelay = GetNode<Timer>("PortalDelay");
        animation = GetNode<AnimationPlayer>("AnimationPlayer");
        Camera = GetNode<Camera2D>("%MainCamera");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
    public async void _on_body_entered(CharacterBody2D body)
    {
        if (!PortalInactive)
        {
            Camera.PositionSmoothingEnabled = false;
            PairPortal.PortalInactive = true;
            PairPortal.animation.Play("PortalAnimation");
            PairPortal.PortalDelay.Start();
            body.Position = PairPortal.Position;
            await Task.Delay(50);
            Camera.PositionSmoothingEnabled = true;
        }
    }
    public void _on_area_entered(Area2D body)
    {
        if (!PortalInactive)
        {
            PairPortal.PortalInactive = true;
            PairPortal.animation.Play("PortalAnimation");
            PairPortal.PortalDelay.Start();
            body.GetParent<Node2D>().Position = PairPortal.Position;
        }
    }

    public void _on_portal_delay_timeout()
    {
        PortalDelay.Stop();
        PortalInactive = false;
    }

    public void OnAnimationFinished(string animation)
    {
        if (animation == "PortalAnimation")
        {
            this.animation.Play("Idle");
        }
    }
}
