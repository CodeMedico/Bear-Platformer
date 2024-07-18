using Godot;
using System;

public partial class SceneTransition : CanvasLayer
{
    public string sceneTarget;

    public override void _Ready()
    {
        ProcessMode = Node.ProcessModeEnum.Always;
    }
    public void SceneTransitionMethod(String target)
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("CloudIn");
        sceneTarget = target;
    }
    public void OnAnimationFinished(String animation)
    {
        if (animation == "CloudIn")
        {

            if(GetTree().ChangeSceneToFile(sceneTarget) == Error.CantOpen)
            {
                GetTree().ReloadCurrentScene();
            }

            GetNode<AnimationPlayer>("AnimationPlayer").Play("CloudOut");
        }
    }

}
