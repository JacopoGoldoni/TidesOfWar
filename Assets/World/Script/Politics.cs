using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party
{
    public string name;
    public int[] ideologies;

    public float popularity;
}

public class Ideology
{
    string name;
}

public enum GovernmentType
{
    Monarchy,
    Imperial,
    Republican
}