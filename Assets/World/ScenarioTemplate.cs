using System;
using System.Collections;
using System.Collections.Generic;

public class ScenarioTemplate
{
    public string name;
    public int year;
    public string description;

    public int scenarioID;

    public ScenarioTemplate(string name, int year, string description, int scenarioID)
    {
        this.name = name;
        this.year = year;
        this.description = description;
        this.scenarioID = scenarioID;
    }
}

public static class ScenraioList
{
    public static ScenarioTemplate[] scenarios =
    {
        new ScenarioTemplate("French revolution", 1797, "Description placeholder", 0),
    };
}