using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    private WorldGenerator worldGenerator;
    private WorldUIManager worldUIManager;

    MapMode mapMode = MapMode.Countries;

    Graph<float, float> worldLogisticGraph;

    //TIME
    //DEFAULT DATE 1st Januray 1796
    DateTime gameDate = new DateTime(1796, 1, 1, 0, 0, 0);
    int lastMonth;
    public float MaxTimer = 4f;
    public float timer;
    float[] timeScales = { 1, 2, 4, 8};
    public int timeScaleIndex = 0;
    public bool pausedGame = false;

    private void Awake()
    {
        worldGenerator = GetComponent<WorldGenerator>();
        worldUIManager = FindFirstObjectByType<WorldUIManager>();

        timer = MaxTimer;

        Initialize_Load();
        worldUIManager.Initialize();

        worldGenerator.BuildWorld();

        //INITIALIZE ALL ARMIES
        ArmyManager[] armyManagers = FindObjectsByType<ArmyManager>(FindObjectsSortMode.None);
        foreach(ArmyManager am in armyManagers)
        {
            am.ArmyInitialize();
        }
    }
    private void Initialize_Load()
    {
        WorldUtility.LoadRasterizedMap();

        //LOAD WORLD ENTITIES
        WorldUtility.LoadProvinces();
        //WorldUtility.LoadRegions();
        WorldUtility.LoadCountries();

        //LOAD WORLD ELEMENTS
        WorldUtility.LoadBuildings();
    }

    public void SetMapMode(int mapMode)
    {
        this.mapMode = (MapMode)mapMode;
    }

    //LOGISTICS
    private void CalculateWorldNodesLogistic(int startingNode, float startingStrenght)
    {

    }
    private void CalculateWorldConnectionLogistic()
    {

    }

    //ARMY
    private void ApplyAttrition()
    {

    }
    private void ApplyReinforcements()
    {

    }

    //BUILDING
    public void AddBuilding(int buildingID, int targetProvince)
    {
        WorldUtility.GetProvinceByID(targetProvince).AddBuilding(buildingID, 1);
    }


    private void Update()
    {
        Update_HourTimer(Time.deltaTime);
        Update_Month();
    }
    private void Update_HourTimer(float deltaTime)
    {
        timer -= deltaTime * timeScales[timeScaleIndex] * Convert.ToInt32(!pausedGame);

        if(timer <= 0f)
        {
            gameDate = gameDate.AddHours(1d);

            worldUIManager.SetDateTime(gameDate);

            timer = MaxTimer;
        }
    }
    private void Update_Month()
    {
        if (lastMonth != gameDate.Month)
        {
            //MONTH CYCLE
            foreach(Country c in WorldUtility.GetAllCountries())
            {
                //c.MonthCalculations();
            }

            lastMonth = gameDate.Month;
        }
    }
}