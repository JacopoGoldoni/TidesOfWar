﻿using System;
using UnityEngine;

[Serializable]
public struct ProvinceInfo
{
    //GLOBAL
    public string name;
    public int ID;
    public Color color;
    public int provinceType;

    //SCENARIO
    public int cityLevel;
    public int population;
    public int order;
    public int development;
    //BUILDINGS
}

[Serializable]
public struct RegionInfo
{
    //GLOBAL
    public string name;
    public int ID;
    public int[] provinces;

    //SCENARIO
}

[Serializable]
public struct CountryInfo
{
    //GLOBAL
    public string name;
    public string TAG;

    //SCENARIO
    public int capitalProvince_ID;
}

[Serializable]
public struct IdeologyInfo
{
    public string name;
    public float LR; //-1 Right +1 Left
}

[Serializable]
public struct CountryModifiers
{
    public string name;
    public string image;
}

[Serializable]
public struct LawInfo
{
    public string name;
    public string image;
}

[Serializable]
public struct ResourceInfo
{
    public string name;
    public string icon;
}

[Serializable]
public struct BuildingInfo
{
    public int id;

    public string name;
    public string icon;
    public string description;
    public float maxDurability;
}