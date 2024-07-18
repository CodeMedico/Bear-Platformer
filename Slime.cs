using Godot;
using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public partial class Slime : Area2D
{
    private PackedScene slime;
    protected game_manager gm;

    private ActionsData actionsData = new ActionsData();

    public AnimationPlayer deathAnimation;
    protected ShapeCast2D slimeTop;
    private RayCast2D rayCast2DLedge;
    private RayCast2D rayCast2DWall;
    private AnimatedSprite2D animatedSprite;
    public Vector2 moveDirection;
    private Vector2 inititalDirection;
    public Vector2 initialPosition;
    private Vector2 inititalScale;
    private Vector2 spriteScale;
    private Vector2 spritePosition;
    private bool isTimeRevindEnd;
    private bool isSlimeAlive = true;
    private bool firstTimeCalledAlive = false;
    public override void _Ready()
    {
        rayCast2DLedge = GetNode<RayCast2D>("RayCast2D");
        rayCast2DWall = GetNode<RayCast2D>("RayCast2DWall");
        slimeTop = GetNode<ShapeCast2D>("ShapeCast2D");
        deathAnimation = GetNode<AnimationPlayer>("AnimationPlayer");
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        if (Scale.X > 0)
        {
            moveDirection = new Vector2(-3f, 0);
        }
        else
        {
            moveDirection = new Vector2(3f, 0);
        }
        inititalScale = Scale;
        initialPosition = Position;
        inititalDirection = moveDirection;
        spriteScale = animatedSprite.Scale;
        spritePosition = animatedSprite.Position;
        gm = GetNode<game_manager>("/root/GM");
        gm.OnResetGame += OnResetGame;
        gm.OnSceneTransition += Gm_OnSceneTransition;
    }

    private void Gm_OnSceneTransition(object sender, EventArgs e)
    {
        moveDirection = Vector2.Zero;
    }

    private async void OnResetGame(object sender, EventArgs e)
    {
        await Task.Delay(200);
        Position = initialPosition;
        Scale = inititalScale;
        moveDirection = inititalDirection;
        Rotation = 0;
        Visible = true;
        isSlimeAlive = true;
        actionsData = new ActionsData();
        animatedSprite.Position = spritePosition;
        animatedSprite.Scale = spriteScale;
    }
    public void _on_tree_exited()
    {
        gm.OnResetGame -= OnResetGame;
        gm.OnSceneTransition -= Gm_OnSceneTransition;
    }
    public override void _Process(double delta)
    {
        HandleTimeRevind();
        HandleMovement();
    }
    public virtual void _on_body_entered(CharacterBody2D body)
    {
        if (!Input.IsActionPressed("TimeRevind") && slimeTop.IsColliding() && body.Velocity.Y > 0)
        {
            body.Velocity = new Vector2(0, -2000f);
            moveDirection = Vector2.Zero;
            deathAnimation.Play("Death");
        }
        else
        {
            gm.ResetGame();
        }
    }

    private void HandleTimeRevind()
    {
        if (Input.IsActionJustReleased("TimeRevind") || actionsData.Positions.Count == 0)
        {
            GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
            isTimeRevindEnd = true;
        }
        if (actionsData.Positions.Count > 60)
        {
            isTimeRevindEnd = false;
        }
        if (Input.IsActionPressed("TimeRevind") && !isTimeRevindEnd)
        {
            GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
            Transform = actionsData.TimeRevind();
            actionsData.DeleteLastPosition();
            if (actionsData.IsDirectionLeft())
            {
                moveDirection = new Vector2(-3f, 0);
            }
            else
            {
                moveDirection = new Vector2(3f, 0);
            }
            if (actionsData.IsSlimeAlive())
            {
                Visible = true;
                isSlimeAlive = true;
                if (firstTimeCalledAlive)
                {
                    deathAnimation.Play("ReverseDeath");
                    firstTimeCalledAlive = false;
                }
            }
            actionsData.DeleteLastSlimeState();
            actionsData.DeleteLastDirection();

        }
        else if (!Input.IsActionPressed("TimeRevind"))
        {

            actionsData.AddNewPosition(Transform);
            actionsData.AddSlimeAlive(isSlimeAlive);
            if (!isSlimeAlive)
            {
                firstTimeCalledAlive = true;
            }
            if (moveDirection.X < 0)
            {
                actionsData.AddDirection(true);
            }
            else
            {
                actionsData.AddDirection(false);
            }
        }
    }
    private void HandleMovement()
    {
        if (rayCast2DLedge.IsColliding() && !rayCast2DWall.IsColliding())
        {
            Position += moveDirection;
        }
        else
        {
            moveDirection = -moveDirection;
            Scale = new Vector2(-Scale.X, Scale.Y);
        }
    }
    private void DeadSlime()
    {
        Position = new Vector2(2000f, 2000f);
        isSlimeAlive = false;
        this.Visible = false;
    }
}
