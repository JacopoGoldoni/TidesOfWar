using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public abstract class UnitManager : MonoBehaviour
{
    //UNIT FACTION
    public string TAG = "FRA";

    [Header("Components")]
    public Mesh[] meshes_LODs;
    public Material unitMaterial;
    public Material m;
    public MeshFilter[] mf;
    public MeshRenderer[] mr;
    public SkinnedMeshRenderer[] smr;
    [HideInInspector] public UnitMovement um;

    [Header("Behaviour")]
    public bool Killable = false;

    //INITIALIZE
    public abstract void Initialize();
    public virtual void InitializeMaterial()
    {
        m = Instantiate(unitMaterial);
        if (TAG == Utility.CameraManager.TAG)
        {
            m.SetColor("_Color", Color.green);
        }
        else
        {
            m.SetColor("_Color", Color.red);
        }

        if(mr.Length != 0)
        {
            for(int i = 0; i < mr.Length; i++)
            {
                mr[i].material = m;
            }
        }
        else if(smr.Length != 0)
        {
            for (int i = 0; i < smr.Length; i++)
            {
                smr[i].material = m;
            }
        }
    }
    public virtual void InitializeMeshes()
    {
        for (int i = 0; i < mr.Length; i++)
        {
            mf[i].mesh = meshes_LODs[i];
        }
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

    public abstract float GetWidth();
    public abstract float GetLenght();

    public abstract void OnSelection();
    public abstract void OnDeselection();

    public virtual void Die()
    {
        if(Killable)
        {
            Destroy(transform.gameObject);
        }
    }
}