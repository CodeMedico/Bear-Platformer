using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static Godot.TextServer;

public partial class PlayerBear : Godot.CharacterBody2D
{
    [Export] public AnimatedSprite2D sprite2D;
    private AnimationPlayer animtions;
    private AnimationTree animationTree;
    [Export] public Sprite2D clock;

    public Camera2D Camera;
    public ActionsData actionsData;
    private game_manager gm;
    private Timer dashTimer;
    private Timer dashCooldown;
    private Timer coyoteTimer;
    private ShapeCast2D shapeCast2D;
    private RayCast2D leftTop;
    private RayCast2D rightTop;
    private RayCast2D midLeftTop;
    private RayCast2D midRightTop;
    private RayCast2D rightLegs;
    private RayCast2D leftLegs;
    private RayCast2D rightLegsMid;
    private RayCast2D leftLegsMid;
    private RayCast2D bottomRay;
    private RayCast2D crushRayRight;
    private RayCast2D crushRayLeft;
    private GpuParticles2D deathAnimationParticles;
    private GpuParticles2D dust;

    public float maxSpeed = 800f;
    public float speedGrowRate = 2000f;
    public const float JumpVelocity = -200.0f;
    public Transform2D inititalPosition;
    public int honeyCount = 0;
    public bool deathAnimation = false;

    private float jumpTime = 0f;
    private float maxJumpTime = .15f;
    private Vector2 dashDirection = Vector2.Right;
    private bool onLadder = false;
    private bool grabLadder = false;
    private bool walJumpAllowed = true;
    private bool crushed = false;

    private float shadowFrequency;
    private float decelerateRate = 300f;


    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready()
    {
        animtions = GetNode<AnimationPlayer>("Bear/AnimationPlayer");
        animationTree = GetNode<AnimationTree>("Bear/AnimationPlayer/AnimationTree");

        inititalPosition = Transform;
        dashTimer = GetNode<Timer>("DashTimer");
        dashCooldown = GetNode<Timer>("DashCooldown");
        coyoteTimer = GetNode<Timer>("Coyote");

        shapeCast2D = GetNode<ShapeCast2D>("ShapeCast2D");
        leftTop = GetNode<RayCast2D>("LeftTop");
        rightTop = GetNode<RayCast2D>("RightTop");
        midLeftTop = GetNode<RayCast2D>("MidLeftTop");
        midRightTop = GetNode<RayCast2D>("MidRightTop");
        rightLegs = GetNode<RayCast2D>("RightLegs");
        rightLegsMid = GetNode<RayCast2D>("RightLegsMid");
        leftLegs = GetNode<RayCast2D>("LeftLegs");
        leftLegsMid = GetNode<RayCast2D>("LeftLegsMid");
        bottomRay = GetNode<RayCast2D>("Bottom");
        crushRayLeft = GetNode<RayCast2D>("CrushRayLeft");
        crushRayRight = GetNode<RayCast2D>("CrushRayRight");

        Camera = GetNode<Camera2D>("%MainCamera");
        deathAnimationParticles = GetNode<GpuParticles2D>("DeathAnimation");
        dust = GetNode<GpuParticles2D>("Dust");

        actionsData = new ActionsData();
        actionsData.AddNewPosition(Transform);
        gm = GetNode<game_manager>("/root/GM");

        gm.OnResetGame += Gm_OnResetGame;
        gm.OnSceneTransition += Gm_OnSceneTransition;
        gm.OnPlayerDeath += Gm_OnPlayerDeath;

        deathAnimation = false;
        crushed = false;
    }

    private void Gm_OnPlayerDeath(object sender, EventArgs e)
    {

        deathAnimationParticles.Emitting = true;
        sprite2D.Visible = false;
        deathAnimation = true;
        gm.deathAnimation = true;
    }

    private void Gm_OnSceneTransition(object sender, EventArgs e)
    {
    }

    private async void Gm_OnResetGame(object sender, EventArgs e)
    {
        await Task.Delay(200);
        crushed = false;
        Camera.PositionSmoothingEnabled = false;
        Transform = inititalPosition;
        Velocity = Vector2.Zero;
        honeyCount = 0;
        sprite2D.Visible = true;
        deathAnimationParticles.Emitting = false;
        await Task.Delay(100);
        actionsData = new ActionsData();
        deathAnimation = false;
        gm.deathAnimation = false;
        Camera.PositionSmoothingEnabled = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("ui_accept"))
        {
            if (Input.IsActionPressed("ui_down"))
            {
                Position += Vector2.Down * 6;
            }
        }
        HandleGravity();
        HandleCrush();
        Vector2 velocity = Velocity;
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        HandleTimeRevind(direction, ref velocity, ref delta);
        HandleJump(ref velocity, delta);

