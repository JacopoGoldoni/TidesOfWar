using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class WorldUtility
{
    static Resource[] resources;
    static string resourcePath = "JSON/Resources/";

    public static void LoadResources()
    {
        TextAsset[] resourceAssets = Resources.LoadAll<TextAsset>(resourcePath);

        if (resourceAssets.Length == 0) { return; }

        resources = new Resource[resourceAssets.Length];

        Debug.Log(resourceAssets.Length + " resources found.");

        for (int i = 0; i < resourceAssets.Length; i++)
        {
            ResourceInfo resourceInfo = JsonUtility.FromJson<ResourceInfo>(resourceAssets[i].text);

            Resource resource = new Resource();
            resource.resourceName = resourceInfo.name;
            resource.resourceIcon = resourceInfo.icon;

            Debug.Log("Resource added:\n" +
                "Name: " + resourceInfo.name + "\n" +
                "Icon: " + resourceInfo.icon);

            resources[i] = resource;
        }
    }

    public static Resource[] GetAllResources()
    {
        return resources;
    }
    public static Resource GetResourceByID(int ID)
    {
        foreach (Resource r in resources)
        {
            if (r.ID == ID)
            {
                return r;
            }
        }
        return null;
    }
}