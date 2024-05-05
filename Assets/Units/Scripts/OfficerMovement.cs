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

    public override void SetDestination(Vector2 newDest, Quaternion newQuat)
    {
        HasChangedOrder = true;
        base.SetDestination(newDest, newQuat);
    }

    private void UpdateMove()
    {
        //IF HAS ORDERS
        if (MovementPoints.Count != 0)
        {
            //CALCULATE PATH
            if(HasChangedOrder)
            {
                navAgent.SetDestination(MovementPoints[0].pos);
                HasChangedOrder = false;
            }
            else
            if (!IsMoving()) //PROBLEM HERE!
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
}