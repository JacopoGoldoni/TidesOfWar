using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Company Template", menuName = "ScriptableObjects/Units/Company")]
public class CompanyTemplate : ScriptableObject
{
    public UnitType type;
    public UnitHardness hardness;

    public GameObject officerPrefab;

    public int CompanySize;

    [Header("Esthetics")]
    public Mesh[] OfficerMesh_LODS;
    public Material officerMaterial;
    public Mesh[] SoldierMesh_LODS;
    public Material soldierMaterial;
    public string CompanyIcon;

    [Header("STATS")]
    public int BaseMorale;
    public int Precision;
    public int Range;
    public int Speed;
    public int ReloadTime;
    public int MaxAmmo;
    public int MeleeAttack;
    public int MeleeDefense;

    [Header("Abilities")]
    public bool MultipleFire;
    public bool Fortification;
    public bool Skirmish;

    [Header("Dimensions")]
    public float FilesDistances = 0.5f;
    public float RankDistance = 1.0f;
}

public enum UnitType
{
    Infantry,
    Cavalry
}
public enum UnitHardness
{
    Light,
    Medium,
    Heavy
}