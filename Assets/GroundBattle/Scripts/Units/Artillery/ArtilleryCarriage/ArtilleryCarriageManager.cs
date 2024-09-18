using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryCarriageManager : UnitManager
{
    public ArtilleryOfficerManager masterArtilleryOfficer;

    public int local_ID;

    public override void Initialize()
    {
        um = GetComponent<ArtilleryCarriageMovement>();

        InitializeMeshes();
        InitializeMaterial();
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

    public override float GetLenght()
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