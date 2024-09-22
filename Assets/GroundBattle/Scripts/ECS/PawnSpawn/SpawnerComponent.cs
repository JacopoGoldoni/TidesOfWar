using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct SpawnerComponent : IComponentData
{
    public Entity entityPrefab;

    public bool active;
}
public struct SpawnBuffer : IBufferElementData
{
    public float3 spawnLocation;
}