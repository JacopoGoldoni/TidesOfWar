using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


public struct PawnComponent : IComponentData
{
    public int formation_ID = 0;
    public bool Loaded = true; 
    
    public PawnComponent()
    {

    }
}