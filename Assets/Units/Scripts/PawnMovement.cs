using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PawnMovement : UnitMovement
{
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {
        um = GetComponent<PawnManager>();
        navAgent = GetComponent<NavMeshAgent>();

        SetDestination(transform.position, transform.rotation);
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
            //SET DESTINATION
            navAgent.SetDestination(MovementPoints[0].pos);
            unitState = UnitState.Walk;

            if (navAgent.remainingDistance == 0f && !navAgent.pathPending)
            {
                //ARRIVED AT DESTINATION
                MovementPoints.RemoveAt(0);
                unitState = UnitState.Idle;
            }
        }
    }

    //MOVEMENT OVERRIDE
    public override void SetDestination(Vector2 newDest, Quaternion newQuat)
    {
        MovementPoints.Clear();
        MovementPoints.Add(
             new MovementOrder(
                 Utility.V2toV3(newDest),
                 newQuat
                 )
             );
    }
    public override void AddDestination(Vector2 newDest, Quaternion newQuat)
    {
        throw new System.NotImplementedException();
    } 
}