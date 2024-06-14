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
    public virtual void InitializeMaterial()
    {
        m = Instantiate(UnitMaterial);
        if (Utility.Camera.GetComponent<CameraManager>().faction == faction)
        {
            m.SetColor("_Color", Color.green);
        }
        else
        {
            m.SetColor("_Color", Color.red);
        }
        ms.material = m;
    }

    public virtual void ReceiveMovementOrder(bool add, Vector2 pos, Quaternion rot)
    {
        if (add)
        {
            um.AddDestination(pos, rot);
        }
        else
        {
            um.SetDestination(pos, rot);
        }
    }

    public virtual void Die()
    {
        Destroy(transform.gameObject);
    }
}