using Den.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WorldUtility
{
    //DATA LISTS
    static Province[] provinces;
    static List<Region> regions = new List<Region>();
    static List<Country> countries = new List<Country>();
    static List<Building> buildings = new List<Building>();

    //MAPS
    static Texture2D rasterizedMap;

    //PATHS
    static string rasterizedMapPath = "Assets/Resources/RasterizedMap_Europe.tif";
    static string provincePath = "/JSON/Provinces/";
    static string regionPath = "/JSON/Regions/";
    static string countryPath = "/JSON/Countries/";
    static string buildingPath = "/JSON/Buildings/palette.Contains()";

    //LOADERS
    public static void LoadRasterizedMap()
    {
        rasterizedMap = Resources.Load<Texture2D>("RasterizedMap_Europe");
        if(rasterizedMap != null)
        {
            Debug.Log("Rasterized map loaded");
        }
    }
    public static void LoadProvinces()
    {
        TextAsset[] provinceAssets = Resources.LoadAll<TextAsset>(Application.dataPath + provincePath);
        
        provinces = new Province[provinceAssets.Length];

        for(int i = 0; i < provinceAssets.Length; i++)
        {
            ProvinceInfo provinceInfo = JsonUtility.FromJson<ProvinceInfo>(provinceAssets[i].text);

            Province province = new Province(provinceInfo.color);
            
            provinces[i] = province;
        }
    }
    public static void LoadRegions()
    {
        TextAsset[] regionAssets = Resources.LoadAll<TextAsset>(Application.dataPath + regionPath);

        for (int i = 0; i < regionAssets.Length; i++)
        {
            Region region = JsonUtility.FromJson<Region>(regionAssets[i].text);
            regions.Add(region);
        }
    }
    public static void LoadCountries()
    {
        TextAsset[] countryAssets = Resources.LoadAll<TextAsset>(Application.dataPath + countryPath);

        for (int i = 0; i < countryAssets.Length; i++)
        {
            Country country = JsonUtility.FromJson<Country>(countryAssets[i].text);
            countries.Add(country);
        }
    }
    public static void LoadBuildings()
    {
        TextAsset[] buildingsAssets = Resources.LoadAll<TextAsset>(Application.dataPath + buildingPath);

        for (int i = 0; i < buildingsAssets.Length; i++)
        {
            Building building = JsonUtility.FromJson<Building>(buildingsAssets[i].text);
            buildings.Add(building);
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
        foreach (Region r in regions)
        {
            if (r.regionID == ID)
            {
                return r;
            }
        }
        return null;
    }
    public static Province GetProvinceByID(int ID)
    {
        return provinces[ID];
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
}