        HandleHorizontalMovement(ref direction, ref velocity, delta);
        HandleWallInteraction(direction, ref velocity);
        HandleDash(dashDirection, ref velocity);
        HandleLadder(direction, ref velocity);
        HandleSealing();
        HandleStairs();
        Velocity = velocity;
        MoveAndSlide();
        MonitorSurface();
        DashAnimation(delta);
        //AnimationsMachine();
    }

    public void _on_dash_timer_timeout()
    {
        dashTimer.Stop();
        gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
        Velocity = new Vector2(0, 0.35f * Velocity.Y);
        jumpTime = maxJumpTime;
        sprite2D.Play("dash");
    }
    public void _on_dash_cooldown_timeout()
    {
        dashCooldown.Stop();
    }
    public void _on_coyote_timeout()
    {
        coyoteTimer.Stop();
    }

    public void _on_area_2d_body_entered(Node2D body)
    {
        gm.ResetGame();
    }

    public void _on_pit_body_entered(Node2D body)
    {
        gm.ResetGame();
    }

    public void _on_ladder_body_entered(Node2D body)
    {
        onLadder = true;
    }

    public void _on_ladder_body_exited(Node2D body)
    {
        onLadder = false;
        grabLadder = false;
        gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    }
    public void _on_tree_exited()
    {
        gm.OnResetGame -= Gm_OnResetGame;
        gm.OnSceneTransition -= Gm_OnSceneTransition;
        gm.OnPlayerDeath -= Gm_OnPlayerDeath;
    }
    private bool ControlsEnabled()
    {
        return !Input.IsActionPressed("TimeRevind") && dashTimer.IsStopped() && !deathAnimation && !gm.sceneTransition;
    }

    private void AnimationsMachine()
    {
        if (Velocity.Y > 0)
        {
            animationTree.Set("parameters/conditions/isFalling", true);
            animationTree.Set("parameters/conditions/isIdle", false);
            animationTree.Set("parameters/conditions/isJumping", false);
            animationTree.Set("parameters/conditions/isMoving", false);
        }
        else if (Velocity.Y < 0)
        {
            animationTree.Set("parameters/conditions/isJumping", true);
            animationTree.Set("parameters/conditions/isIdle", false);
            animationTree.Set("parameters/conditions/isMoving", false);
        }
        if(Velocity.X != 0)
        {
            animationTree.Set("parameters/conditions/isMoving", true);
        }
        if(IsOnFloor() && Velocity == Vector2.Zero)
        {
            animationTree.Set("parameters/conditions/isIdle", true);
            animationTree.Set("parameters/conditions/isMoving", false);
            animationTree.Set("parameters/conditions/isJumping", false);
            animationTree.Set("parameters/conditions/isFalling", false);
        }

    }

    private void HandleTimeRevind(Vector2 direction, ref Vector2 velocity, ref double delta)
    {
        if (Input.IsActionJustReleased("TimeRevind") || actionsData.Positions.Count == 0)
        {
            clock.Visible = false;
            gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
            velocity = Vector2.Zero;
            deathAnimation = false;
            gm.deathAnimation = false;
            crushed = false;
        }
        if (Input.IsActionPressed("TimeRevind") && actionsData.Positions.Count == 0)
        {
            GetTree().Paused = true;
        }
        if (Input.IsActionPressed("TimeRevind") && actionsData.Positions.Count > 0)
        {
            DashAnimation(delta);
            clock.Visible = true;
            sprite2D.Visible = true;
            Velocity = Vector2.Zero;
            Transform = actionsData.TimeRevind();
            actionsData.DeleteLastPosition();
            if (gravity != 0)
            {
                gravity = 0;
            }
            if (actionsData.ActionsRewind() == Actions.Walk)
            {
                sprite2D.Play("run");
            }
            else if (actionsData.ActionsRewind() == Actions.Stay)
            {
                sprite2D.Play("idle");
            }
            else if (actionsData.ActionsRewind() == Actions.Jump)
            {
                sprite2D.Play("jump");
            }
            else if (actionsData.ActionsRewind() == Actions.Dash)
            {
                sprite2D.Play("dash");
            }
            actionsData.DeleteLastAction();
            if (actionsData.IsDirectionLeft())
            {
                sprite2D.FlipH = true;
            }
            else if (!actionsData.IsDirectionLeft())
            {
                sprite2D.FlipH = false;
            }
            actionsData.DeleteLastDirection();

            if (actionsData.IsInteraction())
            {
                Input.ActionPress("Interact");
            }
            actionsData.DeleteLastInteraction();
        }
        else if (!Input.IsActionPressed("TimeRevind"))
        {
            clock.Visible = false;
            actionsData.AddNewPosition(Transform);
            actionsData.AddInteraction(Input.IsActionJustPressed("Interact"));
            if (!dashTimer.IsStopped())
            {
                actionsData.AddNewAction(Actions.Dash);
            }
            else if (direction == Vector2.Zero && IsOnFloor())
            {
                actionsData.AddNewAction(Actions.Stay);
            }
            else if (!IsOnFloor() && dashTimer.IsStopped())
            {
                actionsData.AddNewAction(Actions.Jump);
            }
            else if (direction != Vector2.Zero && IsOnFloor())
            {
                actionsData.AddNewAction(Actions.Walk);
            }
            if (!sprite2D.FlipH)
            {
                actionsData.AddDirection(false);
            }
            else if (sprite2D.FlipH)
            {
                actionsData.AddDirection(true);
            }
        }
    }

    private void HandleLadder(Vector2 direction, ref Vector2 velocity)
    {
        if (onLadder)
        {
            if (direction == Vector2.Up || direction == Vector2.Down)
            {
                grabLadder = true;
            }
            if (direction != Vector2.Zero && grabLadder)
            {
                Position += direction * 10;
            }
            else if (Input.IsActionPressed("ui_accept") && grabLadder)
            {
                gravity = gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
                velocity.Y = JumpVelocity;
                grabLadder = false;
            }
            if (grabLadder)
            {
                velocity = Vector2.Zero;
                gravity = 0;
            }
        }
    }

    private void HandleDash(Vector2 direction, ref Vector2 velocity)
    {
        if (Input.IsActionJustPressed("dash") && dashCooldown.IsStopped() && !Input.IsActionPressed("TimeRevind")
            && !IsOnWall() && !deathAnimation)
        {
            velocity.Y = 0f;
            velocity = dashDirection * 2000f;
            gravity = 0f;
            dashTimer.Start();
            dashCooldown.Start();
            sprite2D.Play("dash");
        }
    }

    private void HandleWallInteraction(Vector2 direction, ref Vector2 velocity)
    {
        if (shapeCast2D.IsColliding() && GetWallNormal() == Vector2.Right && direction == Vector2.Left ||
            shapeCast2D.IsColliding() && GetWallNormal() == Vector2.Left && direction == Vector2.Right)
        {
            gravity = 1500f;
            velocity.Y = Math.Clamp(velocity.Y, -10000f, 500f);
        }
        if (shapeCast2D.IsColliding())
        {
            gravity = 1500f;
            velocity.Y = Math.Clamp(velocity.Y, -10000f, 1000f);
            if (Input.IsActionJustPressed("ui_accept") && walJumpAllowed)
            {
                velocity = 1000f * GetWallNormal();
                walJumpAllowed = false;
            }
        }
        else
        {
            gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
        }
        if (Input.IsActionPressed("ui_accept"))
        {
            walJumpAllowed = false;
        }
    }

    private void HandleHorizontalMovement(ref Vector2 direction, ref Vector2 velocity, double delta)
    {
        if (direction != Vector2.Zero)
            dashDirection = direction;
        if (direction.X != 0 && ControlsEnabled())
        {
            if (velocity.X < maxSpeed && velocity.X > -maxSpeed ||
                velocity.X >= maxSpeed && direction.X < 0 ||
                velocity.X <= -maxSpeed && direction.X > 0)
            {
                velocity.X += 3 * direction.Normalized().X * speedGrowRate * (float)delta;
            }
            else if (velocity.X >= maxSpeed && direction.X > 0 ||
                velocity.X <= -maxSpeed && direction.X < 0)
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, 25);
            }
            //velocity.X = Math.Clamp(velocity.X, -maxSpeed, maxSpeed);
            if (IsOnFloor() && direction.X != 0 && !Input.IsActionPressed("TimeRevind"))
            {
                sprite2D.Play("run");
            }
            if (direction.X > 0)
            {
                sprite2D.FlipH = false;

            }
            else if (direction.X < 0)
            {
                sprite2D.FlipH = true;
            }
        }
        else if (dashTimer.IsStopped())
        {
            if (IsOnFloor())
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, speedGrowRate * decelerateRate);
            }
            else
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, 25);
            }
            if (IsOnFloor() && ControlsEnabled())
            {
                sprite2D.Play("idle");
            }
        }
    }

    private void HandleJump(ref Vector2 velocity, double delta)
    {
        if (!IsOnFloor() && dashTimer.IsStopped() && ControlsEnabled() && !grabLadder)
        {
            sprite2D.Play("jump");
            velocity.Y += gravity * (float)delta;
            velocity.Y = Math.Clamp(velocity.Y, -10000f, 2000f);
        }

        if ((Input.IsActionPressed("ui_accept") && jumpTime < maxJumpTime && ControlsEnabled() && Velocity.Y <= 0)
            || (Input.IsActionPressed("ui_accept") && !coyoteTimer.IsStopped()))
        {
            dust.Emitting = true;
            if (velocity.Y > -800 && !Input.IsActionPressed("ui_down"))
            {
                velocity.Y += JumpVelocity;
                jumpTime += (float)delta;
            }
        }
        if (Input.IsActionJustReleased("ui_accept"))
        {
            jumpTime = maxJumpTime;
            walJumpAllowed = true;
        }
        if (IsOnFloor() || (shapeCast2D.IsColliding() && walJumpAllowed))
        {
            jumpTime = 0f;
            coyoteTimer.Start();
        }
        else
        {
            gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
        }
    }

    private void HandleGravity()
    {
        if (gm.sceneTransition || deathAnimation)
        {
            gravity = 0;
            Velocity = Vector2.Zero;
            this.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
        }
        else if (Velocity.Y > 0 && !shapeCast2D.IsColliding())
        {
            gravity *= 1.8f;
        }
        else if (!shapeCast2D.IsColliding())
        {
            gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
        }
    }
    private void HandleSealing()
    {
        float distance = 10f;
        if (leftTop.IsColliding() && !midLeftTop.IsColliding() && !midRightTop.IsColliding() && !rightTop.IsColliding())
        {
            Position += Vector2.Right * distance;
        }
        if (!leftTop.IsColliding() && !midLeftTop.IsColliding() && !midRightTop.IsColliding() && rightTop.IsColliding())
        {
            Position += Vector2.Left * distance;
        }
    }
    private void HandleStairs()
    {
        float distance = 5f;
        if ((leftLegs.IsColliding() && !leftLegsMid.IsColliding()) || (rightLegs.IsColliding() && !rightLegsMid.IsColliding()))
        {
            GD.Print("pushUp");
            Position += Vector2.Up * distance;
        }
    }

    private void MonitorSurface()
    {
        if (GetLastSlideCollision() != null)
        {
            var collider = this.GetLastSlideCollision().GetCollider();
            if (collider is PhysicsBody2D)
            {
                var body = collider as PhysicsBody2D;
                if (body.GetCollisionLayerValue(8))
                {
                    decelerateRate = .01f;
                }
                else
                {
                    decelerateRate = 300f;
                }
            }
            else if (collider is TileMap)
            {
                var tilemap = collider as TileMap;
                if (tilemap.TileSet.GetPhysicsLayerCollisionLayer(0) == 128)
                {
                    decelerateRate = .01f;
                }
                else
                {
                    decelerateRate = 300f;
                }
            }
        }
    }

    private async void DashAnimation(double delta)
    {

        if (!dashTimer.IsStopped() && shadowFrequency > 0.01f)
        {
            Sprite2D sprite = new Sprite2D();
            sprite.Set("texture", ResourceLoader.Load("res://rushBear.png"));
            GetTree().Root.AddChild(sprite);
            sprite.GlobalPosition = GlobalPosition;
            sprite.FlipH = sprite2D.FlipH;
            sprite.Scale = sprite2D.Scale * Scale;
            sprite.Modulate = new Color(0, 0, 0, 0.2f);
            shadowFrequency = 0;
            await Task.Delay(100);
            sprite.QueueFree();

        }
        shadowFrequency += (float)delta;
    }
    private void HandleCrush()
    {
        if (bottomRay.IsColliding() && (midLeftTop.IsColliding() || midRightTop.IsColliding()))
        {
            if (bottomRay.GetCollisionPoint().Y - midLeftTop.GetCollisionPoint().Y < 70
                || bottomRay.GetCollisionPoint().Y - midRightTop.GetCollisionPoint().Y < 70)
            {
                if (!crushed)
                {
                    gm.ResetGame();
                    crushed = true;
                }
            }
        }
        if (crushRayLeft.IsColliding() && crushRayRight.IsColliding())
        {
            if (crushRayLeft.GetCollisionPoint().X - crushRayRight.GetCollisionPoint().X < 68)
            {
                if (!crushed)
                {
                    gm.ResetGame();
                    crushed = true;
                }
            }
        }
    }
    public void HoneyColleted()
    {
        honeyCount++;
    }
}
