using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artillery class", menuName = "ScriptableObjects/Weapons/Artillery")]
public class ArtilleryClass : ScriptableObject
{
    public string artilleryName;

    public Sprite artillerySprite;
    public Mesh artilleryMesh;

    public bool indirectFire;
    public float ReloadTime;
    public float Precision;
    public float Range;

    public int crewPerGun;

    public int Cost;
}