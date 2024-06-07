using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitManager : MonoBehaviour
{
    //UNIT FACTION
    public Factions faction = Factions.France;

    //Components
    [HideInInspector] public MeshRenderer ms;
    [HideInInspector] public Material m;
    [HideInInspector] public UnitMovement um;

    [Header("Esthetics")]
    public Mesh UnitMesh;
    public Material UnitMaterial;

    //INITIALIZE
    public abstract void Initialize();
}