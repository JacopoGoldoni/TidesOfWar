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
        UpdateMove();
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

            if (navAgent.remainingDistance == 0f && !navAgent.pathPending)
            {
                if( ((OfficerManager)um).ArePawnIdle() )
                {
                    //ARRIVED AT DESTINATION
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
                else
                {
                    RotateToward(MovementPoints[0].rot);
                }
            }
        }
    }

    private void RotateToward(Quaternion targetRot)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, RotationSpeed * Time.deltaTime);
    }

    public override void SetDestination(Vector2 newDest, Quaternion newQuat)
    {
        MovementPoints.Clear();
        MovementPoints.Add(
             new MovementOrder(
                 Utility.V2toV3(newDest),
                 newQuat
                 )
             );
        unitState = UnitState.Idle;
    }
    public override void AddDestination(Vector2 newDest, Quaternion newQuat)
    {
        MovementPoints.Add(
            new MovementOrder(
                Utility.V2toV3(newDest), 
                newQuat
                )
            );
    }
}