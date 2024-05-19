using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country
{
    string TAG = "FRA";

    public Sprite GetFlag()
    {
        return GFXUtility.GetFlag(TAG);
    }
}