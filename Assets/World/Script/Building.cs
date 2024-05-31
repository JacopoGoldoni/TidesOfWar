using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    public string name;

    public float maxDurability;

    public Building(string name, float maxDurability)
    {
        this.name = name;
        this.maxDurability = maxDurability;
    }
}

public class BuildingSlot
{
    public float durability;
    public int level;

    public int buildingID;

    public BuildingSlot(int level, int buildingID)
    {
        this.level = level;
        this.buildingID = buildingID;

        //TODO TAKE BUILDING INFO FOR INITIAL DURABILITY
    }
}