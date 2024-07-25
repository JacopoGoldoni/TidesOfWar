using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class WorldUtility
{
    static Country[] countries;
    static string countryPath = "JSON/Countries/";
    public static void LoadCountries()
    {
        TextAsset[] countryAssets = Resources.LoadAll<TextAsset>(countryPath);

        if (countryAssets.Length == 0) { return; }

        countries = new Country[countryAssets.Length];

        Debug.Log(countryAssets.Length + " countries found.");

        for (int i = 0; i < countryAssets.Length; i++)
        {
            CountryInfo countryInfo = JsonUtility.FromJson<CountryInfo>(countryAssets[i].text);

            Country country = new Country(countryInfo.TAG);

            Debug.Log("Country added:\n" +
                "Name: " + countryInfo.name + "\n" +
                "TAG: " + countryInfo.TAG + "\n");

            countries[i] = country;
        }
    }

    public static Country GetCountryByTAG(string TAG)
    {
        foreach (Country c in countries)
        {
            if (c.TAG == TAG)
            {
                return c;
            }
        }
        return null;
    }
    public static Country[] GetAllCountries()
    {
        return countries.ToArray();
    }
    public static Country[] GetAllOtherCountries(string TAG)
    {
        Country c = GetCountryByTAG(TAG);
        List<Country> all = GetAllCountries().ToList();

        all.Remove(c);

        return all.ToArray();
    }
}
