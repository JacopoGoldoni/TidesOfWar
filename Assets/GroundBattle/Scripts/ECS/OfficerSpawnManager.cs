using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEditor.Search;
using UnityEngine;
using Unity.Mathematics;

public class OfficerSpawnManager : MonoBehaviour
{
    public void Spawn(float3[] spawnPoints)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery query = entityManager.CreateEntityQuery(typeof(SpawnerComponent));
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);
        
        if (entities.Length > 0)
        {
            Entity spawner = entities[0];

            //APPEND SPAWN LOCATIONS
            DynamicBuffer<SpawnBuffer> spawnBuffer = entityManager.GetBuffer<SpawnBuffer>(spawner);
            foreach(float3 p in spawnPoints)
            {
                spawnBuffer.Add(new SpawnBuffer { spawnLocation = p});
            }

            //ACTIVATE SPAWN COMPONENT
            SpawnerComponent spawnerComponent = entityManager.GetComponentData<SpawnerComponent>(spawner);
            spawnerComponent.active = true;

            entityManager.SetComponentData(spawner, spawnerComponent);
        }

        entities.Dispose();
    }
}
