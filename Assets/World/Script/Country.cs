using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country
{
    public readonly string TAG = "FRA";
    public string[] subjects_TAG;

    public bool isSubject = false;
    public string overlord_TAG;

    public Country() { }
    public Country(string TAG)
    {
        this.TAG = TAG;
    }

    public Sprite GetFlag()
    {
        return GFXUtility.GetFlag(TAG);
    }
}