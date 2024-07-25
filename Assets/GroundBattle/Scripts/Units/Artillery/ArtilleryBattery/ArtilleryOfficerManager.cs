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

    //STATS
    [SerializeField] public ArtilleryBatteryTemplate artilleryBatteryTemplate;
    public Stats stats { get; private set; }
    public void Accept(IVisitor visitor) => visitor.Visit(this);

    [Header("Cannons")]
    public GameObject cannonPrefab;
    public List<ArtilleryManager> cannons = new List<ArtilleryManager>();

    [Header("Battery identifier")]
    public int batteryNumber;
    public string batteryName;

    [Header("Battery formation")]
    public Formation batteryFormation;
    private bool _formationChanged = false;
    public int batterySize = 4;
    public Bounds batteryBounds;

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
    public UnitManager targetCompany = null;
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
        ms = GetComponent<MeshRenderer>();
        um = GetComponent<ArtilleryOfficerMovement>();
        lineRenderer = GetComponent<LineRenderer>();

        //SET MATERIAL
        InitializeMaterial();

        InitializeStats();
        InitializeFormation();
        SpawnBatteryCannons();

        //INITIALIZE FINITE STATE MACHINE
        artilleryBatteryStateMachine = new FiniteStateMachine();
        FiniteStateMachineInitializer();

        //APPEND COMPANY FLAG
        Utility.Camera.GetComponent<UIManager>().AppendArtilleryBatteryFlag(this);
        //APPEND COMPANY CARD
        if (Utility.Camera.GetComponent<CameraManager>().faction == faction)
        {
            Utility.Camera.GetComponent<UIManager>().AddArtilleryBatteryCard(this);
        }
    }
    private void InitializeStats()
    {
        //INITIALIZE STATS
        //stats = new Stats(new StatsMediator(), companyTemplate);

        um.MovementSpeed = Speed;
        batterySize = Mathf.CeilToInt(artilleryBatteryTemplate.BatterySize * GameUtility.UNIT_SCALE);
        Ammo = MaxAmmo;
        Morale = artilleryBatteryTemplate.BaseMorale;
        Range = artilleryBatteryTemplate.Range;
    }
    private void InitializeFormation()
    {
        batteryFormation = new Line(batterySize);
        batteryFormation.SetSizeByRanks(batterySize, 1);
        batteryBounds = CalculateCompanyBounds();
    }

    //SPAWN CONTROLLED PAWNS
    public void SpawnBatteryCannons()
    {
        for (int i = 0; i < batterySize; i++)
        {
            Vector2 v2 = GetFormationCoords(i);
            SpawnCannons(Utility.V2toV3(v2) + transform.position);
        }
    }
    private void SpawnCannons(Vector3 pos)
    {
        GameObject cannon = Instantiate(cannonPrefab);
        cannon.transform.position = pos;
        cannon.transform.rotation = transform.rotation;

        ArtilleryManager artilleryManager = cannon.GetComponent<ArtilleryManager>();
        ArtilleryMovement artilleryMovememnt = artilleryManager.GetComponent<ArtilleryMovement>();

        cannons.Add(artilleryManager);
        artilleryManager.masterOfficer = this;
        artilleryManager.ArtilleryLocalID = cannons.Count - 1;
        artilleryManager.faction = faction;

        artilleryMovememnt.MovementSpeed = Speed * 1.5f;

        artilleryManager.name = "Battery" + batteryName.ToString() + "_" + artilleryManager.ArtilleryLocalID;

        artilleryManager.Initialize();
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
        //artilleryBatteryStateMachine.Update();

        //stateName = artilleryBatteryStateMachine.currentState.name;

        //UPDATE TIMER
        UpdateTimers();

        DrawPathLine();
    }
    private void UpdateTimers()
    {
        //Stats.Mediator.Update(Time.deltaTime);
    }

    public float GetWidth()
    {
        return (batteryFormation.Lines - 1) * batteryFormation.a;
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
                null,
                null
            );
        artilleryOfficerStates.Add(Idle);
        //MOVING
        State Moving = new State(
                "Moving",
                null,
                null,
                () => {
                    //MOVEMENT BEHAVIOUR
                    ((OfficerMovement)um).UpdateMovement();

                    //PAWN MOVEMENT
                    SendFormation();
                }
            );
        artilleryOfficerStates.Add(Moving);
        //FIRING
        State Firing = new State(
                "Firing",
                () => {
                    SendFireMessage();
                },
                null,
                null
            );
        artilleryOfficerStates.Add(Firing);
        //RELOADING
        State Reloading = new State(
                "Reloading",
                () => {
                    //SEND RELOAD MESSAGE
                    SendRelaodMessage();
                },
                null,
                null
            );
        artilleryOfficerStates.Add(Reloading);
        //FLEE
        State Fleeing = new State(
                "Fleeing",
                () => {

                    Vector3 fleePos = (transform.position - targetCompany.transform.position).normalized + transform.position;
                    Quaternion fleeRot = Quaternion.LookRotation((transform.position - targetCompany.transform.position).normalized, Vector3.up);

                    um.SetDestination(Utility.V3toV2(fleePos), fleeRot);
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
        artilleryOfficerTransitions.Add(AnyFleeing);
            //IDLE -> MOVING
        Transition IdleMoving = new Transition(
                Idle,
                Moving,
                () => {
                    if (um.MovementPoints.Count != 0)
                    {
                        return true;
                    }
                    return false;
                }
            );
        artilleryOfficerTransitions.Add( IdleMoving );
            //MOVING -> IDLE
        Transition MovingIdle = new Transition(
                Moving,
                Idle,
                () => {
                    if (um.MovementPoints.Count == 0)
                    {
                        return true;
                    }
                    return false;
                }
            );
        artilleryOfficerTransitions.Add( MovingIdle );
            //IDLE -> FIRING
        Transition IdleFiring = new Transition(
                Idle,
                Firing,
                () => {
                    if (targetCompany != null && CheckLoadedStatus() && GetFormationType() == "Line")
                    {
                        return true;
                    }
                    return false;
                }
            );
        artilleryOfficerTransitions.Add(IdleFiring);
            //FIRING -> RELOADING
        Transition FiringReloading = new Transition(
                Firing,
                Reloading,
                () => {
                    if (CheckUnLoadedStatus() && um.MovementPoints.Count == 0)
                    {
                        return true;
                    }
                    return false;
                }
            );
        artilleryOfficerTransitions.Add(FiringReloading);
            //RELOADING -> IDLE
        Transition ReloadingIdle = new Transition(
                Reloading,
                Idle,
                () => {
                    if (CheckLoadedStatus())
                    {
                        return true;
                    }
                    return false;
                }
            );
        artilleryOfficerTransitions.Add(ReloadingIdle);
        //IDLE -> RELOADING
        Transition IdleReloading = new Transition(
                Reloading,
                Idle,
                () => {
                    if (CheckUnLoadedStatus())
                    {
                        return true;
                    }
                    return false;
                }
            );
        artilleryOfficerTransitions.Add(IdleReloading);


        artilleryBatteryStateMachine.AddStates(artilleryOfficerStates);
        artilleryBatteryStateMachine.AddTransitions(artilleryOfficerTransitions);

        artilleryBatteryStateMachine.initialState = Idle;

        artilleryBatteryStateMachine.Initialize();
    }

    //SENSING
    private UnitManager EnemyInRange(float Range)
    {
        float R2 = 5f;
        Vector3 Start = transform.position + transform.up * 1 + transform.forward * -R2;

        float d = (batteryFormation.Lines/2 * (Range + R2)) / (2* R2);

        Vector2 a = Utility.V3toV2((transform.forward * (R2 + Range) + transform.right * d).normalized * (R2 + Range));
        Vector2 b = Utility.V3toV2((transform.forward * (R2 + Range) + transform.right * -d).normalized * (R2 + Range));

        if(faction == Factions.France && ShowSightLines)
        {
            Debug.DrawLine(Start, Start + transform.forward * (R2 + Range), Color.red, 0f, true);
            Debug.DrawLine(Start, Start + Utility.V2toV3(a), Color.red, 0f, true);
            Debug.DrawLine(Start, Start + Utility.V2toV3(b), Color.red, 0f, true);
        }
        List<OfficerManager> Units = GroundBattleUtility.GetAllCompanies();

        foreach(OfficerManager of in Units)
        {
            if(of.faction != faction)
            {
                //IF NOT SAME FACTION
                Vector2 p = Utility.V3toV2(of.transform.position - Start);


                if (faction == Factions.France && ShowSightLines)
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
        for (int i = 0; i < batterySize; i++)
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Vector3 FormationSlot = Utility.V2toV3(GetFormationCoords(i)) + transform.position;
            Gizmos.DrawSphere(FormationSlot, 0.2f);
        }
    }
}