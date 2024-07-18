using Godot;
using System;
using System.Threading.Tasks;
using Godot.Collections;
using System.Threading;

public partial class ui : CanvasLayer
{
    [Export] private PlayerBear characterBody2D;
    private game_manager gm;
    private SceneTransition st;
    private Label honeyCountLabel;

    private CanvasLayer lvlMap;
    private CanvasLayer toolTip;

    private Button restartButton;
    private Button exitButton;
    private Button newGameButton;
    private Button optionsButton;
    private Button continueButton;

    private Array<Node> lvlButtons;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ProcessMode = Node.ProcessModeEnum.Always;

        toolTip = GetNode<CanvasLayer>("ToolTip");
        lvlMap = GetNode<CanvasLayer>("LevelsMap");
        gm = GetNode<game_manager>("/root/GM");
        gm.OnResetGame += Gm_OnResetGame;
        gm.OnSceneTransition += Gm_OnSceneTransition;
        gm.OnPlayerDeath += Gm_OnPlayerDeath;
        st = GetNode<SceneTransition>("/root/SceneTransition");
        honeyCountLabel = GetNode<Label>("HoneyCounterDisplay/Label");
        restartButton = GetNode<Button>("Button");
        gm.sceneTransition = false;

        exitButton = GetNode<Button>("Menu/MenuButtons/ExitButton");
        newGameButton = GetNode<Button>("Menu/MenuButtons/NewGameButton");
        optionsButton = GetNode<Button>("Menu/MenuButtons/OptionsButton");
        continueButton = GetNode<Button>("Menu/MenuButtons/ContinueButton");

