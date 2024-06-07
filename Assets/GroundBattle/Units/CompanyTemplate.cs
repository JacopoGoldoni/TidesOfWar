using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Company Template", menuName = "ScriptableObjects/Units/Company")]
public class CompanyTemplate : ScriptableObject
{
    public UnitType type;

    public int CompanySize;

    [Header("Esthetics")]
    public Mesh OfficerMesh;
    public Mesh SoldierMesh;
    public Sprite CompanyIcon;

    [Header("STATS")]
    public int BaseMorale;
    public int Precision;
    public int Speed;
    public int MaxAmmo;
    public int MeleeAttack;
    public int MeleeDefense;

    [Header("Abilities")]
    public bool MultipleFire;
    public bool Fortification;
    public bool SquareFormation;
    public bool Skirmish;
}

public enum UnitType
{
    Infantry,
    Cavalry,
    Artillery
}