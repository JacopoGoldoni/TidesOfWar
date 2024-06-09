using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [Header("River settings")]
    public int maxRiverNumber = 1;
    public float riverSize = 8f;
    private List<River> rivers;
    Texture2D riverTextureMap;

    GameObject terrainObject;

    private void Start()
    {
        if(active)
        {
            GenerateRivers();
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

                float riverValue = riverTextureMap.GetPixel(i, j).r;
                terrainHeights[i, j] = riverValue;
                //terrainHeights[i, j] = Mathf.PerlinNoise(xCoord, yCoord) * perlinNoiseStrenght;
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
    private void GenerateRivers()
    {
        //TODO: RANDOMIZE RIVER NUMBER
        rivers = new List<River>();
        riverTextureMap = new Texture2D(worldSize, worldSize);
        for(int i = 0; i < maxRiverNumber; i++)
        {
            River river = new River();
            //GENERATE RIVER
            
            //RANDOM START, END POSITIONS
            river.start = new Vector2(Random.Range(0, worldSize), Random.Range(0, worldSize));
            river.end = new Vector2(Random.Range(0, worldSize), Random.Range(0, worldSize));

            //RANDOM START, END TANGENTS
            river.start_tangent = new Vector2();
            river.end_tangent = new Vector2();

            //CHECK INTERSECTION
            //for(int j = 0; j < i; j++)
            //{

            //}

            rivers.Add(river);
        }

        //DRAW RIVER TEXTUREMAP
        for (int x = 0; x < riverTextureMap.width; x++)
        {
            for (int y = 0; y < riverTextureMap.height; y++)
            {
                Color c = new Color(1f, 1f, 1f);
                riverTextureMap.SetPixel(x, y, c);
            }
        }
        for (int x = 0; x < riverTextureMap.width; x++)
        {
            for (int y = 0; y < riverTextureMap.height; y++)
            {
                //SET PIXEL WITH NEAREST SPLINE DISTANCE
                for (int i = 0; i < maxRiverNumber; i++)
                {
                    float normalizedDistance = rivers[i].Distance(new Vector2(x, y)) / rivers[i].size;
                    Debug.Log(normalizedDistance);

                    if (normalizedDistance < 1)
                    {
                        Color c = new Color(normalizedDistance, normalizedDistance, normalizedDistance);
                        riverTextureMap.SetPixel(x, y, c);
                    }
                }
            }
        }
    }

    class River
    {
        public Vector2 start;
        public Vector2 end;
        public Vector2 start_tangent;
        public Vector2 end_tangent;
        public float size;

        public float Distance(Vector2 point)
        {
            //SIMPLE RIVER (LINE)
            Vector2 dir = (end - start).normalized;
            float a = Vector2.Dot(point - start, dir);

            float d = Mathf.Sqrt( (point - start).sqrMagnitude - a * a);

            return d;
        }
    }
}