using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject prefab;

    private class AuthoringBaker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            Entity authoringEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(authoringEntity, new SpawnerComponent
            {
                entityPrefab = GetEntity(authoring.prefab ,TransformUsageFlags.Dynamic),
                active = false
            });
            AddBuffer<SpawnBuffer>(authoringEntity);
        }
    }
}
