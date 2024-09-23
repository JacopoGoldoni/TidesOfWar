using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics.Internal;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.Scenes;
using Unity.Rendering;

public partial class OfficerManager : UnitManager, IVisitable
{
    //COMPONENTS
    LineRenderer lineRenderer;

    //STATS
    [SerializeField] public CompanyTemplate companyTemplate;
    public Stats stats { get; private set; }
    public void Accept(IVisitor visitor) => visitor.Visit(this);

    public int ID;

    public CaptainManager masterCaptain;
    public CompanyCardManager companyCardRef;

    public MeshFilter companySightMeshFilter;

    [Header("Pawns")]
    public GameObject pawnPrefab;
    public List<PawnManager> pawns = new List<PawnManager>();

    [Header("Company identifier")]
    public int companyNumber;
    public string companyName;

    [Header("Company formation")]
    public int companySize = 120;
    private Vector2[] snakeFormationVertices;
    private Formation _companyFormation;
    public Formation companyFormation
    {
        get => _companyFormation;
        set
        {
            if(value != _companyFormation)
            {
                _companyFormation = value;
                _formationChanged = true;
            }
        }
    }
    private bool _formationChanged = false;
    public float Range = 20f;
    public Bounds companyBounds;

    [Header("Company combact")]
    public float Precision { get { return stats.Precision; } }
    public int Ammo;
    public int MaxAmmo { get { return companyTemplate.MaxAmmo; } }

    [Header("Company movement")]
    public float Speed { get { return stats.Speed; } }
    public const float RunMultiplier = 1.5f;
    public bool drawPathLine = false;

    [Header("Abilities")]
    public bool MultipleLineFire { get { return companyTemplate.MultipleFire; } }
    public bool Fortification { get { return companyTemplate.Fortification; } }
    public bool Skirmish { get { return companyTemplate.Skirmish; } }

    [Header("Company state")]
    public bool isRouting { get { return stateName == "Fleeing"; } }
    public bool FireAll = true;
    public UnitManager targetUnit = null;
    private FiniteStateMachine OfficerStateMachine;
    public string stateName;
    public int Morale;
    public int FleeThreashold = 25;

    [Header("Debug")]
    public bool ShowSightLines = false;
    public bool ShowFormation = false;

    //SELECTION DESELECTION DETACH ATTACH
    public override void OnSelection()
    {
        companyCardRef.HighLight(true);
        companySightMeshFilter.gameObject.SetActive(true);
    }
    public override void OnDeselection()
    {
        companyCardRef.HighLight(false);
        companySightMeshFilter.gameObject.SetActive(false);
    }
    public void Detach()
    {
        masterCaptain.battalionFormation.LineCompanies.Remove(this);
        masterCaptain.battalionFormation.RearCompanies.Remove(this);
        masterCaptain.battalionFormation.FrontCompanies.Remove(this);

        masterCaptain = null;

        Utility.Camera.GetComponent<CameraManager>().uimanager.UpdateCompanyCommandStatus();
    }
    public void Attach(CaptainManager battalion)
    {
        masterCaptain = battalion;

        masterCaptain.battalionFormation.AddCompany(this);
    }

    //INITIALIZE
    public override void Initialize()
    {
        //GET COMPONENTS
        um = GetComponent<OfficerMovement>();
        lineRenderer = GetComponent<LineRenderer>();

        InitializeMeshes();
        InitializeMaterial();

        InitializeStats();
        InitializeFormation();
        GroundBattleUtility.RegisterCompany(this);
        //SpawnCompanyPawns();

        //INITIALIZE FINITE STATE MACHINE
        OfficerStateMachine = new FiniteStateMachine();
        FiniteStateMachineInitializer();

        //APPEND COMPANY FLAG
        Utility.Camera.GetComponent<UIManager>().AppendCompanyFlag(this);
        //APPEND COMPANY CARD
        if (Utility.Camera.GetComponent<CameraManager>().TAG == TAG)
        {
            Utility.Camera.GetComponent<UIManager>().AddCompanyCard(this);
        }

        GenerateSightMesh(11, (companyFormation.Lines - 1) * companyFormation.a / 2f, companyTemplate.Range, 5f);
    }
    private void InitializeStats()
    {
        //INITIALIZE STATS
        stats = new Stats(new StatsMediator(), companyTemplate);

        um.MovementSpeed = Speed;
        companySize = Mathf.CeilToInt(companyTemplate.CompanySize * GameUtility.UNIT_SCALE);
        Ammo = MaxAmmo;
        Morale = companyTemplate.BaseMorale;
        Range = companyTemplate.Range;
    }
    private void InitializeFormation()
    {
        companyFormation = new Line((int)companySize);
        companyFormation.a = companyTemplate.FilesDistances;
        companyFormation.b = companyTemplate.RankDistance;
        companyBounds = CalculateCompanyBounds();
    }

