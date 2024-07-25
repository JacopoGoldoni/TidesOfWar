using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OfficerMovement : UnitMovement
{
    public bool HasChangedOrder = false;

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

    public void UpdateMovement()
    {
        UpdateAgentSpeed();
        UpdateMove();
    }

    public override void SetDestination(Vector2 newDest, Quaternion newQuat)
    {
        HasChangedOrder = true;
        base.SetDestination(newDest, newQuat);
    }

    private void UpdateMove()
    {
        //CALCULATE PATH
        if (HasChangedOrder)
        {
            navAgent.SetDestination(MovementPoints[0].pos);
            HasChangedOrder = false;
        }
        else
        if (!IsMoving())
        {
            //NOT MOVING
            if (!IsRotating())
            {
                //NOT ROTATING
                if (((OfficerManager)um).ArePawnIdle())
                {
                    //ALL REGIMENT ARRIVED AT DESTINATION
                    if (MovementPoints.Count > 1)
                    {
                        //PROCEED TO NEXT POINT
                        MovementPoints.RemoveAt(0);
                        HasChangedOrder = true;
                    }
                    else
                    {
                        //LAST ORDER
                        MovementPoints.Clear();
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