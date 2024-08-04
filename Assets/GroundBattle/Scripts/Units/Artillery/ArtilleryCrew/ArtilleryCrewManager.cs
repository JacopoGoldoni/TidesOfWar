using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryCrewManager : UnitManager
{
    public ArtilleryManager masterArtillery;

    public int ID;

    public override void Initialize()
    {
        mr = GetComponent<MeshRenderer>();
        um = GetComponent<ArtilleryCrewMovement>();

        Material m = Instantiate(UnitMaterial);

        if (Utility.Camera.GetComponent<CameraManager>().faction == faction)
        {
            m.SetColor("_Color", Color.green);
        }
        else
        {
            m.SetColor("_Color", Color.red);
        }

        mr.material = m;
    }

    public void MoveTo(Vector2 dest, Quaternion quat)
    {
        um.SetDestination(dest, quat);
    }

    public void Die()
    {
        Destroy(transform.gameObject);
    }

    public override float GetWidth()
    {
        throw new System.NotImplementedException();
    }

    public override void OnSelection()
    {
        throw new System.NotImplementedException();
    }
    public override void OnDeselection()
    {
        throw new System.NotImplementedException();
    }
}