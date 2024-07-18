using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Godot.NativeInterop;
using Godot.Collections;

public partial class game_manager : Node
{
    public event EventHandler OnResetGame;
    public event EventHandler OnSceneTransition;
    public event EventHandler OnPlayerDeath;
    public bool sceneTransition = false;
    public bool deathAnimation = false;
    public int honeyCount = 0;
    public int currentLevel = 1;
    public int lives;
    public bool minusLife = false;
    public Array<Node> animations;
    public bool dieFirstTime = true;
    public GameState gameState = GameState.MainMenu;

    public bool[] levelsDone = new bool[17];

    public override void _Ready()
    {
        for (int i = 0; i < levelsDone.Length; i++)
        {
            levelsDone[i] = false;
        }
        LoadGame();
        foreach(var value in levelsDone)
        {
            GD.Print(value);
        }
        
        ProcessMode = Node.ProcessModeEnum.Always;
        honeyCount = 0;
        animations = GetTree().GetNodesInGroup("Animations");
        lives = GetTree().GetFirstNodeInGroup("UI").GetNode<BoxContainer>("BoxContainer").GetChildren().Count;
    }

    public override void _Process(double delta)
    {
        animations = GetTree().GetNodesInGroup("Animations");
        if (Input.IsActionJustPressed("Pause") && gameState == GameState.OnLevel)
        {
            GetTree().Paused = true;
            gameState = GameState.Paused;
        }
        else if (Input.IsActionJustPressed("Pause") && (gameState == GameState.Paused ||  gameState == GameState.LevelsMap))
        {
            GetTree().Paused = false;
            gameState = GameState.OnLevel;
        }
        if (Input.IsActionJustReleased("TimeRevind") || (Input.IsActionJustPressed("TimeRevind") && minusLife))
        {
            GetTree().Paused = false;
            minusLife = false;
        }
        if (Input.IsActionPressed(("TimeRevind")))
        {
            foreach (var anim in animations)
            {
                var animation = anim as AnimationPlayer;
                animation.PlayBackwards();
            }
        }
        else
        {
            foreach (var anim in animations)
            {
                var animation = anim as AnimationPlayer;
                animation.Play();
            }
        }

    }
    public async void ResetGame()
    {
        if (!sceneTransition && !deathAnimation)
        {
            if (lives > 0)
            {
                OnPlayerDeath?.Invoke(this, EventArgs.Empty);
                await Task.Delay(500);
                GetTree().Paused = true;
                minusLife = true;
                lives--;
            }
            else
            {
                OnPlayerDeath?.Invoke(this, EventArgs.Empty);
                await Task.Delay(500);
                //GetTree().ReloadCurrentScene();
                OnResetGame?.Invoke(this, EventArgs.Empty);
                await Task.Delay(200);
                foreach (var anim in animations)
                {
                    var animation = anim as AnimationPlayer;
                    animation.Play("RESET");
                    animation.Play("fly");
                }
                lives = GetTree().GetFirstNodeInGroup("UI").GetNode<BoxContainer>("BoxContainer").GetChildren().Count;
            }
        }

        
    }
    public void SceneTransition()
    {
        sceneTransition = true;
        lives = GetTree().GetFirstNodeInGroup("UI").GetNode<BoxContainer>("BoxContainer").GetChildren().Count;
        OnSceneTransition?.Invoke(this, EventArgs.Empty);
        SaveGame();
    }
    public void AddHoney()
    {
        honeyCount++;
    }

    public void SaveGame()
    {

        using var saveGame = FileAccess.Open("res://savegame.save", FileAccess.ModeFlags.Write);
        var levelsDoneData = new Godot.Collections.Array();
        foreach(var value in levelsDone)
        {
            levelsDoneData.Add(value);
        }
        var nodeData = levelsDoneData;
        var jsonString = Json.Stringify(nodeData);
        saveGame.StoreLine(jsonString);
    }
    public void LoadGame()
    {
        using var saveGame = FileAccess.Open("res://savegame.save", FileAccess.ModeFlags.Read);
        var jsonString = saveGame.GetLine();
        var json = new Json();
        var parseResult = json.Parse(jsonString);
        var nodeData = new Godot.Collections.Array((Godot.Collections.Array)json.Data);
        for (int i = 0; i < nodeData.Count; i++)
        {
            levelsDone[i] = (bool)nodeData[i];
        }
    }
    public void ResetProgress()
    {
        using var saveGame = FileAccess.Open("res://savegame.save", FileAccess.ModeFlags.Write);
        var levelsDoneData = new Godot.Collections.Array();
        foreach (var value in levelsDone)
        {
            levelsDoneData.Add(false);
        }
        var nodeData = levelsDoneData;
        var jsonString = Json.Stringify(nodeData);
        saveGame.StoreLine(jsonString);
    }
}

public enum GameState
{
    MainMenu,
    LevelsMap,
    OnLevel,
    Paused,
}
