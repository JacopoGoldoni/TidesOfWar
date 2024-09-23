using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    public string name;
    public string sprite;
    public string description;

    public float maxDurability;

    public Building(string name, string sprite, string description, float maxDurability)
    {
        this.name = name;
        this.sprite = sprite;
        this.description = description;
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