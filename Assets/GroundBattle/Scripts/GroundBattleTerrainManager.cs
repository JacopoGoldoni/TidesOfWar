using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Analytics;

public class GroundBattleTerrainManager : MonoBehaviour
{
    [Header("World settings")]
    public bool active = true;
    public int resolution = 500;
    public int worldSize = 1000;
    public int height = 1;
    public float perlinNoiseStrenght = 1f;
    public float perlinNoiseSize = 1f;

    private string savePath = "GroundBattle/TerrainData/";

    public Material terrainMaterial;

    GameObject terrainObject;

    private void Start()
    {
        if(active)
        {
            BuildTerrain();
        }
    }

    private void BuildTerrain()
    {
        TerrainData terrainData = new TerrainData();

        string name = "Terrain" + 0 + "_" + 0;

        terrainData.baseMapResolution = worldSize + 1;
        terrainData.heightmapResolution = worldSize + 1;
        terrainData.size = new Vector3(worldSize, height, worldSize);

        //terrainData.baseMapResolution = resolution;
        //terrainData.heightmapResolution = resolution;
        //chunkTerrainData.alphamapResolution = controlTextureResolution;
        //chunkTerrainData.SetDetailResolution(detailResolution, detailResolutionPerPatch);

        //GENERATE HEIGHTMAP
        float[,] terrainHeights = new float[worldSize, worldSize];
        for (int i = 0; i < worldSize; i++)
        {
            for (int j = 0; j < worldSize; j++)
            {
                float xCoord = ((float)i / (float)worldSize) * perlinNoiseSize;
                float yCoord = ((float)j / (float)worldSize) * perlinNoiseSize;

                terrainHeights[i, j] = Mathf.PerlinNoise(xCoord, yCoord) * perlinNoiseStrenght;
            }
        }

        terrainData.SetHeights(0, 0, terrainHeights);

        terrainObject = Terrain.CreateTerrainGameObject(terrainData);

        terrainObject.name = name;
        terrainObject.layer = 8;
        terrainObject.transform.position = new Vector3(0, 0, 0);

        terrainObject.GetComponent<Terrain>().materialTemplate = terrainMaterial;

        AssetDatabase.CreateAsset(terrainData, "Assets/" + savePath + name + ".asset");
    }
}
