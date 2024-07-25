using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Province
{
    public string name;
    public readonly Color colorCode;

    public int regionID;

    public string OWNER_TAG;

    public bool isCostal;

    public int POPULATION;
    public int ORDER;
    public int DEVELOPMENT;
    public ProvinceType PROVINCE_TYPE;

    public List<BuildingSlot> BUILDINGS_SLOTS;

    public Province(string name, Color colorCode)
    {
        this.name = name;
        this.colorCode = colorCode;

        BUILDINGS_SLOTS = new List<BuildingSlot>(12);
    }

    public void Update()
    {
        //UPDATE POPULATION
            //DEVELOPMENT
            //REGION MODIFIERS
            //NATIONAL MODIFIERS
        int POPULATION_RATE = 0;
        POPULATION = POPULATION_RATE * POPULATION;
        if (POPULATION < 0) { POPULATION = 0; }

        //UPDATE DEVELOPMENT
            //REGIONAL MODIFIERS
            //NATIONAL MODIFIERS
            //INVESTMENTS
        int DEVELOPMENT_RATE = 0;
        DEVELOPMENT += DEVELOPMENT_RATE;
        if (DEVELOPMENT < 0) { DEVELOPMENT = 0; }

        //UPDATE ORDER
            //REGIONAL MODIFIERS
            //NATIONAL MODIFIERS
            //GOVERNMENT MODIFIERS
        int ORDER_RATE = 0;
        ORDER += ORDER_RATE;
        if (ORDER < 0) { ORDER = 0; }
    }

    public void AddBuilding(int buildingID, int level)
    {
        bool isNew = true;
        int localIndex = 0;

        for(int i = 0; i < BUILDINGS_SLOTS.Count; i++)
        {
            if (BUILDINGS_SLOTS[i] != null)
            {
                if(BUILDINGS_SLOTS[i].buildingID == buildingID)
                {
                    isNew = false;
                    localIndex = i;
                    break;
                }
            }
        }

        if(isNew)
        {
            BuildingSlot newSlot = new BuildingSlot(level, buildingID);

            BUILDINGS_SLOTS.Add(newSlot);
        }
        else
        {
            BuildingSlot localSlot = BUILDINGS_SLOTS[localIndex];
            localSlot.level =+ level;
        }
    }
    public void RemoveBuilding(string name)
    {
        /*
        foreach (Building b in BUILDINGS)
        {
            if(b.name == name)
            {
                BUILDINGS.Remove(b);
                return;
            }
        }
        */
    }
}

public enum ProvinceType
{
    plain,
    city,
    hill,
    mountain,
    desert
}