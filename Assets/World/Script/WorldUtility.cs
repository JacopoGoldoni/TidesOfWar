using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WorldUtility
{
    //DATA LISTS
    static Province[] provinces;
    static Region[] regions;
    static Country[] countries;
    static Building[] buildings;
    static Law[] laws;
    static Technology[] techs;

    //ARMY & NAVY
    static List<Army> armies = new List<Army>();
    static List<Fleet> Fleets = new List<Fleet>();

    //MAPS
    static Texture2D rasterizedMap;

    //PATHS
    static string rasterizedMapPath = "RasterizedMap_Europe";
    static string provincePath = "JSON/Provinces/";
    static string regionPath = "JSON/Regions/";
    static string countryPath = "JSON/Countries/";
    static string buildingPath = "JSON/Buildings/";
    static string ideologyPath = "JSON/Ideologies/";
    static string lawPath = "JSON/Laws/";

    //LOADERS
    public static void LoadRasterizedMap()
    {
        rasterizedMap = Resources.Load<Texture2D>(rasterizedMapPath);
        if(rasterizedMap != null)
        {
            Debug.Log("Rasterized map loaded");
        }
    }
    public static void LoadProvinces()
    {
        TextAsset[] provinceAssets = Resources.LoadAll<TextAsset>(provincePath);

        if(provinceAssets.Length == 0) { return; }

        provinces = new Province[provinceAssets.Length];

        Debug.Log(provinceAssets.Length + " provinces found.");

        for(int i = 0; i < provinceAssets.Length; i++)
        {
            ProvinceInfo provinceInfo = JsonUtility.FromJson<ProvinceInfo>(provinceAssets[i].text);

            Province province = new Province(provinceInfo.name ,provinceInfo.color);

            Debug.Log("Province added:\n" + 
                "Name: " + provinceInfo.name + "\n" + 
                "ID: " + provinceInfo.ID + "\n" +
                "ColorCode: " + provinceInfo.color);

            provinces[provinceInfo.ID] = province;
        }
    }
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

            for(int j = 0; j < regionInfo.provinces.Length; j++)
            {
                int provinceID = regionInfo.provinces[j];

                GetProvinceByID(provinceID).regionID = regionInfo.ID;
            }

            regions[regionInfo.ID] = region;
        }
    }
    public static void LoadCountries()
    {
        TextAsset[] countryAssets = Resources.LoadAll<TextAsset>(provincePath);

        if (countryAssets.Length == 0) { return; }

        countries = new Country[countryAssets.Length];

        Debug.Log(countryAssets.Length + " coutnries found.");

        for (int i = 0; i < countryAssets.Length; i++)
        {
            CountryInfo countryInfo = JsonUtility.FromJson<CountryInfo>(countryAssets[i].text);

            Country country = new Country(countryInfo.TAG);

            Debug.Log("Country added:\n" +
                "Name: " + countryInfo.name + "\n" +
                "TAG: " + countryInfo.TAG + "\n");

            countries[i] = country;
        }
    }
    public static void LoadLaws()
    {
        TextAsset[] lawAssets = Resources.LoadAll<TextAsset>(lawPath);

        if (lawAssets.Length == 0) { return; }

        laws = new Law[lawAssets.Length];

        Debug.Log(lawAssets.Length + " laws found.");

        for (int i = 0; i < lawAssets.Length; i++)
        {
            LawInfo lawInfo = JsonUtility.FromJson<LawInfo>(lawAssets[i].text);

            Law law = new Law();

            Debug.Log("Country added:\n" +
                "Name: " + lawInfo.name + "\n");

            laws[i] = law;
        }
    }

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
        return regions[ID];
    }
    public static int GetProvincesCount()
    {
        return provinces.Length;
    }
    public static Province GetProvinceByID(int ID)
    {
        return provinces[ID];
    }
    public static Province GetProvinceByColorCode(Color colorCode)
    {
        for(int i = 0; i < provinces.Length; i++)
        {
            if (provinces[i].colorCode == colorCode)
            {
                return provinces[i];
            }
        }
        return null;
    }
    public static Law[] GetAllLaws()
    {
        return laws;
    }
    public static Technology[] GetAllTechs()
    {
        return techs;
    }
    public static Technology GetTechByID(int ID)
    {
        for(int i = 0; i < techs.Length; i++)
        {
            if (techs[i].techID == ID)
            {
                return techs[i];
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

    public static Vector2 GetWorldUV(Vector3 worldPosition)
    {
        Vector2 v2 = Utility.V3toV2(worldPosition);
        Vector2 UV = new Vector2
            (v2.x / 1152f, v2.y / 512f);

        return UV;
    }
    public static Color GetWorldColor(Vector2 UV)
    {
        return rasterizedMap.GetPixel((int)(UV.x * rasterizedMap.width), (int)(UV.y * rasterizedMap.height));
    }
    public static Color[] GetColorPalette()
    {
        if (rasterizedMap == null) { return null; }

        List<Color> palette = new List<Color>();

        Color[] pixels = rasterizedMap.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (!palette.Contains(pixels[i]))
            {
                palette.Add(pixels[i]);
            }
        }

        return palette.ToArray();
    }

    //ARMY & NAVY METHODS
    public static void AppendArmy(Army army)
    {
        army.ID = armies.Count;
        armies.Add(army);
    }
    public static Army GetArmyByID(int ID)
    {
        return armies[ID];
    }
    public static Army[] GetArmiesByTAG(string TAG)
    {
        List<Army> foundArmies = new List<Army>(armies.FindAll(a => a.TAG == TAG));
        return foundArmies.ToArray();
    }
    public static void AppendFleet(Fleet fleet)
    {
        fleet.ID = Fleets.Count;
        Fleets.Add(fleet);
    }
    public static Fleet GetFleetByID(int ID)
    {
        return Fleets[ID];
    }
    public static Fleet[] GetFleetsByTAG(string TAG)
    {
        List<Fleet> foundFleet = new List<Fleet>(Fleets.FindAll(f => f.TAG == TAG));
        return foundFleet.ToArray();
    }
}