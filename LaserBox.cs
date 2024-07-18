using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class LaserBox : Line2D
{
    public game_manager gm;
    public LaserData.LaserState laserState;
    private LaserData laserData = new LaserData();

    public RayCast2D rayCast2D;
    public Timer timerArm;
    public Timer timerFire;
    public Timer timerLoad;
    public AnimationPlayer laserGrowth;
    public bool laserCanDamage = false;
    private bool firstTime = true;

    public override void _Ready()
    {
        gm = GetNode<game_manager>("/root/GM");
        gm.OnResetGame += Gm_OnResetGame;
        rayCast2D = GetNode<RayCast2D>("RayCast2D");
        timerArm = GetNode<Timer>("TimerArm");
        timerFire = GetNode<Timer>("TimerFire");
        timerLoad = GetNode<Timer>("TimerLoad");
        laserGrowth = GetNode<AnimationPlayer>("AnimationPlayer");
        timerLoad.Start();
        Width = 0;
        laserState = LaserData.LaserState.Load;
    }

    private void Gm_OnResetGame(object sender, EventArgs e)
    {
        timerArm.Stop();
        timerFire.Stop();
        timerLoad.Stop();
        laserGrowth.Stop();
        timerLoad.Start();
        Width = 0;
        laserCanDamage = false;
        laserData = new LaserData();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        SetPointPosition(1, ToLocal(rayCast2D.GetCollisionPoint()));
        if (!rayCast2D.IsColliding())
        {
            SetPointPosition(1, rayCast2D.TargetPosition);
        }
        if (rayCast2D.GetCollider() is PlayerBear && laserCanDamage)
        {
            gm.ResetGame();
        }
        if (!Input.IsActionPressed("TimeRevind"))
        {
            laserData.AddLaserState(laserState);
        }
        else if (Input.IsActionPressed("TimeRevind"))
        {
            timerArm.Stop();
            timerFire.Stop();
            timerLoad.Stop();
            Width = 0;
            laserCanDamage = false;
            laserState = laserData.TimeRevind();
            laserData.DeleteLastState();
            if (laserState == LaserData.LaserState.Arm)
            {
                laserGrowth.Stop();
                Width = 3;
            }
            else if (laserState == LaserData.LaserState.Fire)
            {
                Width = 15;
                if (firstTime)
                {
                    laserGrowth.Play("LaserGrowth");
                    firstTime = false;
                }
                Material.Set("shader_parameter/_fatLaser", true);
            }
            else
            {
                Width = 0;
            }
        }
        if (Input.IsActionJustReleased("TimeRevind"))
        {
            if (laserData.laserStates[0] == LaserData.LaserState.Load)
            {
                timerLoad.Start();
            }
            else if (laserData.laserStates[0] == LaserData.LaserState.Arm)
            {
                timerArm.Start();
            }
            else
            {
                timerFire.Start();
            }
            firstTime = true;
        }
    }

    public void _on_timer_arm_timeout()
    {
        Material.Set("shader_parameter/_fatLaser", true);
        Width = 15;
        laserGrowth.Play("LaserGrowth");
        timerFire.Start();
        timerArm.Stop();
        laserCanDamage = true;
        laserState = LaserData.LaserState.Fire;
    }

    public void _on_timer_fire_timeout()
    {
        Material.Set("shader_parameter/_fatLaser", false);
        Width = 0;
        timerFire.Stop();
        timerLoad.Start();
        laserCanDamage = false;
        laserState = LaserData.LaserState.Load;

    }

    public void _on_timer_load_timeout()
    {
        Width = 3;
        timerArm.Start();
        timerLoad.Stop();
        laserState = LaserData.LaserState.Arm;
    }

    public void _on_tree_exited()
    {
        gm.OnResetGame -= Gm_OnResetGame;
    }

}

public class LaserData
{
    public enum LaserState
    {
        Load,
        Arm,
        Fire,
    }
    public List<LaserState> laserStates = new List<LaserState>();

    public void AddLaserState(LaserState laserState)
    {
        if (laserStates.Count < 240)
        {
            laserStates.Add(laserState);
        }
        else
        {
            laserStates.RemoveAt(0);
            laserStates.Add(laserState);
        }
    }
    public LaserState TimeRevind()
    {
        return laserStates[laserStates.Count - 1];
    }
    public void DeleteLastState()
    {
        if (laserStates.Count > 1)
            laserStates.RemoveAt(laserStates.Count - 1);
    }
}
