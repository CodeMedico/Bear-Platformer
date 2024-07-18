using Godot;
using System;
using System.Threading.Tasks;

public partial class SawFly : Node2D
{
	public game_manager gm;

	public Area2D sawFly;
	public RayCast2D rayCast;
	private Vector2 flyDirection = new Vector2(-5f, 0f);

	private Transform2D inititalPosition;
	private Vector2 inititalScale;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		sawFly = GetNode<Area2D>("Saw");
		rayCast = GetNode<RayCast2D>("RayCast2D");

		gm = GetNode<game_manager>("/root/GM");
        gm.OnResetGame += Gm_OnResetGame;

		inititalPosition = Transform;
		inititalScale = Scale;
	}

    private void Gm_OnResetGame(object sender, EventArgs e)
    {
		Transform = inititalPosition;
		flyDirection = new Vector2(-5f, 0f);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		Position += flyDirection;
		if (rayCast.IsColliding() || Input.IsActionJustPressed("TimeRevind") || Input.IsActionJustReleased("TimeRevind"))
		{
			flyDirection = -flyDirection;
			Scale = new Vector2(-Scale.X, Scale.Y);
		}
	}
	public void _on_tree_exited()
	{
		gm.OnResetGame -= Gm_OnResetGame;
	}
}
