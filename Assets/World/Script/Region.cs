using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.HostingServices;
using UnityEngine;

[Serializable]
public class Region 
{
    readonly public int regionID;
    public int[] provincesID;

    public string OWNER_TAG;
    public string CORE_TAG;
    public string[] CLAIM_TAG;

    public Region(int regionID)
    {
        this.regionID = regionID;
    }

    public void InitializeRegion(int scenarioI)
    {
        //WIP
    }
}