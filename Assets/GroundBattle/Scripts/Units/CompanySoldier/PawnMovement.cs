using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class PawnMovement : UnitMovement
{
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    void Update()
    {
        UpdateMove();

        if(IsMoving())
        {
            UpdateAgentSpeed();
        }
    }

    private void UpdateMove()
    {

        //IF HAS ORDERS
        if (MovementPoints.Count != 0)
        {
            // OLD VERSION
            //navAgent.SetDestination(MovementPoints[0].pos);

            // NEW VERSION
            ((PawnManager)um).navAgentAuthoring.targetPos = MovementPoints[0].pos;

            if (!IsMoving())
            {
                //ARRIVED AT DESTINATION
                if(!IsRotating())
                {
                    //ALIGNED WITH FORMATION
                    MovementPoints.RemoveAt(0);
                }
                else
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, CurrentRotation(), RotationSpeed * Time.deltaTime);
                }
            }
        }
    }
}