    //SPAWN CONTROLLED PAWNS
    public void SpawnCompanyPawns()
    {
        //NEW SYSTEM
        OfficerSpawnManager officerSpawnManager = GetComponent<OfficerSpawnManager>();
        float3[] pawnPositions = new float3[companySize];
        for(int i = 0; i < companySize; i++)
        {
            //GET WORLD POSITION
            Vector2 v2 = GetFormationCoords(i);
            v2 += Utility.V3toV2(transform.position);

            //CONVERT TO FLOAT3
            pawnPositions[i] = new float3(v2.x ,0f, v2.y);
        }
        officerSpawnManager.Spawn(pawnPositions);

        //OLD SYSTEM
        //for (int i = 0; i < companySize; i++)
        //{
        //    Vector2 v2 = GetFormationCoords(i);
        //    SpawnPawn(v2 + Utility.V3toV2(transform.position));
        //}
    }
    private void SpawnPawn(Vector2 pos)
    {
        //INSTANTIATE PAWN
        GameObject pawn = Instantiate(pawnPrefab);
        pawn.transform.position = GroundBattleUtility.GetMapPosition(pos);
        pawn.transform.rotation = transform.rotation;

        //GET COMPONENTS
        PawnManager pawnManager = pawn.GetComponent<PawnManager>();
        PawnMovement pawnMovememnt = pawnManager.GetComponent<PawnMovement>();

        //APPEND TO COMPANY PAWNS
        pawns.Add(pawnManager);

        //LOAD INFOS INTO PAWN
        pawnManager.masterOfficer = this;
        pawnManager.local_ID = pawns.Count - 1;
        pawnManager.TAG = TAG;
        pawnMovememnt.MovementSpeed = Speed * 1.5f;
        pawnManager.meshes_LODs = companyTemplate.SoldierMesh_LODS;
        pawnManager.unitMaterial = companyTemplate.officerMaterial;

        //NAME PAWN
        pawnManager.name = "Company" + companyNumber.ToString() + "_" + pawnManager.local_ID;

        //WARP AGENT TO NAVMESH
        GroundBattleUtility.WarpAgentToNavMesh(pawn);

        pawnManager.Initialize();
    }

    //UPDATES
    void Update()
    {
        /*
        if(stateName != "Fleeing")
        {
            targetCompany = EnemyInRange(Range);
        }
        */

        CalculateMorale();

        //STATE MACHINE
        OfficerStateMachine.Update();

        stateName = OfficerStateMachine.currentState.name;

        //UPDATE TIMER
        UpdateTimers();

        DrawPathLine();
    }
    private void UpdateTimers()
    {
        //Stats.Mediator.Update(Time.deltaTime);
    }

    public void GenerateSightMesh(int arcVertices, float baseWidth, float SightRadius, float CenterRadius)
    {
        Mesh sightMesh = new Mesh
        {
            name = "Sight mesh"
        };

        float unitAngle = Mathf.Atan(baseWidth / CenterRadius) / ((arcVertices - 1) / 2f);

        Vector3 s = Vector3.forward * -CenterRadius;

        //VERTICES
        List<Vector3> vertices = new List<Vector3>
        {
            //BASE VERTICES
            Vector3.zero,                       //0
            Vector3.right * -baseWidth,         //1
            Vector3.right * baseWidth           //2
        };

        //ARC VERTICES
        for (int i = 0; i < arcVertices; i++)
        {
            float angle = (i - (arcVertices - 1) / 2f) * -unitAngle * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 v = s + rot * Vector3.forward * (CenterRadius + SightRadius);

            vertices.Add(v);
        }

        //FACES
        List<int> faces = new List<int>
        {
            //BASE FACES
            1,
            arcVertices + 2,
            0
        };

        //ARC FACES
        for (int i = 0; i < arcVertices; i++)
        {
            faces.Add(0);
            faces.Add(2 + i + 1);
            faces.Add(2 + i);
        }

        sightMesh.vertices = vertices.ToArray();
        sightMesh.triangles = faces.ToArray();

        companySightMeshFilter.mesh = sightMesh;
    }

    //SENSING
    private UnitManager EnemyInRange(float Range)
    {
        float R2 = 5f;
        Vector3 Start = transform.position + transform.up * 1 + transform.forward * -R2;

        float d = ((companyFormation.Lines - 1) * companyFormation.a * (Range + R2)) / (2* R2);

        Vector2 a = Utility.V3toV2((transform.forward * (R2 + Range) + transform.right * d).normalized * (R2 + Range));
        Vector2 b = Utility.V3toV2((transform.forward * (R2 + Range) + transform.right * -d).normalized * (R2 + Range));

        if(TAG == Utility.CameraManager.TAG && ShowSightLines)
        {
            Debug.DrawLine(Start, Start + transform.forward * (R2 + Range), Color.red, 0f, true);
            Debug.DrawLine(Start, Start + Utility.V2toV3(a), Color.red, 0f, true);
            Debug.DrawLine(Start, Start + Utility.V2toV3(b), Color.red, 0f, true);
        }
        List<OfficerManager> Units = GroundBattleUtility.GetAllCompanies();

        foreach(OfficerManager of in Units)
        {
            if(of.TAG != TAG)
            {
                //IF NOT SAME FACTION
                Vector2 p = Utility.V3toV2(of.transform.position - Start);


                if (TAG == Utility.CameraManager.TAG && ShowSightLines)
                {
                    Debug.DrawLine(Start, Start + Utility.V2toV3(p), Color.yellow, 0f, true);
                }

                if (UtilityMath.IsInCircularSector(p, R2, Range + R2, b, a))
                {
                    //IS IN SIGHT
                    return of;
                }
            }
        }

        return null;
    }
    private int AlliedCompaniesInRange(float Range)
    {
        int n = 0;
        foreach(OfficerManager om in GroundBattleUtility.GetAllCompanies())
        {
            if(om != this)
            {
                float distance = Utility.V3toV2(om.gameObject.transform.position - gameObject.transform.position).magnitude;
                if(distance <= Range)
                {
                    n++;
                }
            }
        }
        return n;
    }

    //GIZMOS
    public void OnDrawGizmos()
    {

        if (ShowFormation)
            FormationGizmo();
    }
    private void FormationGizmo()
    {
        for (int i = 0; i < companySize; i++)
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Vector3 FormationSlot = Utility.V2toV3(GetFormationCoords(i)) + transform.position;
            Gizmos.DrawSphere(FormationSlot, 0.2f);
        }
    }
}