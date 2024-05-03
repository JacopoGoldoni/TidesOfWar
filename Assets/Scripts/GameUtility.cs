using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class GameUtility
{
    private static List<OfficerManager> regimentsRef;

    public static List<OfficerManager> FindAllRegiments()
    {
        if(regimentsRef == null || regimentsRef.Count == 0)
        {
            regimentsRef = (Object.FindObjectsByType<OfficerManager>(FindObjectsSortMode.None)).ToList<OfficerManager>();
        }

        return regimentsRef;
    }
}