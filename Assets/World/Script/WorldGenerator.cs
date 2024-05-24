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
    public int chunkHeightmapResolution = 512;
    //public int detailResolution = 1024;
    //public int detailResolutionPerPatch = 8;
    //public int controlTextureResolution = 512;
    //public int baseTextureResolution = 1024;

    public int width = 1000;
    public int lenght = 1000;
    public int height = 600;
    public Vector2Int chunks = new Vector2Int(1, 1);

    public float heightContrast = 0.5f;
    public float heightStrenght = 1f;

    private string path = string.Empty;

    public Texture2D[] worldHeightMaps;
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

        //navMeshSurface.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void BuildTerrain()
    {
        GameObject WorldTerrain = new GameObject("World Terrain");
        WorldTerrain.transform.position = new Vector3(0, 0, 0);

        for (int x = 0; x < chunks.x; x++)
        {
            for (int y = 0; y < chunks.y; y++)
            {

                TerrainData chunkTerrainData = new TerrainData();

                string name = "Chunk_" + x + "_" + y;

                chunkTerrainData.size = new Vector3(width / chunks.x, height, lenght / chunks.y);

                chunkTerrainData.baseMapResolution = chunkHeightmapResolution;
                chunkTerrainData.heightmapResolution = chunkHeightmapResolution;
                //chunkTerrainData.alphamapResolution = controlTextureResolution;
                //chunkTerrainData.SetDetailResolution(detailResolution, detailResolutionPerPatch);

                //GET CHUNK HEIGHTMAP
                float[,] chunkHeights = new float[chunkHeightmapResolution + 1, chunkHeightmapResolution + 1];
                int chunkIndex = x + y * chunks.x;
                Texture2D chunkMap = worldHeightMaps[chunkIndex];
                for (int i = 0; i <= chunkHeightmapResolution; i++)
                {
                    for (int j = 0; j <= chunkHeightmapResolution; j++)
                    {
                        chunkHeights[j, i] = Mathf.Pow(chunkMap.GetPixel(i, j).grayscale, heightContrast) * heightStrenght;
                    }
                }

                chunkTerrainData.SetHeights(0, 0, chunkHeights);

                chunkTerrainData.name = name;
                GameObject chunkTerrain = (GameObject)Terrain.CreateTerrainGameObject(chunkTerrainData);

                chunkTerrain.name = name;
                chunkTerrain.transform.parent = WorldTerrain.transform;
                chunkTerrain.transform.position = new Vector3(chunkTerrainData.size.x * x, 0, chunkTerrainData.size.z * y);

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
