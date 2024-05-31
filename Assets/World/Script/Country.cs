using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country
{
    public readonly string TAG = "FRA";
    public string[] subjects_TAG;

    public bool isSubject = false;
    public string overlord_TAG;

    public GovernmentType governmentType;

    public Country(string TAG, int scenarioID)
    {
        this.TAG = TAG;
        //TODO: GET ALL INFORMATIONS BY SCENARIO TEMPLATE
    }

    public Sprite GetFlag()
    {
        return GFXUtility.GetFlag(TAG);
    }
}