using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class UnitMovement : MonoBehaviour
{
    //Components
    public NavMeshAgent navAgent;
    public UnitManager um;

    public Vector3 direction = new Vector3(0, 0, 0);
    public Vector3 rotationVector = new Vector3(0, 0, 0);

    public List<MovementOrder> MovementPoints = new List<MovementOrder>();

    public float MovementSpeed = 2f;
    public float RotationSpeed = 10f;
    public float MovementThreashold = 0.1f;
    public float RotationThreashold = 1f;

    //AREA COSTS
    public const float BaseCost = 2f;
    public const float CostWeight = 1f;

    public UnitState unitState = UnitState.Idle;

    //INITIALIZE
    public abstract void Initialize();

    //MOVEMENT
    public abstract void SetDestination(Vector2 newDest, Quaternion newQuat);
    public abstract void AddDestination(Vector2 newDest, Quaternion newQuat);
    public void UpdateAgentSpeed()
    {
        NavMeshHit navHit;
        navAgent.SamplePathPosition(NavMesh.AllAreas, 0f, out navHit);

        float cost = BaseCost - NavMesh.GetAreaCost(IndexFromMask(navHit.mask));

        navAgent.speed = MovementSpeed * (1 + cost * CostWeight);
    }

    private int IndexFromMask(int mask)
    {
        for(int i = 0; i < 32; i++)
        {
            if((1 << i) == mask)
            {
                return i;
            }
        }
        return -1;
    }

    //CONDITION FUNCTIONS
    public bool IsMoving()
    {
        if(unitState == UnitState.Walk || unitState == UnitState.Running)
        {
            return true;
        }

        return false;
    }
    public bool IsIdle()
    {
        return unitState == UnitState.Idle;
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
        this.pos = position;
        this.rot = rotation;
    }
}