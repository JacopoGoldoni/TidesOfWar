using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public abstract class UnitMovement : MonoBehaviour
{
    //VARIABLES
    [Header("Components")]
    public NavMeshAgent navAgent;
    public UnitManager um;

    [Header("Movement")]
    public Vector3 direction = new Vector3(0, 0, 0);
    public Vector3 rotationVector = new Vector3(0, 0, 0);
    public List<MovementOrder> MovementPoints = new List<MovementOrder>();
    public UnitState unitState;

    [Header("Move and rotation")]
    public float MovementSpeed = 2f;
    public float RotationSpeed = 10f;
    public float MovementThreashold = 0.1f;
    public float RotationThreashold = 0.1f;

    [Header("Area costs")]
    public const float BaseCost = 2f;
    public const float CostWeight = 1f;



    //INITIALIZE
    public virtual void Initialize()
    {
        navAgent = GetComponent<NavMeshAgent>();
        um = GetComponent<UnitManager>();
    }

    //MOVEMENT
    public void SetDestination(Vector2 newDest, Quaternion newQuat)
    {
        MovementPoints.Clear();
        MovementPoints.Add(
             new MovementOrder(
                 Utility.V2toV3(newDest),
                 newQuat
                 )
             );
    }
    public void AddDestination(Vector2 newDest, Quaternion newQuat)
    {
        MovementPoints.Add(
            new MovementOrder(
                Utility.V2toV3(newDest),
                newQuat
                )
            );
    }
    public void ClearDestination()
    {
        MovementPoints.Clear();
    }
    public void UpdateAgentSpeed()
    {
        NavMeshHit navHit;
        navAgent.SamplePathPosition(NavMesh.AllAreas, 0f, out navHit);

        float cost = BaseCost - NavMesh.GetAreaCost(Utility.IndexFromMask(navHit.mask));

        navAgent.speed = MovementSpeed * (1 + cost * CostWeight);
    }
    public void RotateToward(Quaternion targetRot)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, RotationSpeed * Time.deltaTime);
    }

    //CONDITIONS
    public bool IsMoving()
    {
        if(navAgent != null)
        {
            return !(navAgent.remainingDistance == 0f && !navAgent.pathPending);
        }
        return false;
    }
    public bool IsRotating()
    {
        return (Mathf.Abs(CurrentRotation().eulerAngles.y - transform.rotation.eulerAngles.y) > RotationThreashold);
    }
    public bool IsIdle()
    {
        return !IsMoving() && !IsRotating();
    }

    public Vector3 CurrentDestination()
    {
        if(MovementPoints.Count != 0)
        {
            return MovementPoints[0].pos;
        }
        else
        {
            return transform.position;
        }
    }
    public Quaternion CurrentRotation()
    {
        if (MovementPoints.Count != 0)
        {
            return MovementPoints[0].rot;
        }
        else
        {
            return transform.rotation;
        }
    }
}

public struct MovementOrder
{
    public Vector3 pos;
    public Quaternion rot;

    public MovementOrder(Vector3 position, Quaternion rotation)
    {
        pos = position;
        rot = rotation;
    }
}