using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public abstract class UnitMovement : MonoBehaviour
{
    [Header("Components")]
    public NavMeshAgent navAgent;
    public UnitManager um;

    [Header("Movement")]
    public Vector3 direction = new Vector3(0, 0, 0);
    public Vector3 rotationVector = new Vector3(0, 0, 0);
    public List<MovementOrder> MovementPoints = new List<MovementOrder>();

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
    public virtual void SetDestination(Vector2 newDest, Quaternion newQuat)
    {
        MovementPoints.Clear();
        MovementPoints.Add(
             new MovementOrder(
                 GroundBattleUtility.GetMapPosition(newDest),
                 newQuat
                 )
             );

        navAgent.SetDestination(MovementPoints[0].pos);
    }
    public void AddDestination(Vector2 newDest, Quaternion newQuat)
    {
        MovementPoints.Add(
            new MovementOrder(
                GroundBattleUtility.GetMapPosition(newDest),
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
            if(navAgent.remainingDistance >= navAgent.stoppingDistance + 0.01f)
            {
                return true;
            }
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

[Serializable]
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