using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WorldUtility
{
    static List<Country> countries = new List<Country>();
    static List<Region> regions = new List<Region>();
    static List<Province> provinces = new List<Province>();

    //GETTERS
    public static Country GetCountryByTAG(string TAG)
    {
        foreach(Country c in countries)
        {
            if(c.TAG == TAG)
            {
                return c;
            }
        }
        return null;
    }
    public static Country[] GetAllCountries()
    {
        return countries.ToArray();
    }
    public static Country[] GetAllOtherCountries(string TAG)
    {
        Country c = GetCountryByTAG(TAG);
        List<Country> all = GetAllCountries().ToList();

        all.Remove(c);

        return all.ToArray();
    }
    public static Region GetRegionByID(int ID)
    {
        foreach (Region r in regions)
        {
            if (r.ID == ID)
            {
                return r;
            }
        }
        return null;
    }
    public static Province GetProvinceByID(int ID)
    {
        foreach (Province p in provinces)
        {
            if (p.ID == ID)
            {
                return p;
            }
        }
        return null;
    }

    public static Province[] GetProvincesInRegion(int ID)
    {
        Region r = GetRegionByID(ID);
        List<Province> p = new List<Province>();

        foreach(int provinceID in r.provincesID)
        {
            p.Add(GetProvinceByID(provinceID));
        }

        return provinces.ToArray();
    }
    public static Region GetRegionOfProvince(int ID)
    {
        return GetRegionByID( GetProvinceByID(ID).regionID );
    }
    public static Country GetProvinceOwner(int ID)
    {
        return GetCountryByTAG(GetProvinceByID(ID).OWNER_TAG);
    }
    public static Country GetRegionOwner(int ID)
    {
        return GetCountryByTAG(GetRegionByID(ID).OWNER_TAG);
    }
}