        lvlButtons = lvlMap.GetNode<GridContainer>("GridContainer").GetChildren();
        SetLevelsMap();
        
    }

    private void Gm_OnPlayerDeath(object sender, EventArgs e)
    {
        MinusLife();
    }

    private async void Gm_OnSceneTransition(object sender, EventArgs e)
    {
        gm.currentLevel++;
        st.SceneTransitionMethod("res://Level" + gm.currentLevel.ToString() + ".tscn");
        await Task.Delay(470);
        gm.honeyCount = 0;
    }

    private void Gm_OnResetGame(object sender, EventArgs e)
    {
        var life = GetNode<BoxContainer>("BoxContainer").GetChildren();
        foreach (TextureRect child in life)
        {
            child.Visible = true;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (characterBody2D != null)
        {
            honeyCountLabel.Text = "x " + characterBody2D.honeyCount.ToString() + "/" + gm.honeyCount.ToString();
            if (characterBody2D.honeyCount == gm.honeyCount && !gm.sceneTransition)
            {
                if (gm.levelsDone.Length >= gm.currentLevel)
                {
                    gm.levelsDone[gm.currentLevel - 1] = true;
                }
                gm.SceneTransition();

            }
        }
        if (toolTip != null)
        {
            if (Input.IsAnythingPressed())
            {
                if (toolTip.Visible)
                {
                    toolTip.Visible = false;
                    if (!Input.IsActionJustPressed("Pause"))
                    {
                        GetTree().Paused = false;
                    }
                }
            }
            if (toolTip.Visible)
            {
                GetTree().Paused = true;
            }
        }
        if (Input.IsActionJustPressed("TimeRevind") && GetNode<CanvasLayer>("DieMessage").Visible)
        {
            GetNode<CanvasLayer>("DieMessage").Visible = false;
        }
        if (gm.gameState == GameState.Paused)
        {
            GetNode<CanvasLayer>("Menu").Visible = true;
            GetNode<BoxContainer>("HoneyCounterDisplay").Visible = false;
            GetNode<BoxContainer>("BoxContainer").Visible = false;
            continueButton.Visible = true;
            lvlMap.Visible = false;
            newGameButton.Text = "Level Map";
        }
        else if (gm.gameState == GameState.OnLevel)
        {
            GetNode<CanvasLayer>("Menu").Visible = false;
            GetNode<BoxContainer>("HoneyCounterDisplay").Visible = true;
            GetNode<BoxContainer>("BoxContainer").Visible = true;
            lvlMap.Visible = false;

        }
        else if (gm.gameState == GameState.MainMenu)
        {
            GetNode<CanvasLayer>("Menu").Visible = true;
            GetNode<BoxContainer>("HoneyCounterDisplay").Visible = false;
            GetNode<BoxContainer>("BoxContainer").Visible = false;
            continueButton.Visible = false;
            lvlMap.Visible = false;
            newGameButton.Text = "New Game";
        }
        else if (gm.gameState == GameState.LevelsMap)
        {
            GetNode<CanvasLayer>("Menu").Visible = false;
            GetNode<BoxContainer>("HoneyCounterDisplay").Visible = false;
            GetNode<BoxContainer>("BoxContainer").Visible = false;
            lvlMap.Visible = true;
        }
    }

    public void MinusLife()
    {
        var life = GetNode<BoxContainer>("BoxContainer").GetChildren();
        foreach (TextureRect child in life)
        {
            if (child.Visible == true)
            {
                child.Visible = false;
                break;
            }
        }
        if (gm.dieFirstTime)
        {
            GetNode<CanvasLayer>("DieMessage").Visible = true;
            gm.dieFirstTime = false;
        }
    }

    public void _on_tree_exited()
    {
        gm.OnResetGame -= Gm_OnResetGame;
        gm.OnSceneTransition -= Gm_OnSceneTransition;
        gm.OnPlayerDeath -= Gm_OnPlayerDeath;
    }

    public void _on_button_up()
    {
        gm.dieFirstTime = false;
        gm.lives = 0;
        gm.ResetGame();
        restartButton.FocusMode = Control.FocusModeEnum.None;
    }
    public void _on_exit_button_pressed()
    {
        gm.SaveGame();
        GetTree().Quit();
    }

    public void _on_new_game_button_pressed()
    {
        gm.gameState = GameState.LevelsMap;

        /*gm.currentLevel = 0;
        gm.SceneTransition();
        gm.gameState = GameState.OnLevel;
        gm.honeyCount = 0;*/
    }
    public void _on_continue_button_pressed()
    {
        gm.gameState = GameState.OnLevel;
        GetTree().Paused = false;
    }
    public void _on_reset_progress_pressed()
    {
        gm.ResetProgress();
        gm.LoadGame();
        SetLevelsMap();
    }

    private void SetLevelsMap()
    {
        for (int i = 0; i < lvlButtons.Count; i++)
        {
            if (gm.levelsDone[i])
            {
                var button = lvlButtons[i] as Button;
                button.AddThemeColorOverride("font_color", new Color(1, 1, 1, 0));
                button.Text = (i + 1).ToString();
                button.Icon = ResourceLoader.Load("res://Cup.png") as Texture2D;
            }
            else
            {
                var button = lvlButtons[i] as Button;
                button.AddThemeColorOverride("font_color", new Color(1, 1, 1, 1));
                button.Text = (i + 1).ToString();
                button.Icon = null;
            }
        }
    }
    public void _on_1_pressed()
    {
        ChangeLevel(1);
    }
    public void _on_2_pressed()
    {
        ChangeLevel(2);
    }
    public void _on_3_pressed()
    {
        ChangeLevel(3);
    }
    public void _on_4_pressed()
    {
        ChangeLevel(4);
    }
    public void _on_5_pressed()
    {
        ChangeLevel(5);
    }
    public void _on_6_pressed()
    {
        ChangeLevel(6);
    }
    public void _on_7_pressed()
    {
        ChangeLevel(7);
    }
    public void _on_8_pressed()
    {
        ChangeLevel(8);
    }
    public void _on_9_pressed()
    {
        ChangeLevel(9);
    }
    public void _on_10_pressed()
    {
        ChangeLevel(10);
    }
    public void _on_11_pressed()
    {
        ChangeLevel(11);
    }
    public void _on_12_pressed()
    {
        ChangeLevel(12);
    }
    public void _on_13_pressed()
    {
        ChangeLevel(13);
    }
    public void _on_14_pressed()
    {
        ChangeLevel(14);
    }
    public void _on_15_pressed()
    {
        ChangeLevel(15);
    }
    public void _on_16_pressed()
    {
        ChangeLevel(16);
    }
    public void _on_17_pressed()
    {
        ChangeLevel(17);
    }

    private void ChangeLevel(int level)
    {
        gm.currentLevel = level - 1;
        gm.SceneTransition();
        gm.gameState = GameState.OnLevel;
        gm.honeyCount = 0;
        GetTree().Paused = false;
    }
}
