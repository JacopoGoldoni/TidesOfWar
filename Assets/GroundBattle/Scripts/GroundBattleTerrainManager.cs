using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;

public class GroundBattleTerrainManager : MonoBehaviour
{
    Erosion erosion;
    MiniTerrainDecorator terrainDecorator;
    NavMeshSurface navMeshSurface;

    [Header("Activated features")]
    public bool active = true;
    public bool riverActive = false;
    public bool decorateActive = false;
    public bool treeActive = false;

    [Header("World settings")]
    public int resolution = 500;
    public int grassDensity;
    public int detailResolutionPerPatch;
    public int worldSize = 1000;
    public int height = 1;
    public float perlinNoiseStrenght = 1f;
    public float perlinNoiseScale = 1f;
    public int octaves = 2;
    public float lacunarity = 2f;
    public float persistence = 0.5f;
    private string savePath = "GroundBattle/TerrainData/";
    public Material terrainMaterial;

    [Header("Layers")]
    public Texture2D terrainHeightmap;
    public List<TerrainLayer> terrainLayers;

    [Header("Erosion settings")]
    public bool erode = false;
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
    private List<RoadNode> roadNodes;
    Texture2D roadTextureMap;

    [Header("Town settings")]
    private List<Town> towns;
    public int maxTownCount;
    public float minTownRadius;
    public float maxTownRadius;

    [Header("Tree settings")]
    public List<TreeProfile> treeProfiles;

    GameObject terrainObject;

    private void OnEnable()
    {
        erosion = GetComponent<Erosion>();
    }
    public void CallGeneration()
    {
        if(active)
        {
            roadNodes = new List<RoadNode>();

            if (riverActive)
                GenerateRivers();

            BuildTerrain();

            if(decorateActive)
                Decorate();

            //GenerateTowns();
            //GenerateRoads();
            //GenerateBridges();

            GenerateNavMeshSurface();
        }
    }

    //BUILDERS
    private void BuildTerrain()
    {
        TerrainData terrainData = new TerrainData();

        string name = "Terrain" + 0 + "_" + 0;

        terrainData.baseMapResolution = resolution + 1;
        terrainData.heightmapResolution = resolution + 1;
        terrainData.size = new Vector3(worldSize, height, worldSize);

        //terrainData.baseMapResolution = resolution;
        //chunkTerrainData.alphamapResolution = controlTextureResolution;
        terrainData.SetDetailResolution(grassDensity, detailResolutionPerPatch);

        terrainData.terrainLayers = terrainLayers.ToArray();

        //GENERATE HEIGHTMAP
        Texture2D terrainHeightMap = TextureMath.AdvancedPerlinTexture
            (worldSize, perlinNoiseScale, octaves, lacunarity, persistence);

        if (erode)
        {
            TextureMath.Erode(terrainHeightMap, erosion, erosionIterations, 0);
        }

        if(riverActive)
        {
            terrainHeightMap = TextureMath.Subtraction(
                    terrainHeightMap,
                    TextureMath.Multiplication(riverTextureMap, 0.5f)
                    );

            //GENERATE RIVER MESHES
            for (int i = 0; i < rivers.Count; i++)
            {
                rivers[i].GenerateMesh();

                GameObject river = new GameObject();
                river.name = "River_" + i;
                river.transform.position = Utility.V2toV3((rivers[i].start + rivers[i].end) / 2f);

                MeshFilter mf = river.AddComponent<MeshFilter>();
                MeshRenderer mr = river.AddComponent<MeshRenderer>();

                mf.mesh = rivers[i].mesh;
                mr.material = riverMaterial;
            }
        }

        if(treeActive)
        {
            //TREE
            List<TreePrototype> treePrototypes = new List<TreePrototype>();
            foreach (TreeProfile treeProfile in treeProfiles)
            {
                TreePrototype treeProrotype = new TreePrototype();
                treeProrotype.prefab = treeProfile.treePrefab;
                treePrototypes.Add(treeProrotype);
            }
            terrainData.treePrototypes = treePrototypes.ToArray();
        }

        terrainData.SetHeights(0, 0, TextureMath.Extract(terrainHeightMap, 0));

        terrainHeightmap = terrainHeightMap;
        terrainObject = Terrain.CreateTerrainGameObject(terrainData);

        terrainObject.name = name;
        terrainObject.layer = 8;
        terrainObject.transform.position = new Vector3(0, 0, 0);

        Terrain terrain = terrainObject.GetComponent<Terrain>();
        terrain.materialTemplate = terrainMaterial;

        AssetDatabase.CreateAsset(terrainData, "Assets/" + savePath + name + ".asset");
        GroundBattleUtility.terrainRef = terrain;
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

        MiniTerrainDecorator.Rules rockRule1 = new MiniTerrainDecorator.Rules();
        rockRule1.active = true;
        rockRule1.blend = MiniTerrainDecorator.BlendType.add;
        rockRule1.filter = MiniTerrainDecorator.FilterType.slope;
        rockRule1.min = 10;
        rockRule1.max = 90;
        rockLayer.rules.Add(rockRule1);
        MiniTerrainDecorator.Rules rockRule2 = new MiniTerrainDecorator.Rules();
        rockRule2.active = true;
        rockRule2.blend = MiniTerrainDecorator.BlendType.add;
        rockRule2.filter = MiniTerrainDecorator.FilterType.texture;
        rockRule2.texture = riverTextureMap;
        rockLayer.rules.Add(rockRule2);

        terrainDecorator.layers.Add(rockLayer);

        //TREE LAYER
        for(int i = 0; i < treeProfiles.Count; i++)
        {
            terrainDecorator.layers.Add(GenerateTree(treeProfiles[i], i + 2, 0, 10, 2));
        }

        terrainDecorator.Decorate();
    }

