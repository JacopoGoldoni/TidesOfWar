using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Resources;

public class FiniteStateMachine
{
    public State initialState;
    public State currentState;
    private List<State> states = new List<State>();
    private List<Transition> transitions = new List<Transition>();

    public void Initialize()
    {
        currentState = initialState;
    }

    public void Update()
    {
        //UPDATE BEHAVIOUR
        currentState.OnUpdateState();
        //UPDATE STATE
        CheckStateTransitions();
    }
    public bool ChangeState(State newState, bool doExit)
    {
        if(states.Contains(newState))
        {
            if(doExit)
            {
                currentState.OnExitState();
            }

            currentState = newState;

            currentState.OnEnterState();
            
            return true;
        }
        return false;
    }
    public void CheckStateTransitions()
    {
        foreach(Transition t in transitions)
        {
            if(t.neededState.CompareTo(currentState) == 1)
            {
                if(t.IsTransitionable())
                {
                    ChangeState(t.targetState, true);
                    return;
                }
            }
        }
    }

    public void AddState(State newState)
    {
        states.Add(newState);
    }
    public void AddState(string name, Action _onEnterTask, Action _onExitTask, Action _onUpdateTask)
    {
        states.Add(new State(name, _onEnterTask, _onExitTask, _onUpdateTask));
    }
    public void AddStates(List<State> states)
    {
        foreach(State s in states)
        {
            AddState(s);
        }
    }
    public void AddTransition(Transition newTransition)
    {
        transitions.Add(newTransition);
    }
    public void AddTransition(State neededState, State targetState, Transition.TransitionConditionDelegate TransitionCondition)
    {
        transitions.Add( new Transition(neededState, targetState, TransitionCondition));
    }
    public void AddTransitions(List<Transition> transitions)
    {
        foreach (Transition t in transitions)
        {
            AddTransition(t);
        }
    }
}

public class State
{
    public string name;

    private Action _onEnterTask;
    private Action _onExitTask;
    private Action _onUpdateTask;

    public State(string name, Action _onEnterTask, Action _onExitTask, Action _onUpdateTask)
    {
        this.name = name;
        this._onEnterTask = _onEnterTask;
        this._onExitTask = _onExitTask;
        this._onUpdateTask = _onUpdateTask;
    }

    public virtual int CompareTo(State state)
    {
        if (name == state.name)
        {
            return 1;
        }
        return 0;
    }

    public void OnEnterState() { if(_onEnterTask != null) _onEnterTask.Invoke(); }
    public void OnExitState() { if (_onExitTask != null) _onExitTask.Invoke(); }
    public void OnUpdateState() { if (_onUpdateTask != null) _onUpdateTask.Invoke(); }
}

public class AnyState : State, IComparable<State>
{
    public AnyState() : base("Any", null, null, null) { }

    public override int CompareTo(State state)
    {
        return 1;
    }

}

public class Transition
{
    public State neededState;
    public State targetState;

    public delegate bool TransitionConditionDelegate();
    TransitionConditionDelegate TransitionCondition;

    public Transition(State neededState, State targetState, TransitionConditionDelegate TransitionCondition)
    {
        this.neededState = neededState;
        this.targetState = targetState;
        this.TransitionCondition = TransitionCondition;
    }

    public bool IsTransitionable()
    {
        return TransitionCondition();
    }
}