using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class WorldUtility
{
    static Building[] buildings;
    static string buildingPath = "JSON/Buildings/";

    public static void LoadBuildings()
    {
        TextAsset[] buildingAssets = Resources.LoadAll<TextAsset>(buildingPath);

        if (buildingAssets.Length == 0) { return; }

        buildings = new Building[buildingAssets.Length];

        Debug.Log(buildingAssets.Length + " buildings found.");

        for (int i = 0; i < buildingAssets.Length; i++)
        {
            BuildingInfo buildingInfo = JsonUtility.FromJson<BuildingInfo>(buildingAssets[i].text);

            Building building = new Building(buildingInfo.name, buildingInfo.icon, buildingInfo.description, buildingInfo.maxDurability);

            Debug.Log("Building added:\n" +
                "Name: " + buildingInfo.name + "\n" +
                "Icon: " + buildingInfo.icon + "\n" +
                "Description: " + buildingInfo.description + "\n" +
                "Max durability: " + buildingInfo.maxDurability);

            buildings[buildingInfo.id] = building;
        }
    }

    public static int GetBuildingsCount()
    {
        return buildings.Length;
    }
    public static Building GetBuildingByID(int ID)
    {
        return buildings[ID];
    }
}