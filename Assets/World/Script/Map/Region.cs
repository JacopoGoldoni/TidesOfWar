using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Region 
{
    public string name;
    public int[] provincesID;

    public string OWNER_TAG;
    public string CORE_TAG;
    public string[] CLAIM_TAG;

    public Region(string name)
    {
        this.name = name;
    }

    public void InitializeRegion(int scenarioI)
    {
        //WIP
    }
}