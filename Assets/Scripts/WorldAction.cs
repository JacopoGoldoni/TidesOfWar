using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WorldAction
{
    string description;
    Action action;

    public WorldAction(string description, Action action)
    {
        this.description = description;
        this.action = action;
    }

    public void Call()
    {
        action.Invoke();
    }
}

public static class WorldActionList
{
    static Dictionary<string, WorldAction> actionList = new Dictionary<string, WorldAction>();

    public static void AddAction(string name, string description, Action action)
    {
        actionList.Add(name, new WorldAction(description, action));
    }
    public static WorldAction GetActionByName(string name)
    {
        if(actionList.ContainsKey(name))
        {
            return actionList[name];
        }
        return null;
    }
}