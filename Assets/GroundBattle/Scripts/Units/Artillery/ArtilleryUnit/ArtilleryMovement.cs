using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryMovement : UnitMovement
{
    private void Start()
    {
        Initialize();
    }

    public void UpdateMovement()
    {
        UpdateMove();

        if (IsMoving())
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
                if (!IsRotating())
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