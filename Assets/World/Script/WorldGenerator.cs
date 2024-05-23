using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class WorldGenerator : MonoBehaviour
{
    [Header("World settings")]
    public int heightmapResolution = 512;
    public int detailResolution = 1024;
    public int detailResolutionPerPatch = 8;
    public int controlTextureResolution = 512;
    public int baseTextureResolution = 1024;
    public Vector2 tileAmount = Vector2.one;

    public float width = 1000;
    public float lenght = 1000;
    public float height = 600;

    public float heightContrast = 0.5f;
    public float heightStrenght = 1f;

    private string path = string.Empty;

    public Texture2D worldHeightMap;
    public Material worldMaterial;

    private NavMeshSurface navMeshSurface;

    private void Awake()
    {
        navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
        navMeshSurface.agentTypeID = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        BuildTerrain();

        navMeshSurface.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void BuildTerrain()
    {
        GameObject WorldTerrain = new GameObject("World Terrain");
        WorldTerrain.transform.position = new Vector3(0, 0, 0);

        for (int x = 1; x <= tileAmount.x; x++)
        {
            for (int y = 1; y <= tileAmount.y; y++)
            {
                TerrainData chunkTerrainData = new TerrainData();

                string name = "Chunk_" + x + "-" + y;

                chunkTerrainData.size = new Vector3(width, height, lenght);

                chunkTerrainData.baseMapResolution = baseTextureResolution;
                chunkTerrainData.heightmapResolution = heightmapResolution;
                chunkTerrainData.alphamapResolution = controlTextureResolution;
                chunkTerrainData.SetDetailResolution(detailResolution, detailResolutionPerPatch);

                //NO CHUNCK SLICING YET
                float[,] chunksHeights = new float[heightmapResolution, heightmapResolution];
                for(int i = 0; i < heightmapResolution; i++)
                {
                    for (int j = 0; j < heightmapResolution; j++)
                    {
                        if(i >= worldHeightMap.width || j >= worldHeightMap.height)
                        {
                            continue;
                        }
                        chunksHeights[j, i] = Mathf.Pow(worldHeightMap.GetPixel(i * 2, j).grayscale, heightContrast) * heightStrenght;
                    }
                }

                chunkTerrainData.SetHeights(0, 0, chunksHeights);

                chunkTerrainData.name = name;
                GameObject chunkTerrain = (GameObject)Terrain.CreateTerrainGameObject(chunkTerrainData);

                chunkTerrain.name = name;
                chunkTerrain.transform.parent = WorldTerrain.transform;
                chunkTerrain.transform.position = new Vector3(lenght * (x - 1), 0, width * (y - 1));

                chunkTerrain.GetComponent<Terrain>().materialTemplate = worldMaterial;

                AssetDatabase.CreateAsset(chunkTerrainData, "Assets/" + path + name + ".asset");
            }
        }
    }
    private void ValidatePath()
    {
        if (path == string.Empty) path = "TiledTerrain/TerrainData/";

        string pathToCheck = Application.dataPath + "/" + path;
        if (Directory.Exists(pathToCheck) == false)
        {
            Directory.CreateDirectory(pathToCheck);
        }
    }
}
