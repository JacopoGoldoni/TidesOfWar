using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class GameUtility
{
    private static List<OfficerManager> regimentsRef;

    public static List<OfficerManager> GetAllRegiments()
    {
        if(regimentsRef == null || regimentsRef.Count == 0)
        {
            regimentsRef = (Object.FindObjectsByType<OfficerManager>(FindObjectsSortMode.None)).ToList<OfficerManager>();
        }

        return regimentsRef;
    }

    public static void RegisterRegiment(OfficerManager om)
    {
        regimentsRef.Add(om);
    }

    public static OfficerManager GetRegimentByID(int ID)
    {
        return regimentsRef[ID];
    }

    public static int GetIDOfRegiment(OfficerManager om)
    {
        return regimentsRef.IndexOf(om);
    }
}