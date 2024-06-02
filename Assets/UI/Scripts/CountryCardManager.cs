using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CountryCardManager : MonoBehaviour
{
    Country cardCountry;

    public Image image;

    public void Initialize(string TAG, int scenarioID)
    {
        cardCountry = new Country(TAG);

        image.sprite = cardCountry.GetFlag();
    }
}
