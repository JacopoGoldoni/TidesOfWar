using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Battalion Template", menuName = "ScriptableObjects/Units/Battalion")]
public class BattalionTemplate : ScriptableObject
{
    public UnitType type;
    public BattalionType battalionType;
    public string BattalionIcon;

    public GameObject captainPrefab;

    public Mesh captainMesh;
    public Material captainMaterial;

    public CompanyTemplate[] companies;

    [Header("Abilities")]
    public bool SquareFormation;

    [Header("Dimensions")]
    public float CompanyXDistance = 2f;
    public float CompanyYDistance = 6f;
}

public enum BattalionType
{
    Line,
    Light
}