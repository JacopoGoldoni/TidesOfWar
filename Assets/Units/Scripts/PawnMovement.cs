using System.Collections;
using System.Collections.Generic;
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

    public override void Initialize()
    {
        um = GetComponent<PawnManager>();
        navAgent = GetComponent<NavMeshAgent>();
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
            //SET DESTINATION
            navAgent.SetDestination(MovementPoints[0].pos);

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