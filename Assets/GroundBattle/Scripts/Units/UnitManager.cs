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

    [Header("Behaviour")]
    public bool Killable = false;

    //INITIALIZE
    public abstract void Initialize();
    public virtual void Die()
    {
        Destroy(transform.gameObject);
    }
}