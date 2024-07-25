using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryCrewManager : UnitManager
{
    public ArtilleryManager masterArtillery;

    public int ID;

    public override void Initialize()
    {
        ms = GetComponent<MeshRenderer>();
        um = GetComponent<ArtilleryMovement>();

        Material m = Instantiate(UnitMaterial);

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

    public void MoveTo(Vector2 dest, Quaternion quat)
    {
        um.SetDestination(dest, quat);
    }

    public void Update()
    {
        
    }

    public void Die()
    {
        Destroy(transform.gameObject);
    }
}