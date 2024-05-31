using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    private WorldGenerator worldGenerator;

    private void Awake()
    {
        worldGenerator = GetComponent<WorldGenerator>();

        Initialize_Load();

        worldGenerator.BuildWorld();
    }

    private void Initialize_Load()
    {
        WorldUtility.LoadRasterizedMap();

        //LOAD WORLD ENTITIES
        //WorldUtility.LoadProvinces();
        //WorldUtility.LoadRegions();
        //WorldUtility.LoadCountries();

        //LOAD WORLD ELEMENTS
        //WorldUtility.LoadBuildings();
    }
}
