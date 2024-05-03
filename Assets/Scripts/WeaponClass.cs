using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponClass : ScriptableObject
{
    public string name;
}

[CreateAssetMenu(fileName = "WeaponClass", menuName = "ScriptableObjects/MeleeClass")]
public class MeleeClass : WeaponClass
{
    public int Damage;
    public int Charge;
}

[CreateAssetMenu(fileName = "WeaponClass", menuName = "ScriptableObjects/RifleClass")]
public class RifleClass : WeaponClass
{
    public int Precision;
    public int Reload;
    public int Weaight;
}

[CreateAssetMenu(fileName = "WeaponClass", menuName = "ScriptableObjects/ArtilleryClass")]
public class ArtilleryClass : WeaponClass
{
    public int Range;
    public int Weight;
    public int Caliber;
}