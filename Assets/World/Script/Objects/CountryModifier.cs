using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountryModifier : ScriptableObject
{
    public string modifierName;
    public int ID;

    //POLITICAL
    public float politicalPowerFactor = 0f;
    public float stabilityFactor = 0f;
    public float warSupportFactor = 0f;
    public float integrationFactor = 0f;

    //DEMOGRAPHY
    public float populationGrowthFactor = 0f;
    public float migrationAttractionFactor = 0f;

    //ECONOMY
    public float taxFactor = 0f;
    public float expenditureFactor = 0f;

    //ARMY
    public float manpowerFactor = 0f;
    public float armyMoraleFactor = 0f;

    public string GetDescription()
    {
        return "";
    }
}