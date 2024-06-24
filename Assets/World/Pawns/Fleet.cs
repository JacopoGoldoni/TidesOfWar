using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fleet
{
    public int ID;
    public string TAG = "FRA";
    public string fleetName;

    public List<Ship> ships;

    public bool docked;

    public Fleet(string TAG)
    {
        this.TAG = TAG;
        WorldUtility.AppendFleet(this);

        Initialize();
    }

    private void Initialize()
    {
        //GENERATE ARMY NAME
        fleetName = TAG + ID;
    }

    public int GetFleetSize()
    {
        return ships.Count;
    }
}

public class Ship
{
    public ShipClass shipClass;

    public string TAG;
    public string name;
    public int hull;
    
    public Ship(string TAG, ShipClass shipClass)
    {
        this.TAG = TAG;
        this.shipClass = shipClass;

        Initialize();
    }

    private void Initialize()
    {
        hull = shipClass.max_hull;
    }
}