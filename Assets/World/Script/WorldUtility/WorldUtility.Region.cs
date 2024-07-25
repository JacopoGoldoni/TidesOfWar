using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class WorldUtility
{
    static Region[] regions;
    static string regionPath = "JSON/Regions/";
    public static void LoadRegions()
    {
        TextAsset[] regionAssets = Resources.LoadAll<TextAsset>(regionPath);

        if (regionAssets.Length == 0) { return; }

        regions = new Region[regionAssets.Length];

        Debug.Log(regionAssets.Length + " regions found.");

        for (int i = 0; i < regionAssets.Length; i++)
        {
            RegionInfo regionInfo = JsonUtility.FromJson<RegionInfo>(regionAssets[i].text);

            Region region = new Region(regionInfo.name);

            Debug.Log("Region added:\n" +
                "Name: " + regionInfo.name + "\n" +
                "ID: " + regionInfo.ID);

            for (int j = 0; j < regionInfo.provinces.Length; j++)
            {
                int provinceID = regionInfo.provinces[j];

                GetProvinceByID(provinceID).regionID = regionInfo.ID;
            }

            regions[regionInfo.ID] = region;
        }
    }

    public static Region GetRegionByID(int ID)
    {
        return regions[ID];
    }
    public static int[] GetOwnedRegionsByCountry(string TAG)
    {
        List<int> countryRegionIndexes = new List<int>();
        for (int i = 0; i < regions.Length; i++)
        {
            if (regions[i].OWNER_TAG == TAG)
            {
                countryRegionIndexes.Add(i);
            }
        }
        return countryRegionIndexes.ToArray();
    }
    public static int[] GetCoreRegionsByCountry(string TAG)
    {
        List<int> countryRegionIndexes = new List<int>();
        for (int i = 0; i < regions.Length; i++)
        {
            if (regions[i].CORE_TAG == TAG)
            {
                countryRegionIndexes.Add(i);
            }
        }
        return countryRegionIndexes.ToArray();
    }
    public static int[] GetClaimedRegionsByCountry(string TAG)
    {
        List<int> countryRegionIndexes = new List<int>();
        for (int i = 0; i < regions.Length; i++)
        {
            Region r = regions[i];
            foreach (string s in r.CLAIM_TAG)
            {
                if (s == TAG)
                {
                    countryRegionIndexes.Add(i);
                    break;
                }
            }
        }
        return countryRegionIndexes.ToArray();
    }
    public static int GetRegionIndexOfProvince(int ID)
    {
        return GetProvinceByID(ID).regionID;
    }
    public static string GetRegionOwner(int ID)
    {
        return GetRegionByID(ID).OWNER_TAG;
    }
}
