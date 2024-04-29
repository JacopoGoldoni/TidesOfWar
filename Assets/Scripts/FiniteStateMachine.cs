using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    public State currentState;
    List<State> states;

    public void CheckTransition(State newState, bool condition)
    {
        if(states.Contains(newState) && condition)
        {
            currentState = newState;
        }
    }
}

public class State
{
    string Name;
}