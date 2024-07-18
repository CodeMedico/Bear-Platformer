using Godot;
using System;

public partial class GravitySphere : Node2D
{
	private Area2D area;

	[Export(PropertyHint.Range,"50,400,")]
	private float gravityPower = 150f;
	[Export(PropertyHint.Range,"0.5,5,")]
	private float coeficitent = 1.5f;
	public override void _Ready()
	{
		area = GetNode<Area2D>("Area2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (area.HasOverlappingBodies())
		{
			var bodies = area.GetOverlappingBodies();
			foreach ( var body in bodies )
			{
				if (body is PlayerBear)
				{
					var bear = (PlayerBear)body;
					bear.Velocity += (bear.Position - Position) * gravityPower/ (Position.DistanceTo(bear.Position)/coeficitent);
				}
			}
		}
	}
}
