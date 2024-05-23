using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    public string name;
    public int level = 1;

    public float maxDurability;
    public float durability;

    public Building(string name, int level, float maxDurability)
    {
        this.level = level;
        this.name = name;
        this.maxDurability = maxDurability;
        this.durability = maxDurability;
    }
}

public class Barrack : Building
{
    public Barrack() : base("barrack", 1, 100)
    {

    }
}