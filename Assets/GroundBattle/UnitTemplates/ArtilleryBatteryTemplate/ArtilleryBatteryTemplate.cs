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
    public GameObject cannonPrefab;

    [Header("Esthetics")]
    public Mesh ArtilleryOfficerMesh;
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

    [Header("Bone names")]
    public string Root = "Root";
    public string Body = "Root/Body";
    public string Barrel = "Root/Body/Barrel";
    public string LeftWheel = "Root/Wheel.L";
    public string RightWheel = "Root/Wheel.R";

    [Header("Crew positions names")]
    public string[] FirstClassCrew_Positions;
}

public enum ArtilleryType
{
    FieldArtillery,
    Howitzer
}