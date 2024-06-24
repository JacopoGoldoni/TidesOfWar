using System;
using System.Collections;
using UnityEngine;

public class BehaviourTree
{
    private Node _root = null;

    public void Initialize()
    {

    }
    public void Execute()
    {
        if(_root != null)
        {
            _root.Evaluate();
        }
    }
}