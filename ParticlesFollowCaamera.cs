using Godot;
using System;

public partial class ParticlesFollowCaamera : GpuParticles2D
{
	private Camera2D mainCamera;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		mainCamera = GetNode<Camera2D>("%MainCamera");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = new Vector2(mainCamera.GlobalPosition.X, GlobalPosition.Y);
	}
}
