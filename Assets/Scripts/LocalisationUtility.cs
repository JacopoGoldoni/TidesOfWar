using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

public static class LocalisationUtility
{
    static string localisationFolderPath = "Assets/Localisation/";

    public static string GetValue(string entry)
    {
        StreamReader sr = new StreamReader(localisationFolderPath + "country_L.txt");

        string file = sr.ReadToEnd();

        List<string> lines = file.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList<string>();

        Dictionary<string, string> country_dictionary = new Dictionary<string, string>();

        for (int i = 1; i < lines.Count; i++)
        {
            string line = lines[i];

            line = line.Remove(0, 2);

            line = line.Replace(':', '\0');

            Debug.Log(line.Split(' ')[0]);

            country_dictionary.Add(line.Split(' ')[0], line.Split(' ')[1]);
        }

        return country_dictionary[entry];
    }
}