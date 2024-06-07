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

    private string savePath = "World/GroundBattle/TerrainData/";

    public Material terrainMaterial;

    GameObject terrain;

    private void Start()
    {
        BuildTerrain();
    }

    private void BuildTerrain()
    {
        TerrainData terrainData = new TerrainData();
        terrainData.size = new Vector3(worldSize, height, worldSize);

        terrainData.baseMapResolution = resolution;
        terrainData.heightmapResolution = resolution;
        //chunkTerrainData.alphamapResolution = controlTextureResolution;
        //chunkTerrainData.SetDetailResolution(detailResolution, detailResolutionPerPatch);

        //GENERATE HEIGHTMAP
        float[,] terrainHeights = new float[resolution + 1, resolution + 1];
        for (int i = 0; i <= resolution; i++)
        {
            for (int j = 0; j <= resolution; j++)
            {
                float xCoord = i / worldSize * perlinNoiseSize;
                float yCoord = j / worldSize * perlinNoiseSize;

                terrainHeights[i, j] = Mathf.PerlinNoise(xCoord, yCoord) * perlinNoiseStrenght;
            }
        }

        terrainData.SetHeights(0, 0, terrainHeights);

        terrain = Terrain.CreateTerrainGameObject(terrainData);

        terrain.layer = 8;
        terrain.transform.position = new Vector3(0, 0, 0);

        terrain.GetComponent<Terrain>().materialTemplate = terrainMaterial;

        AssetDatabase.CreateAsset(terrainData, "Assets/" + savePath + name + ".asset");
    }
}
