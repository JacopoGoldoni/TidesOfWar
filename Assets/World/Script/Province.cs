using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Province
{
    public string name;
    public int regionID;
    public readonly Color colorCode;

    public string OWNER_TAG;

    public int POPULATION;
    public int ORDER;
    public int DEVELOPMENT;
    public ProvinceType PROVINCE_TYPE;

    //public List<Building> BUILDINGS;

    public Province(string name, Color colorCode)
    {
        this.name = name;
        this.colorCode = colorCode;
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

    public void AddBuilding(Building b)
    {
        //BUILDINGS.Add(b);
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
    town,
    city,
    hill,
    mountain,
    costal,
    desert
}