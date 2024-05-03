using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OfficerMovement : UnitMovement
{
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {
        um = GetComponent<OfficerManager>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAgentSpeed();
        if (
            ((OfficerManager)um).GetState().name == "Idle" || ((OfficerManager)um).GetState().name == "Moving"
            )
        {
            UpdateMove();
        }
    }

    private void UpdateMove()
    {
        //IF HAS ORDERS
        if (MovementPoints.Count != 0)
        {
            if(unitState == UnitState.Idle)
            {
                //SET DESTINATION
                navAgent.SetDestination(MovementPoints[0].pos);
                unitState = UnitState.Walk;
            }

            if (!IsMoving())
            {
                //NOT MOVING
                if(!IsRotating())
                {
                    //NOT ROTATING
                    if (((OfficerManager)um).ArePawnIdle())
                    {
                        //ALL REGIMENT ARRIVED AT DESTINATION
                        if (MovementPoints.Count > 1)
                        {
                            //PROCEED TO NEXT POINT
                            MovementPoints.RemoveAt(0);
                            unitState = UnitState.Idle;
                        }
                        else
                        {
                            //LAST ORDER
                            MovementPoints.Clear();
                            unitState = UnitState.Idle;
                        }
                    }
                }
                else
                {
                    RotateToward(MovementPoints[0].rot);
                }
            }
        }
    }
}