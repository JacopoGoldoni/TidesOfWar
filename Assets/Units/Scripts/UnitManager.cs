using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitManager : MonoBehaviour
{
    //UNIT FACTION
    public Factions faction = Factions.France;

    //Components
    public MeshRenderer ms;
    public Material m;
    public UnitMovement um;

    //Esthetic
    public Mesh UnitMesh;
    public Material UnitMaterial;

    //INITIALIZE
    public abstract void Initialize();
}
