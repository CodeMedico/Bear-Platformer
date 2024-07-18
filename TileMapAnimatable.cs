using Godot;
using System;

public partial class TileMapAnimatable : TileMap
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Scale = new Vector2(2,2);
		this.CollisionAnimatable = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!this.CollisionAnimatable)
		{
            this.CollisionAnimatable = true;
        }
	}
}
