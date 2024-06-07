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
    public bool active = true;
    public int width = 1000;
    public int lenght = 1000;
    public int height = 600;
    public Vector2Int chunks = new Vector2Int(1, 1);

    [Header("Texture settings")]
    public int chunkHeightmapResolution = 512;
    //public int detailResolution = 1024;
    //public int detailResolutionPerPatch = 8;
    //public int controlTextureResolution = 512;
    //public int baseTextureResolution = 1024;
    

    public float heightContrast = 0.5f;
    public float heightStrenght = 1f;

    private string savePath = "World/TerrainData/";

    public Texture2D[] worldHeightMaps;
    public Material worldMaterial;

    public void BuildWorld()
    {
        if(active)
        {
            BuildTerrain();

            NavMeshSurface navMeshSurface = GetComponent<NavMeshSurface>();

            navMeshSurface.BuildNavMesh();
        }
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
                GameObject chunkTerrain = Terrain.CreateTerrainGameObject(chunkTerrainData);

                chunkTerrain.name = name;
                chunkTerrain.layer = 8;
                chunkTerrain.transform.parent = WorldTerrain.transform;
                chunkTerrain.transform.position = new Vector3(chunkTerrainData.size.x * x, 0, chunkTerrainData.size.z * y);

                chunkTerrain.GetComponent<Terrain>().materialTemplate = worldMaterial;

                AssetDatabase.CreateAsset(chunkTerrainData, "Assets/" + savePath + name + ".asset");
            }
        }
    }
    private void PlaceCities()
    {
        for(int i = 0; i < WorldUtility.GetProvincesCount(); i++)
        {
            Province p = WorldUtility.GetProvinceByID(i);
            if(p.PROVINCE_TYPE == ProvinceType.city)
            {
                //TODO place city object
            }
        }
    }
    private void ValidatePath()
    {
        if (savePath == string.Empty) savePath = "TiledTerrain/TerrainData/";

        string pathToCheck = Application.dataPath + "/" + savePath;
        if (Directory.Exists(pathToCheck) == false)
        {
            Directory.CreateDirectory(pathToCheck);
        }
    }
}
