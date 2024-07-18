using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class UnstableBlock : StaticBody2D
{
    private game_manager gm;

    private ActionsData blockData = new ActionsData();  

    private RayCast2D[] rayCast2Ds = new RayCast2D[4];

    private LightOccluder2D lightOccluder2D;
    private CollisionShape2D collisionShape2D;
    private Sprite2D sprite2D;
    private GpuParticles2D particles2D;
    private Timer timerCollapse;
    private AnimationPlayer animationPlayer;

    private Vector2 initialPosition;

    private bool firstTimeBack = true;
    public override void _Ready()
    {
        timerCollapse = GetNode<Timer>("Timer");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        rayCast2Ds[0] = GetNode<RayCast2D>("RayCast2D");
        rayCast2Ds[1] = GetNode<RayCast2D>("RayCast2D2");
        rayCast2Ds[2] = GetNode<RayCast2D>("RayCast2D3");
        rayCast2Ds[3] = GetNode<RayCast2D>("RayCast2D4");
        particles2D = GetNode<GpuParticles2D>("GPUParticles2D");
        collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        lightOccluder2D = GetNode<LightOccluder2D>("LightOccluder2D");
        sprite2D = GetNode<Sprite2D>("Sprite2D");
        gm = GetNode<game_manager>("/root/GM");
        gm.OnResetGame += Gm_OnResetGame;
        initialPosition = GlobalPosition;
    }

    private async void Gm_OnResetGame(object sender, EventArgs e)
    {
        timerCollapse.Stop();
        animationPlayer.Stop();
        //await Task.Delay(200);
        await RemoveBlock();
        GlobalPosition = initialPosition;
        sprite2D.Visible = true;
        collisionShape2D.Disabled = false;
        lightOccluder2D.Visible = true;
    }

    public override void _Process(double delta)
    {
        if (!Input.IsActionPressed("TimeRevind"))
        {
            if (rayCast2Ds.Any(rayCast2D => rayCast2D.IsColliding()) && timerCollapse.IsStopped())
            {
                timerCollapse.Start();
                animationPlayer.Play("beforeCollapse");
            }
            blockData.AddNewPosition(Transform);
        }
        else
        {
            Transform = blockData.TimeRevind();
            blockData.DeleteLastPosition();
            if(Position != new Vector2(20000, 20000) && !firstTimeBack)
            {
                particles2D.Emitting = true;
                firstTimeBack = true;
                BlockBack();
            }
        }

    }

    public void _on_timer_timeout()
    {
        timerCollapse.Stop();
        sprite2D.Visible = false;
        particles2D.Emitting = true;
        collisionShape2D.Disabled = true;
        lightOccluder2D.Visible = false;
        RemoveBlock();
    }

    public async Task RemoveBlock()
    {
        await Task.Delay(200);
        Position = new Vector2(20000, 20000);
        firstTimeBack = false;
    }
    public void _on_tree_exited()
    {
        gm.OnResetGame -= Gm_OnResetGame;
    }

    private async void BlockBack()
    {
        await Task.Delay(200);
        sprite2D.Visible = true;
        collisionShape2D.Disabled = false;
        lightOccluder2D.Visible = true;
        await Task.Delay(800);
        animationPlayer.Stop();
    }
}
