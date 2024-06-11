using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Analytics;

public class GroundBattleTerrainManager : MonoBehaviour
{
    Erosion erosion;
    MiniTerrainDecorator terrainDecorator;

    public bool active = true;

    [Header("World settings")]
    public int resolution = 500;
    public int grassDensity;
    public int detailResolutionPerPatch;
    public int worldSize = 1000;
    public int height = 1;
    public float perlinNoiseStrenght = 1f;
    public float perlinNoiseSize = 1f;
    private string savePath = "GroundBattle/TerrainData/";
    public Material terrainMaterial;

    [Header("Layers")]
    public List<TerrainLayer> terrainLayers;

    [Header("Erosion settings")]
    public int erosionIterations = 1;

    [Header("River settings")]
    public int maxRiverNumber = 1;
    public float riverSize = 8f;
    public static int riverResolution = 10;
    public Material riverMaterial;
    private List<River> rivers;
    Texture2D riverTextureMap;

    [Header("Road settings")]
    public float maxRoadSize;
    public float minRoadSize;
    private List<Road> roads;
    Texture2D roadTextureMap;

    [Header("Town settings")]
    private List<Town> towns;

    [Header("Tree settings")]
    public List<GameObject> treePrefabs;
    public float treeDensity;
    public float treeScale;

    GameObject terrainObject;

    private void OnEnable()
    {
        erosion = GetComponent<Erosion>();
    }
    private void Start()
    {
        if(active)
        {
            GenerateRivers();
            BuildTerrain();
        }
    }

    //BUILDERS
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
        terrainData.SetDetailResolution(grassDensity, detailResolutionPerPatch);

        terrainData.terrainLayers = terrainLayers.ToArray();

        //GENERATE HEIGHTMAP
        float[,] terrainHeights = new float[worldSize, worldSize];
        for (int i = 0; i < worldSize; i++)
        {
            for (int j = 0; j < worldSize; j++)
            {
                terrainHeights[i, j] = GenerateHeight(i, j);
            }
        }

        float[] heightmap = new float[worldSize * worldSize];
        for(int i = 0; i < worldSize; i++)
        {
            for (int j = 0; j < worldSize; j++)
            {
                heightmap[i + j * worldSize] = terrainHeights[i,j];
            }
        }
        erosion.Erode(heightmap, worldSize, erosionIterations, false);
        for (int i = 0; i < worldSize; i++)
        {
            for (int j = 0; j < worldSize; j++)
            {
                terrainHeights[i, j] = heightmap[i + j * worldSize];
            }
        }

        for(int i = 0; i < worldSize; i++)
        {
            for(int j = 0; j < worldSize; j++)
            {
                terrainHeights[i, j] -= 0.5f * (1 - riverTextureMap.GetPixel(i, j).r);
            }
        }

        //GENERATE RIVERS
        for (int i = 0; i < rivers.Count; i++)
        {
            rivers[i].GenerateMesh();

            GameObject river = new GameObject();
            river.name = "River_" + i;

            MeshFilter mf = river.AddComponent<MeshFilter>();
            MeshRenderer mr = river.AddComponent<MeshRenderer>();

            mf.mesh = rivers[i].mesh;
            mr.material = riverMaterial;
        }

        //TREE
        List<TreePrototype> treePrototypes = new List<TreePrototype>();
        foreach (GameObject treeGO in treePrefabs)
        {
            TreePrototype treeProrotype = new TreePrototype();
            treeProrotype.prefab = treeGO;
            treePrototypes.Add(treeProrotype);
        }
        terrainData.treePrototypes = treePrototypes.ToArray();

        terrainData.SetHeights(0, 0, terrainHeights);

        terrainObject = Terrain.CreateTerrainGameObject(terrainData);

        terrainObject.name = name;
        terrainObject.layer = 8;
        terrainObject.transform.position = new Vector3(0, 0, 0);

        Terrain terrain = terrainObject.GetComponent<Terrain>();
        terrain.materialTemplate = terrainMaterial;

        AssetDatabase.CreateAsset(terrainData, "Assets/" + savePath + name + ".asset");

