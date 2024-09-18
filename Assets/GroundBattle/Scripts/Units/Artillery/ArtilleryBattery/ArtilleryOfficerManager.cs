using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public partial class ArtilleryOfficerManager : UnitManager, IVisitable
{
    //COMPONENTS
    LineRenderer lineRenderer;
    public ArtilleryBatteryCardManager artilleryBatteryCardManager;

    //STATS
    [SerializeField] public ArtilleryBatteryTemplate artilleryBatteryTemplate;
    public Stats stats { get; private set; }
    public void Accept(IVisitor visitor) => visitor.Visit(this);

    public MeshFilter batterySightMeshFilter;

    [Header("Cannons")]
    public GameObject cannonPrefab;
    public List<ArtilleryManager> cannons = new List<ArtilleryManager>();
    public int batterySize { get => cannons.Count; }

    [Header("Battery identifier")]
    public int batteryNumber;
    public string batteryName;

    [Header("Battery formation")]
    public Formation batteryFormation;
    private bool _formationChanged = false;
    public Bounds batteryBounds;

    [Header("Carriage")]
    public List<ArtilleryCarriageManager> carriages = new List<ArtilleryCarriageManager>();
    public GameObject carriagePrefab;
    private Formation carriageFormation;

    [Header("Battery combact")]
    public float Precision { get { return stats.Precision; } }
    public float Range = 20f;
    public int MaxAmmo { get { return artilleryBatteryTemplate.MaxAmmo; } }
    public int Ammo;

    [Header("Battery movement")]
    public float Speed { get { return stats.Speed; } }
    public const float RunMultiplier = 1.5f;
    public bool IsCarried = false;
    public bool drawPathLine = false;

    [Header("Abilities")]
    public bool ExplosiveShells { get { return artilleryBatteryTemplate.ExplosiveShells; } }
    public bool GrapeShots { get { return artilleryBatteryTemplate.GrapeShots; } }

    [Header("Battery state")]
    public bool FireAll = true;
    public UnitManager targeUnit = null;
    private FiniteStateMachine artilleryBatteryStateMachine;
    public string stateName;
    public int Morale;
    public int FleeThreashold = 25;

    [Header("Debug")]
    public bool ShowSightLines = false;
    public bool ShowFormation = false;

    public void Start()
    {
        Initialize();
    }

    //INITIALIZE
    public override void Initialize()
    {
        //GET COMPONENTS
        um = GetComponent<ArtilleryOfficerMovement>();
        lineRenderer = GetComponent<LineRenderer>();

        InitializeMeshes();
        InitializeMaterial();

        InitializeStats();
        InitializeFormation();
        SpawnBatteryCannons();
        SpawnBatteryCarriages();

        //INITIALIZE FINITE STATE MACHINE
        artilleryBatteryStateMachine = new FiniteStateMachine();
        FiniteStateMachineInitializer();

        //APPEND COMPANY FLAG
        Utility.Camera.GetComponent<UIManager>().AppendArtilleryBatteryFlag(this);
        //APPEND COMPANY CARD
        if (TAG == Utility.CameraManager.TAG)
        {
            Utility.Camera.GetComponent<UIManager>().AddArtilleryBatteryCard(this);
        }

        GenerateSightMesh(11, (batteryFormation.Lines - 1) * batteryFormation.a / 2f, artilleryBatteryTemplate.Range, 5f);
    }
    private void InitializeStats()
    {
        //INITIALIZE STATS
        stats = new Stats(new StatsMediator(), artilleryBatteryTemplate);

        um.MovementSpeed = Speed;
        Ammo = MaxAmmo;
        Morale = artilleryBatteryTemplate.BaseMorale;
        Range = artilleryBatteryTemplate.Range;
    }
    private void InitializeFormation()
    {
        batteryFormation = new Line(artilleryBatteryTemplate.BatterySize);
        batteryFormation.SetSizeByRanks(artilleryBatteryTemplate.BatterySize, 1);
        batteryFormation.a = 4f;
        batteryBounds = CalculateCompanyBounds();

        carriageFormation = new Column(artilleryBatteryTemplate.CarriageSize);
        carriageFormation.SetSizeByLines(artilleryBatteryTemplate.CarriageSize, 1);
        carriageFormation.b = -4f;
    }

    //SPAWN CONTROLLED CANNONS
    public void SpawnBatteryCannons()
    {
        for (int i = 0; i < artilleryBatteryTemplate.BatterySize; i++)
        {
            Vector2 v2 = GetFormationCoords(i);
            SpawnCannon(Utility.V2toV3(v2) + transform.position - Vector3.up * 0.2f);
        }
    }
    private void SpawnCannon(Vector3 pos)
    {
        GameObject cannon = Instantiate(cannonPrefab);
        cannon.transform.position = pos;
        cannon.transform.rotation = transform.rotation;

        ArtilleryManager artilleryManager = cannon.GetComponent<ArtilleryManager>();
        ArtilleryMovement artilleryMovememnt = artilleryManager.GetComponent<ArtilleryMovement>();

        cannons.Add(artilleryManager);
        artilleryManager.masterOfficer = this;
        artilleryManager.ArtilleryLocalID = cannons.Count - 1;
        artilleryManager.TAG = TAG;
        artilleryManager.firstClassCrewSize = 6;

        artilleryMovememnt.MovementSpeed = Speed * 1.5f;

        artilleryManager.name = "Battery" + batteryName.ToString() + "_" + artilleryManager.ArtilleryLocalID;

        artilleryManager.Initialize();
    }

    //SPAWN CARRIAGE
    public void SpawnBatteryCarriages()
    {
        for (int i = 0; i < artilleryBatteryTemplate.CarriageSize; i++)
        {
            Vector2 v2 = CarriagePosition(i);
            SpawnCarriage(Utility.V2toV3(v2) + transform.position + Vector3.up * 0.4f);
        }
    }
    public void SpawnCarriage(Vector3 pos)
    {
        GameObject carriage = Instantiate(carriagePrefab);
        carriage.transform.position = pos;
        carriage.transform.rotation = transform.rotation;

        ArtilleryCarriageManager carriageManager = carriage.GetComponent<ArtilleryCarriageManager>();
        ArtilleryCarriageMovement carriageMovememnt = carriageManager.GetComponent<ArtilleryCarriageMovement>();

        carriages.Add(carriageManager);
        carriageManager.masterArtilleryOfficer = this;
        carriageManager.local_ID = carriages.Count - 1;
        carriageManager.TAG = TAG;

        carriageMovememnt.MovementSpeed = Speed * 1.5f;

        carriageManager.name = "Battery" + batteryNumber.ToString() + "_Carriage_" + carriageManager.local_ID;

        carriageManager.Initialize();
    }
    private Vector2 CarriagePosition(int i)
    {
        Vector2 pos2;
        if (batteryFormation.name == "Column")
        {
            //IN COLUMN
            pos2 = Vector2.down * (cannons.Count * batteryFormation.b + 6) + carriageFormation.GetPos(i);
        }
        else
        {
            //IN LINE
            pos2 = Vector2.down * 6f + carriageFormation.GetPos(i);
        }

        Vector3 pos3 = Utility.V2toV3(pos2);

        Quaternion rotation = transform.rotation;

        pos3 = rotation * pos3;
        
        return Utility.V3toV2(pos3);
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

        batterySightMeshFilter.mesh = sightMesh;
    }

    public override void OnSelection()
    {
        batterySightMeshFilter.gameObject.SetActive(true);
    }
    public override void OnDeselection()
    {
        batterySightMeshFilter.gameObject.SetActive(false);
    }

    //UPDATES
    void Update()
    {

        CalculateMorale();

        //STATE MACHINE
        artilleryBatteryStateMachine.Update();

        stateName = artilleryBatteryStateMachine.currentState.name;

        //UPDATE TIMER
        UpdateTimers();

        DrawPathLine();
    }
    private void UpdateTimers()
    {
        //Stats.Mediator.Update(Time.deltaTime);
    }

    //STATE MACHINE
    public State GetState()
    {
        return artilleryBatteryStateMachine.currentState;
    }
    private void FiniteStateMachineInitializer()
    {
        List<State> artilleryOfficerStates = new List<State>();
        List<Transition> artilleryOfficerTransitions = new List<Transition>();

        //STATES
        //ANY
        AnyState anyState = new AnyState();
        artilleryOfficerStates.Add( anyState );
        //IDLE
        State Idle = new State(
                "Idle",
                () => {
                    CheckFormation();
                    if (_formationChanged)
                    {
                        ReceiveMovementOrder(false, Utility.V3toV2(transform.position), transform.rotation);
                        SendFormation();
                        _formationChanged = false;
                    }
                },
                () =>
                {
                    targeUnit = null;
                },
                () =>
                {
                    targeUnit = EnemyInRange(artilleryBatteryTemplate.Range);
                }
            );
        artilleryOfficerStates.Add(Idle);
        //MOVING
        State Moving = new State(
                "Moving",
                null,
                null,
                () => {
                    //MOVEMENT BEHAVIOUR
                    ((ArtilleryOfficerMovement)um).UpdateMovement();

                    //CANNONS MOVEMENT
                    SendFormation();
                }
            );
        artilleryOfficerStates.Add(Moving);
        //MOUNTING
        State Mounting = new State(
                "Mounting",
                () => {
                    //CANNONS MOVEMENT
                    SendFormation();
                },
                null,
                null
            );
        artilleryOfficerStates.Add(Mounting);
        //DISMOUNTING
        State Dismounting = new State(
                "Dismounting",
                () => {
                    //CANNONS MOVEMENT
                    SendFormation();
                },
                null,
                null
            );
        artilleryOfficerStates.Add(Dismounting);
        //FLEE
        State Fleeing = new State(
                "Fleeing",
                () => {

                    //Vector3 fleePos = (transform.position - targetCompany.transform.position).normalized + transform.position;
                    //Quaternion fleeRot = Quaternion.LookRotation((transform.position - targetCompany.transform.position).normalized, Vector3.up);

                    //um.SetDestination(Utility.V3toV2(fleePos), fleeRot);
                },
                null,
                () => 
                {
                    ((OfficerMovement)um).UpdateMovement();
                    SendFormation();
                }
            );
        artilleryOfficerStates.Add(Fleeing);


        //TRANSITIONS
            //ANY -> FLEEING
        Transition AnyFleeing = new Transition(
                anyState,
                Fleeing,
                () => {
                    if(Morale < FleeThreashold)
                    {
                        return true;
                    }
                    return false;
                }
            );
        artilleryOfficerTransitions.Add( AnyFleeing );
            //IDLE -> MOUNTING
        Transition IdleMounting = new Transition(
                Idle,
                Mounting,
                () => {
                    return um.MovementPoints.Count != 0;
                }
            );
        artilleryOfficerTransitions.Add( IdleMounting );
            //DISMOUNTING -> IDLE
        Transition DismountingIdle = new Transition(
                Dismounting,
                Idle,
                () => {
                    return AllDismounted();
                }
            );
        artilleryOfficerTransitions.Add( DismountingIdle );
        //MOUNTING -> MOVING
        Transition MountingMoving = new Transition(
                Mounting,
                Moving,
                () => {
                    return AllMounted();
                }
            );
        artilleryOfficerTransitions.Add(MountingMoving);
        //MOVING -> DISMOUNTING
        Transition MovingDismounting = new Transition(
                Moving,
                Dismounting,
                () => {
                    return um.MovementPoints.Count == 0;
                }
            );
        artilleryOfficerTransitions.Add(MovingDismounting);


        artilleryBatteryStateMachine.AddStates(artilleryOfficerStates);
        artilleryBatteryStateMachine.AddTransitions(artilleryOfficerTransitions);

        artilleryBatteryStateMachine.initialState = Idle;

        artilleryBatteryStateMachine.Initialize();
    }

    //SENSING
    private UnitManager EnemyInRange(float Range)
    {
        float R2 = 5f;
        Vector3 Start = transform.position + transform.up + transform.forward * - R2;

        float d = ((batteryFormation.Lines - 1) * batteryFormation.a * (Range + R2)) / (2 * R2);

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

    //GIZMOS
    public void OnDrawGizmos()
    {
        if (ShowFormation)
            FormationGizmo();
    }
    private void FormationGizmo()
    {
        for (int i = 0; i < artilleryBatteryTemplate.BatterySize; i++)
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Vector3 FormationSlot = Utility.V2toV3(GetFormationCoords(i)) + transform.position;
            Gizmos.DrawSphere(FormationSlot, 0.2f);
        }
    }
}