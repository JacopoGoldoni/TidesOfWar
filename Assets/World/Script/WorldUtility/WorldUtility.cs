using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static partial class WorldUtility
{
    //DATA LISTS
    static Technology[] techs;

    //MAPS
    static Texture2D rasterizedMap;

    //PATHS
    static string rasterizedMapPath = "RasterizedMap_Europe";
    static string ideologyPath = "JSON/Ideologies/";

    //LOADERS
    public static void LoadRasterizedMap()
    {
        rasterizedMap = Resources.Load<Texture2D>(rasterizedMapPath);
        if(rasterizedMap != null)
        {
            Debug.Log("Rasterized map loaded");
        }
    }

    //GETTERS
        //TECHS
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

    //WORLD
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