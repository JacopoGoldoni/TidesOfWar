using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artillery battery Template", menuName = "ScriptableObjects/Units/Artillery Battery")]
public class ArtilleryBatteryTemplate : ScriptableObject
{
    public ArtilleryType type;
    public UnitHardness hardness;

    [Header("Prefabs")]
    public GameObject artilleryOfficerPrefab;

    [Header("Esthetics")]
    public Mesh ArtilleryOfficerMesh;
    public Mesh CannonMesh;
    public Mesh CrewMesh;
    public string ArtilleryBatteryIcon;

    [Header("STATS")]
    public int BatterySize;
    public int CarriageSize;
    public int BaseMorale;
    public int Precision;
    public int Range;
    public float ReloadTime;
    public int Speed;
    public int MaxAmmo;
    public int MeleeAttack;
    public int MeleeDefense;

    [Header("Abilities")]
    public bool ExplosiveShells;
    public bool GrapeShots;
}

public enum ArtilleryType
{
    FieldArtillery,
    Howitzer
}