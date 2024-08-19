using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GFXUtility
{
    //GENERAL
    public static Sprite GetSprite(string path)
    {
        return Resources.Load<Sprite>("GFX/" + path);
    }
    public static Sprite[] GetSpriteSheet(string path)
    {
        return Resources.LoadAll("GFX/" + path).OfType<Sprite>().ToArray();
    }

    //FLAG
    public static Sprite GetFlag(string TAG)
    {
        string flagLocation = "GFX/Flags/" + TAG;

        Sprite sprite = Resources.Load<Sprite>(flagLocation);
        return sprite;
    }

    //BUILDING
    public static Sprite GetBuildingSprite(string path)
    {
        return Resources.Load<Sprite>("GFX/Buildings/" + path);
    }

    //UNIT
    public static Sprite GetUnitSprite(string path)
    {
        return Resources.Load<Sprite>("GFX/UnitIcons/" + path);
    }
}