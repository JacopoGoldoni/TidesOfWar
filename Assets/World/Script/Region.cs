using System.Collections;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.HostingServices;
using UnityEngine;

public class Region 
{
    readonly public int ID;
    public int[] provincesID;

    public string OWNER_TAG;
    public string CORE_TAG;
    public string[] CLAIM_TAG;

    public Region(int regionID)
    {
        this.ID = regionID;
    }
}