    private void GenerateRivers()
    {
        if(maxRiverNumber == 0)
        {
            return;
        }

        //TODO: RANDOMIZE RIVER NUMBER
        rivers = new List<River>();
        riverTextureMap = new Texture2D(worldSize, worldSize);

        int n = Random.Range(0, maxRiverNumber);
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
                Color c = new Color(0f, 0f, 0f);
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
                    float s = 1f - UtilityMath.SmoothFunction(normalizedDistance);
                    Color c = new Color(s, s, s);
                    riverTextureMap.SetPixel(x, y, c);
                }
            }
        }
    }
    private void GenerateRoads()
    {
        roads = new List<Road>();
        roadTextureMap = new Texture2D(worldSize, worldSize);

        int i = 0;
        while (!AreAllRoadNodeConnected())
        {
            Road road = new Road();
            road.size = maxRoadSize;
            //GENERATE ROAD

            //GET NEAREST NODE
            int l = 0;
            float distance = Mathf.Infinity;
            for(int j = 0; j < roadNodes.Count; j++)
            {
                if (j == i) { continue; }
                
                float d = Vector2.Distance(roadNodes[i].position, roadNodes[j].position);
                if(d < distance)
                {
                    l = j;
                    distance = d;
                }
            }

            //MAKE ROAD
            road.start = roadNodes[i].position;
            road.end = roadNodes[l].position;

            //START, END TANGENTS
            if (Vector2.Dot(roadNodes[i].tangent, roadNodes[l].position - roadNodes[i].position) > 0)
            {
                road.start_tangent = roadNodes[i].tangent;
            }
            else
            {
                road.start_tangent = -1 * roadNodes[i].tangent;
            }
            if (Vector2.Dot(roadNodes[l].tangent, roadNodes[i].position - roadNodes[l].position) > 0)
            {
                road.end_tangent = roadNodes[l].tangent;
            }
            else
            {
                road.end_tangent = -1 * roadNodes[l].tangent;
            }

            //CHECK INTERSECTION
            for(int j = 0; j < i; j++)
            {

            }

            roads.Add(road);
            i++;
        }

        //DRAW ROAD TEXTUREMAP
        for (int x = 0; x < roadTextureMap.width; x++)
        {
            for (int y = 0; y < roadTextureMap.height; y++)
            {
                float distance = Mathf.Infinity;
                int index = 0;
                //SET PIXEL WITH NEAREST SPLINE DISTANCE
                for (int m = 0; m < maxRiverNumber; m++)
                {
                    float d = rivers[m].Distance(new Vector2(x, y));
                    if (d < distance)
                    {
                        distance = d;
                        index = m;
                    }
                }
                float normalizedDistance = distance / roads[0].size;

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
    private void GenerateTowns()
    {
        int n = (int)Mathf.Round(Random.Range(0, maxTownCount));
        towns = new List<Town>(n);

        for(int i = 0; i < n; i++)
        {
            towns[i].center = new Vector2(Random.Range(0, worldSize), Random.Range(0, worldSize));
            towns[i].maxRadius = Random.Range(minTownRadius, maxTownRadius);

            RoadNode townNode = new RoadNode();
            townNode.position = towns[i].center;
            townNode.tangent = Random.insideUnitCircle.normalized;
            roadNodes.Add(townNode);
        }
    }
    private MiniTerrainDecorator.Layers GenerateTree
        (TreeProfile treeProfile, int layerIndex, int targetlayerIndex, float frequency, float lacunarity)
    {
        MiniTerrainDecorator.Layers treeLayer = new MiniTerrainDecorator.Layers();
        treeLayer.active = true;
        treeLayer.name = treeProfile.name;
        treeLayer.layerType = MiniTerrainDecorator.LayerType.tree;
        treeLayer.layerIndex = layerIndex;
        treeLayer.maximumTreeCount = (int)(treeProfile.density * worldSize * worldSize);
        treeLayer.probability = 1;
        treeLayer.width = treeProfile.scale;
        treeLayer.height = treeProfile.scale;
        treeLayer.randomPosition = 1;
        treeLayer.randomRotation = 1;
        treeLayer.randomSize = treeProfile.randomScale;

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
    private bool AreAllRoadNodeConnected()
    {
        foreach(RoadNode rn in roadNodes)
        {
            if(!rn.isConnected)
            {
                return false;
            }
        }
        return true;
    }

    private void GenerateNavMeshSurface()
    {
        navMeshSurface = terrainObject.AddComponent<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();
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
        public Vector2 center;
        public float maxRadius;
    }
    class RoadNode
    {
        public Vector2 position;
        public Vector2 tangent;
        public bool isConnected;

        public RoadNode()
        {
            isConnected = false;
        }
    }
}