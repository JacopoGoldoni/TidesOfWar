using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Experimental.AI;
using System;
using Unity.Transforms;
using Unity.Collections;

public partial struct SpawnerSystem : ISystem
{
    private void OnCreate(ref SystemState state)
    {

    }
    private void OnDestroy(ref SystemState state)
    {
        
    }
    private void OnUpdate(ref SystemState state)
    {
        foreach (var (spawnComponent, entity) in SystemAPI.Query<RefRW<SpawnerComponent>>().WithEntityAccess())
        {
            DynamicBuffer<SpawnBuffer> spawnBuffer = state.EntityManager.GetBuffer<SpawnBuffer>(entity);

            ProcessSpawner(ref state, spawnComponent, spawnBuffer);
        }
    }

    private void ProcessSpawner(ref SystemState state, RefRW<SpawnerComponent> spawner, DynamicBuffer<SpawnBuffer> spawnBuffer)
    {
        if(spawner.ValueRO.active)
        {
            for (int i = 0; i < spawnBuffer.Length; i++)
            {
                Entity spawnedEntity = state.EntityManager.Instantiate(spawner.ValueRO.entityPrefab);

                state.EntityManager.SetComponentData(spawnedEntity, LocalTransform.FromPosition(spawnBuffer[i].spawnLocation));
            }

            spawner.ValueRW.active = false;
        }
    }
}