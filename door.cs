using Godot;
using System;
using System.Threading.Tasks;

public partial class door : Node2D
{
    [Export] public Switcher switcher;
    private game_manager gm;
    public AnimationPlayer doorAnimation;
    private Vector2 initialPosition;
    public bool bodyInRange = false;
	public override void _Ready()
	{
		doorAnimation = GetNode<AnimationPlayer>("DoorCloseOpen");
        gm = GetNode<game_manager>("/root/GM");
        gm.OnResetGame += Gm_OnResetGame;
        initialPosition = GetNode<RigidBody2D>("RigidBody2D").Position;
    }


    private async void Gm_OnResetGame(object sender, EventArgs e)
    {
        await Task.Delay(200);
        GetNode<RigidBody2D>("RigidBody2D").Position = initialPosition;
    }
	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("Interact") && switcher.bodyInRange)
        {
            if (switcher.switherOff)
            {
                switcher.sprite2D.Play("SwitchTurnOn");
                switcher.switherOff = false;
                doorAnimation.Play("DoorCloseOpen");
            }
            else
            {
                switcher.sprite2D.Play("SwitchTurnOff");
                switcher.switherOff = true;
                doorAnimation.PlayBackwards("DoorCloseOpen");
            }
        }
    }
    public void _on_tree_exited()
    {
        gm.OnResetGame -= Gm_OnResetGame;
    }
}
