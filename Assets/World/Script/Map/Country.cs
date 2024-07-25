using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country
{
    public readonly string TAG = "FRA";
    public string COSMETIC_TAG = "FRA";
    public List<string> subjects_TAG;

    public Sprite Flag { get { return GFXUtility.GetFlag(COSMETIC_TAG); } }

    public bool isSubject = false;
    public string overlord_TAG;

    public GovernmentType governmentType;

    public List<Army> armies;

    [Header("Country territory")]
    public int[] OwnedRegions;
    public int[] CoreRegions;
    public int[] ClaimedRegions;

    [Header("Country statistics")]
    public int Stability;
    public int Manpower;
    public int GoldReserve;

    [Header("Population")]
    public int CorePopulation { get; private set; }
    public int NonCorePopulation { get; private set; }
    public int TotalPopulation { get { return CorePopulation + NonCorePopulation; } }

    public Country(string TAG)
    {
        this.TAG = TAG;
        this.COSMETIC_TAG = TAG;
    }

    public void Initialize()
    {
        OwnedRegions = WorldUtility.GetOwnedRegionsByCountry(TAG);
        CoreRegions = WorldUtility.GetCoreRegionsByCountry(TAG);
        ClaimedRegions = WorldUtility.GetClaimedRegionsByCountry(TAG);
    }

    public Sprite GetFlag() { return GFXUtility.GetFlag(TAG); }

    private int GetPopulation(bool onlyCore)
    {
        int p = 0;
        int[] indexes;

        if(onlyCore)
        {
            indexes = CoreRegions;
        }
        else
        {
            indexes = OwnedRegions;
        }

        foreach(int regionIndex in indexes)
        {
            int[] regionProvinceIndexes = WorldUtility.GetRegionByID(regionIndex).provincesID;
            
            foreach (int provinceIndex in regionProvinceIndexes)
            {
                p += WorldUtility.GetProvinceByID(provinceIndex).POPULATION;
            }
        }
        return p;
    }

    public void MonthCalculations()
    {
        //ALL THE CALCULATIONS TO BE MADE EACH NEW MONTH

        //UPDATE POPULATION COUNTERS
        CorePopulation = GetPopulation(true);
        NonCorePopulation = GetPopulation(false);

        //UPDATE MANPOWER
        //UPDATE GOLD RESERVES
    }
}