        Decorate();
    }

    private void Decorate()
    {
        terrainDecorator = terrainObject.AddComponent<MiniTerrainDecorator>();

        terrainDecorator.GetTerrain();

        terrainDecorator.layers = new List<MiniTerrainDecorator.Layers>();

        //LAYERS
            //GRASS LAYER
        MiniTerrainDecorator.Layers grassLayer = new MiniTerrainDecorator.Layers();
        grassLayer.active = true;
        grassLayer.name = "grass";
        grassLayer.layerIndex = 0;
        grassLayer.rules = new List<MiniTerrainDecorator.Rules>();
        
        terrainDecorator.layers.Add(grassLayer);
        
            //ROCK LAYER
        MiniTerrainDecorator.Layers rockLayer = new MiniTerrainDecorator.Layers();
        rockLayer.active = true;
        rockLayer.name = "rock";
        rockLayer.layerIndex = 1;

        rockLayer.rules = new List<MiniTerrainDecorator.Rules>();

        MiniTerrainDecorator.Rules rockRule = new MiniTerrainDecorator.Rules();
        rockRule.active = true;
        rockRule.blend = MiniTerrainDecorator.BlendType.add;
        rockRule.filter = MiniTerrainDecorator.FilterType.slope;
        rockRule.min = 10;
        rockRule.max = 90;
        rockLayer.rules.Add(rockRule);
        
        terrainDecorator.layers.Add(rockLayer);

        //TREE LAYER
        for(int i = 0; i < treePrefabs.Count; i++)
        {
            terrainDecorator.layers.Add(GenerateTree("tree", i + 2, 0, 10, 2));
        }

        terrainDecorator.Decorate();
    }

    private void GenerateRivers()
    {
        //TODO: RANDOMIZE RIVER NUMBER
        rivers = new List<River>();
        riverTextureMap = new Texture2D(worldSize, worldSize);
        for(int i = 0; i < maxRiverNumber; i++)
        {
            River river = new River();
            river.size = riverSize;
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
                float distance = Mathf.Infinity;
                int index = 0;
                //SET PIXEL WITH NEAREST SPLINE DISTANCE
                for (int i = 0; i < maxRiverNumber; i++)
                {
                    float d = rivers[i].Distance(new Vector2(x, y));
                    if(d < distance)
                    {
                        distance = d;
                        index = i;
                    }
                }
                float normalizedDistance = distance / rivers[index].size;

                if (normalizedDistance < 1)
                {
                    float s = UtilityMath.SmoothFunction(normalizedDistance);
                    Color c = new Color(s, s, s);
                    riverTextureMap.SetPixel(x, y, c);
                }
            }
        }
    }
    private void GenerateRoads()
    {
        //TODO: RANDOMIZE RIVER NUMBER
        roads = new List<Road>();
        roadTextureMap = new Texture2D(worldSize, worldSize);
        for (int i = 0; i < maxRiverNumber; i++)
        {
            Road road = new Road();
            road.size = maxRoadSize;
            //GENERATE RIVER

            //RANDOM START, END POSITIONS
            road.start = new Vector2(Random.Range(0, worldSize), Random.Range(0, worldSize));
            road.end = new Vector2(Random.Range(0, worldSize), Random.Range(0, worldSize));

            //RANDOM START, END TANGENTS
            road.start_tangent = new Vector2();
            road.end_tangent = new Vector2();

            //CHECK INTERSECTION
            for(int j = 0; j < i; j++)
            {

            }

            roads.Add(road);
        }

        //DRAW ROAD TEXTUREMAP
        for (int x = 0; x < roadTextureMap.width; x++)
        {
            for (int y = 0; y < roadTextureMap.height; y++)
            {
                float distance = Mathf.Infinity;
                int index = 0;
                //SET PIXEL WITH NEAREST SPLINE DISTANCE
                for (int i = 0; i < maxRiverNumber; i++)
                {
                    float d = rivers[i].Distance(new Vector2(x, y));
                    if (d < distance)
                    {
                        distance = d;
                        index = i;
                    }
                }
                float normalizedDistance = distance / roads[index].size;

                if (normalizedDistance < 1)
                {
                    Color c = new Color(1, 1, 1);
                    roadTextureMap.SetPixel(x, y, c);
                }
            }
        }
    }
    private void GenerateBridges()
    {

    }
    private MiniTerrainDecorator.Layers GenerateTree
        (string name, int layerIndex, int targetlayerIndex, float frequency, float lacunarity)
    {
        MiniTerrainDecorator.Layers treeLayer = new MiniTerrainDecorator.Layers();
        treeLayer.active = true;
        treeLayer.name = name;
        treeLayer.layerType = MiniTerrainDecorator.LayerType.tree;
        treeLayer.layerIndex = layerIndex;
        treeLayer.maximumTreeCount = (int)(treeDensity * worldSize * worldSize);
        treeLayer.probability = 1;
        treeLayer.width = treeScale;
        treeLayer.height = treeScale;
        treeLayer.randomPosition = 1;
        treeLayer.randomRotation = 1;
        treeLayer.randomSize = 0.1f;

        MiniTerrainDecorator.Rules treeRule1 = new MiniTerrainDecorator.Rules();
        treeRule1.active = true;
        treeRule1.blend = MiniTerrainDecorator.BlendType.add;
        treeRule1.filter = MiniTerrainDecorator.FilterType.layer;
        treeRule1.targetLayerIndex = targetlayerIndex;
        treeLayer.rules.Add(treeRule1);

        MiniTerrainDecorator.Rules treeRule2 = new MiniTerrainDecorator.Rules();
        treeRule2.active = true;
        treeRule2.blend = MiniTerrainDecorator.BlendType.mul;
        treeRule2.filter = MiniTerrainDecorator.FilterType.noise;
        treeRule2.intensity = 1;
        treeRule2.contrast = 10;
        treeRule2.frequency = frequency;
        treeRule2.lacunarity = lacunarity;
        treeLayer.rules.Add(treeRule2);
        
        return treeLayer;
    }

    private float GenerateHeight(int x, int y)
    {
        float xCoord_Local = ((float)x / (float)worldSize) * perlinNoiseSize;
        float yCoord_Local = ((float)y / (float)worldSize) * perlinNoiseSize;

        float height;

        height = Mathf.PerlinNoise(xCoord_Local, yCoord_Local) * perlinNoiseStrenght;

        height = Mathf.Clamp01(height);

        return height;
    }

    //ELEMENTS
    class River
    {
        public Vector2 start;
        public Vector2 end;
        public Vector2 start_tangent;
        public Vector2 end_tangent;
        public float size;
        public Mesh mesh;

        public float Distance(Vector2 point)
        {
            //SIMPLE RIVER (LINE)
            Vector2 dir = (end - start).normalized;
            float a = Vector2.Dot(point - start, dir);

            float d = Mathf.Sqrt( (point - start).sqrMagnitude - a * a);

            return d;
        }
        public Vector2 PointAt(float t)
        {
            return Vector2.Lerp(start, end, t);
        }
        public Vector2 GetTangentAt(float t)
        {
            return (end - start).normalized;
        }
        public Vector2 GetNormalAt(float t)
        {
            Matrix tangentColumn = Matrix.V2ToMatrix( GetTangentAt(t) );

            Vector2 n = Matrix.MatrixToVector2(Matrix.Rotation2D() * tangentColumn);

            return n;
        }
        public float GetLenght()
        {
            return (end - start).magnitude;
        }
        public void GenerateMesh()
        {
            mesh = new Mesh();
            mesh.name = "RiverMesh";

            float delta = 1f / (float)riverResolution;

            //RIVER VERTICES
            mesh.vertices = new Vector3[(riverResolution + 1) * 2];
            for(int i = 0; i <= riverResolution; i++)
            {
                float t = delta * i;
                Vector2 pos = PointAt(t);
                Vector2 nor = GetNormalAt(t);

                Vector2 vert1 = pos + nor * (size / 2f);
                Vector2 vert2 = pos - nor * (size / 2f);
                mesh.vertices[i * 2] = Utility.V2toV3(vert1);
                mesh.vertices[i * 2 + 1] = Utility.V2toV3(vert2);
            }
            //RIVER FACES
            mesh.triangles = new int[6 * (riverResolution + 1)];
            for (int i = 0; i <= riverResolution; i++)
            {
                mesh.triangles[i * 6] = i * 2;
                mesh.triangles[i * 6 + 1] = i * 2 + 1;
                mesh.triangles[i * 6 + 2] = i * 2 + 2;

                mesh.triangles[i * 6 + 3] = i * 2 + 1;
                mesh.triangles[i * 6 + 4] = i * 2 + 3;
                mesh.triangles[i * 6 + 5] = i * 2 + 2;
            }
        }
    }
    class Road
    {
        public Vector2 start;
        public Vector2 end;
        public Vector2 start_tangent;
        public Vector2 end_tangent;
        public float size;
        public Mesh mesh;

        public static bool Intersect(Road r)
        {

            return false;
        }

        public float Distance(Vector2 point)
        {
            //SIMPLE RIVER (LINE)
            Vector2 dir = (end - start).normalized;
            float a = Vector2.Dot(point - start, dir);

            float d = Mathf.Sqrt((point - start).sqrMagnitude - a * a);

            return d;
        }
        public Vector2 PointAt(float t)
        {
            return Vector2.Lerp(start, end, t);
        }
        public Vector2 GetTangentAt(float t)
        {
            return (end - start).normalized;
        }
        public Vector2 GetNormalAt(float t)
        {
            Matrix tangentColumn = Matrix.V2ToMatrix(GetTangentAt(t));

            Vector2 n = Matrix.MatrixToVector2(Matrix.Rotation2D() * tangentColumn);

            return n;
        }
        public float GetLenght()
        {
            return (end - start).magnitude;
        }
        public void GenerateMesh()
        {
            mesh = new Mesh();
            mesh.name = "RoadMesh";

            float delta = 1f / (float)riverResolution;

            //RIVER VERTICES
            mesh.vertices = new Vector3[(riverResolution + 1) * 2];
            for (int i = 0; i <= riverResolution; i++)
            {
                float t = delta * i;
                Vector2 pos = PointAt(t);
                Vector2 nor = GetNormalAt(t);

                Vector2 vert1 = pos + nor * (size / 2f);
                Vector2 vert2 = pos - nor * (size / 2f);
                mesh.vertices[i * 2] = Utility.V2toV3(vert1);
                mesh.vertices[i * 2 + 1] = Utility.V2toV3(vert2);
            }
            //RIVER FACES
            mesh.triangles = new int[6 * (riverResolution + 1)];
            for (int i = 0; i <= riverResolution; i++)
            {
                mesh.triangles[i * 6] = i * 2;
                mesh.triangles[i * 6 + 1] = i * 2 + 1;
                mesh.triangles[i * 6 + 2] = i * 2 + 2;

                mesh.triangles[i * 6 + 3] = i * 2 + 1;
                mesh.triangles[i * 6 + 4] = i * 2 + 3;
                mesh.triangles[i * 6 + 5] = i * 2 + 2;
            }
        }
    }
    class Town
    {
        Vector2 center;
        float maxRadius;
    }
}