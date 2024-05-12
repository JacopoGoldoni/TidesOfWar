using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class WarMachineV1
{
    //STATE SPACE X
    struct State
    {
        public Vector2[] C_Positions;
        public Vector2[] C_Orientations;
        public int[] C_UnitValues;

        public Vector2[] E_Positions;
        public Vector2[] E_Orientations;
        public int[] E_UnitValues;
    }
    State currentState;

    //ACTION SPACE U
    struct Action
    {
        public Vector2[] C_Positions;
        public Vector2[] C_Orientations;
    }
    Action currentAction;

    //POLICY

}