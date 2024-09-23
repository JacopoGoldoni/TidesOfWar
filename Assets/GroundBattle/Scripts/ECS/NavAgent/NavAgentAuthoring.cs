using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class NavAgentAuthoring : MonoBehaviour
{
    [SerializeField] private float3 targetPosition;
    [SerializeField] private float moveSpeed;

    private class AuthoringBaker : Baker<NavAgentAuthoring>
    {
        public override void Bake(NavAgentAuthoring authoring)
        {
            Entity authoringEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(authoringEntity, new NavAgentComponent
            {
                move = false,
                targetPosition = authoring.targetPosition,
                moveSpeed = authoring.moveSpeed
            });
            AddBuffer<WaypointBuffer>(authoringEntity);
        }
    }
}