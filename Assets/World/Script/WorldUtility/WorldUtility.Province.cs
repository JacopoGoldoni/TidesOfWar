using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class WorldUtility
{
    static Province[] provinces;
    static string provincePath = "JSON/Provinces/";
    public static void LoadProvinces()
    {
        TextAsset[] provinceAssets = Resources.LoadAll<TextAsset>(provincePath);

        if (provinceAssets.Length == 0) { return; }

        provinces = new Province[provinceAssets.Length];

        Debug.Log(provinceAssets.Length + " provinces found.");

        for (int i = 0; i < provinceAssets.Length; i++)
        {
            ProvinceInfo provinceInfo = JsonUtility.FromJson<ProvinceInfo>(provinceAssets[i].text);

            Province province = new Province(provinceInfo.name, provinceInfo.color);

            Debug.Log("Province added:\n" +
                "Name: " + provinceInfo.name + "\n" +
                "ID: " + provinceInfo.ID + "\n" +
                "ColorCode: " + provinceInfo.color);

            provinces[provinceInfo.ID] = province;
        }
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
        for (int i = 0; i < provinces.Length; i++)
        {
            if (provinces[i].colorCode == colorCode)
            {
                return provinces[i];
            }
        }
        return null;
    }
    public static Province[] GetProvincesInRegion(int ID)
    {
        Region r = GetRegionByID(ID);
        List<Province> p = new List<Province>();

        foreach (int provinceID in r.provincesID)
        {
            p.Add(GetProvinceByID(provinceID));
        }

        return provinces.ToArray();
    }
    public static int[] GetProvinceIndexesInRegion(int ID)
    {
        Region r = GetRegionByID(ID);
        return r.provincesID;
    }
    public static string GetProvinceOwner(int ID)
    {
        return GetProvinceByID(ID).OWNER_TAG;
    }
}
