using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree
{
    public abstract class Node
    {
        public abstract Action ExecuteNode();
    }
    public class SelectorNode : Node
    {
        Func<bool> cond;
        Action ifTrue;
        Action ifFalse;
        
        public SelectorNode(Func<bool> cond, Action ifTrue, Action ifFalse)
        {
            this.cond = cond;
            this.ifTrue = ifTrue;
            this.ifFalse = ifFalse;
        }
        
        public override Action ExecuteNode()
        {
            if (cond())
            {
                return ifTrue;
            }
            else
            {
                return ifFalse;
            }
        }
    }
    public class SequencerNode : Node
    {
        List<Action> actions;

        public SequencerNode(List<Action> actions)
        {
            this.actions = actions;
        }

        public override Action ExecuteNode()
        {
            return () => {
                foreach (Action a in actions)
                {
                    a();
                }
            };
        }
    }
    Node startingNode;

    Dictionary<string, Node> nodes = new Dictionary<string, Node>();

    public Action ExecuteBehaviourTree()
    {
        return startingNode.ExecuteNode();
    }

    public void AddSelector(string name, Func<bool> cond, Action ifTrue, Action ifFalse)
    {
        if(nodes.Count == 0)
        {
            startingNode = new SelectorNode(cond, ifTrue, ifFalse);
        }
        else
        {
            nodes.Add(name, new SelectorNode(cond, ifTrue, ifFalse));
        }
    }
    public void AddSequence(string name, List<Action> actions)
    {
        if (nodes.Count == 0)
        {
            startingNode = new SequencerNode(actions);
        }
        else
        {
            nodes.Add(name, new SequencerNode(actions));
        }
    }
}