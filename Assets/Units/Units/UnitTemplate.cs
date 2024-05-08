using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Template", menuName = "ScriptableObjects/Unit")]
public class UnitTemplate : ScriptableObject
{
    public UnitType type;

    public int RegimentSize;

    [Header("Esthetics")]
    public Mesh OfficerMesh;
    public Mesh PawnMesh;
    public Sprite RegimentIcon;

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