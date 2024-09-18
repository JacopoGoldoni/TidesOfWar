using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class NavAgentAuthoring : MonoBehaviour
{
    [SerializeField] public Vector3 targetPos;
    [SerializeField] public float moveSpeed;

    private class AuthoringBake : Baker<NavAgentAuthoring>
    {
        public override void Bake(NavAgentAuthoring authoring)
        {
            Entity authoringEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(authoringEntity, new NavAgentComponent
            {
                targetPos = authoring.targetPos,
                moveSpeed = authoring.moveSpeed
            });
            AddBuffer<WaypointBuffer>(authoringEntity);
        }
    }
}