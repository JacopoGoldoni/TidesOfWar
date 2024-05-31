using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioTemplate
{
    public string name;
    public int year;
    public string description;

    public int scenarioID;

    ScenarioData scenarioData;

    public ScenarioTemplate(string name, int year, string description, int scenarioID)
    {
        this.name = name;
        this.year = year;
        this.description = description;
        this.scenarioID = scenarioID;
    }
}

[Serializable]
public class ScenarioData
{
    //PROVINCE -> CITY SIZE POPULATION ORDER DEVELOPMENT BUILDING
    //REGION -> OWNER CORE CLAIM
    //COUNTRY -> CAPITAL_ID GOVERNMENT_TYPE SUBJECTS

    ProvinceInfo[] provinceInfos;
    ProvinceInfo[] regionInfos;
    ProvinceInfo[] countryInfos;
}

[Serializable]
public struct ProvinceInfo
{
    public string name;
    public int ID;
    public Color color;
    //public bool hasCity;
    //public int cityLevel;
    //public int population;
    //public int order;
    //public int development;
    //BUILDINGS
}
[Serializable]
public struct RegionInfo
{
    public string OWNER_TAG;
    public string CORE_TAG;
    public string[] CLAIM_TAG;
}
[Serializable]
public struct CountryInfo
{
    public int capitalProvince_ID;
    public int government_ID;
    public string[] SUBJECT_TAG;
}

public static class ScenraioList
{
    public static ScenarioTemplate[] scenarios =
    {
        new ScenarioTemplate("French revolution", 1797, "Description placeholder", 0),
    };
}