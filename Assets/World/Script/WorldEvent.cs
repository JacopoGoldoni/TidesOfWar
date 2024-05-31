using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEvent
{
    string title;
    string description;
    Sprite image;

    Action OnCall;

    struct Option
    {
        public string name;
        public string description;
        public Action OnSelect;
    }
    Option[] options;

    public WorldEvent(string title, string description, Action OnCall)
    {
        this.title = title;
        this.description = description;
        this.OnCall = OnCall;
    }

    public void Call()
    {
        OnCall.Invoke();
    }

    public void CallOption(int i)
    {
        options[i].OnSelect.Invoke();
    }
}