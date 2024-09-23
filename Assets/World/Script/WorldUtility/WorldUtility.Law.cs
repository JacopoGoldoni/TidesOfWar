using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class WorldUtility
{
    static Law[] laws;
    static string lawPath = "JSON/Laws/";


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

    public static Law[] GetAllLaws()
    {
        return laws;
    }
}
