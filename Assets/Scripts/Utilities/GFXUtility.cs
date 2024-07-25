using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GFXUtility
{
    private static Dictionary<Factions, string> FactionCodes = new Dictionary<Factions, string>()
    {
        { Factions.France, "FRA" },
        { Factions.Austria, "AUS" },
        { Factions.England, "ENG" },
        { Factions.Russia, "RUS" },
        { Factions.Prussia, "PRU" },
        //ADD SPAIN
        //ADD PIEDMONT
        //ADD HOLAND
        //ADD SWEDEN
        //ADD POLAND
        //ADD PORTUGAL
    };

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
    public static string GetFlagCode(Factions faction)
    {
        return FactionCodes[faction];
    }
    public static Sprite GetFlag(Factions faction)
    {
        string flagLocation = "GFX/Flags/" + GetFlagCode(faction);

        Sprite sprite = Resources.Load<Sprite>(flagLocation);
        return sprite;
    }
    public static Sprite GetFlag(string TAG)
    {
        string flagLocation = "GFX/" + TAG;

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