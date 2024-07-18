using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

public class ActionsData
{
    public List<Transform2D> Positions = new List<Transform2D>();
    public List<Actions> Actions = new List<Actions>();
    public List<bool> isInteraction = new List<bool>();
    public List<bool> isDirectionLeft = new List<bool>();
    public List<bool> isSlimeAlive = new List<bool>();

    public void AddNewPosition(Transform2D position)
    {
        if (Positions.Count < 240)
        {
            Positions.Add(position);
        }
        else
        {
            Positions.RemoveAt(0);
            Positions.Add(position);
        }
    }
    public void AddNewAction(Actions action)
    {
        if (Actions.Count < 240)
        {
            Actions.Add(action);
        }
        else
        {
            Actions.RemoveAt(0);
            Actions.Add(action);
        }
    }
    public void AddDirection(bool isLeft)
    {
        if (isDirectionLeft.Count < 240)
        {
            isDirectionLeft.Add(isLeft);
        }
        else
        {
            isDirectionLeft.RemoveAt(0);
            isDirectionLeft.Add(isLeft);
        }
    }

    public Transform2D TimeRevind()
    {
        return Positions[Positions.Count - 1];
    }
    public Actions ActionsRewind()
    {
        return Actions[Actions.Count - 1];
    }
    public bool IsDirectionLeft()
    {
        return isDirectionLeft[isDirectionLeft.Count - 1];
    }
    public void DeleteLastPosition()
    {
        Positions.RemoveAt(Positions.Count - 1);
    }
    public void DeleteLastAction()
    {
        Actions.RemoveAt(Actions.Count - 1);
    }
    public void DeleteLastDirection()
    {
        isDirectionLeft.RemoveAt(isDirectionLeft.Count - 1);
    }
    public void AddSlimeAlive(bool alive)
    {
        if (isSlimeAlive.Count < 240)
        {
            isSlimeAlive.Add(alive);
        }
        else
        {
            isSlimeAlive.RemoveAt(0);
            isSlimeAlive.Add(alive);
        }
    }
    public bool IsSlimeAlive()
    {
        return isSlimeAlive[isSlimeAlive.Count - 1];
    }
    public void DeleteLastSlimeState()
    {
        isSlimeAlive.RemoveAt(isSlimeAlive.Count - 1);
    }
    public void AddInteraction(bool isInteraction)
    {
        if (this.isInteraction.Count < 240)
        {
            this.isInteraction.Add(isInteraction);
        }
        else
        {
            this.isInteraction.RemoveAt(0);
            this.isInteraction.Add(isInteraction);
        }
    }
    public bool IsInteraction()
    {
        return isInteraction[isInteraction.Count - 1];
    }

    public void DeleteLastInteraction()
    {
        isInteraction.RemoveAt(isInteraction.Count - 1);
    }
}

public enum Actions
{
    Walk,
    Dash,
    Stay,
    Jump,
}
