using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Battalion Template", menuName = "ScriptableObjects/Units/Battalion")]
public class BattalionTemplate : ScriptableObject
{
    public UnitType type;

    public CompanyTemplate[] companies;

    [Header("Abilities")]
    public bool SquareFormation;
}