using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

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
    public bool FireAll = true;
    public OfficerManager targetCompany = null;
    private FiniteStateMachine OfficerStateMachine;
    public string stateName;
    public int Morale;
    public int FleeThreashold = 25;

    [Header("Debug")]
    public bool ShowSightLines = false;
    public bool ShowFormation = false;

    //SELECTION DESELECTION DETACH
    public override void OnSelection()
    {
        
    }
    public override void OnDeselection()
    {
        
    }
    public void Detach()
    { 
        
    }

    //INITIALIZE
    public override void Initialize()
    {
        //GET COMPONENTS
        mr = GetComponent<MeshRenderer>();
        um = GetComponent<OfficerMovement>();
        lineRenderer = GetComponent<LineRenderer>();

        //GET COMPANY ICON

        //SET MATERIAL
        InitializeMaterial();

        InitializeStats();
        InitializeFormation();
        //SpawnCompanyPawns();

        //INITIALIZE FINITE STATE MACHINE
        OfficerStateMachine = new FiniteStateMachine();
        FiniteStateMachineInitializer();

        //APPEND COMPANY FLAG
        Utility.Camera.GetComponent<UIManager>().AppendCompanyFlag(this);
        //APPEND COMPANY CARD
        if (Utility.Camera.GetComponent<CameraManager>().faction == faction)
        {
            Utility.Camera.GetComponent<UIManager>().AddCompanyCard(this);
        }
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
        companyBounds = CalculateCompanyBounds();
    }

    //SPAWN CONTROLLED PAWNS
    public void SpawnCompanyPawns()
    {
        for (int i = 0; i < companySize; i++)
        {
            Vector2 v2 = GetFormationCoords(i);
            SpawnPawn(Utility.V2toV3(v2) + transform.position);
        }
    }
    private void SpawnPawn(Vector3 pos)
    {
        GameObject pawn = Instantiate(pawnPrefab);
        pawn.transform.position = pos;
        pawn.transform.rotation = transform.rotation;

        PawnManager pawnManager = pawn.GetComponent<PawnManager>();
        PawnMovement pawnMovememnt = pawnManager.GetComponent<PawnMovement>();

        pawns.Add(pawnManager);
        pawnManager.masterOfficer = this;
        pawnManager.ID = pawns.Count - 1;
        pawnManager.faction = faction;

        pawnMovememnt.MovementSpeed = Speed * 1.5f;

        pawnManager.name = "Company" + companyNumber.ToString() + "_" + pawnManager.ID;

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

    //STATE MACHINE
    public State GetState()
    {
        return OfficerStateMachine.currentState;
    }
    private void FiniteStateMachineInitializer()
    {
        List<State> officerStates = new List<State>();
        List<Transition> officerTransitions = new List<Transition>();

        //STATES
        //ANY
        AnyState anyState = new AnyState();
        officerStates.Add( anyState );
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
        officerStates.Add(Idle);
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
        officerStates.Add(Moving);
        //FIRING
        State Firing = new State(
                "Firing",
                () => {
                    SendFireMessage();
                },
                null,
                null
            );
        officerStates.Add(Firing);
        //RELOADING
        State Reloading = new State(
                "Reloading",
                () => {
                    //SEND RELOAD MESSAGE
                    SendReloadMessage();
                },
                null,
                null
            );
        officerStates.Add(Reloading);
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
        officerStates.Add(Fleeing);


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
        officerTransitions.Add(AnyFleeing);
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
        officerTransitions.Add( IdleMoving );
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
        officerTransitions.Add( MovingIdle );
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
        officerTransitions.Add(IdleFiring);
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
        officerTransitions.Add(FiringReloading);
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
        officerTransitions.Add(ReloadingIdle);
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
        officerTransitions.Add(IdleReloading);



        OfficerStateMachine.AddStates(officerStates);
        OfficerStateMachine.AddTransitions(officerTransitions);

        OfficerStateMachine.initialState = Idle;

        OfficerStateMachine.Initialize();
    }

    //SENSING
    private OfficerManager EnemyInRange(float Range)
    {
        float R2 = 5f;
        Vector3 Start = transform.position + transform.up * 1 + transform.forward * -R2;

        float d = (companyFormation.Lines/2 * (Range + R2)) / (2* R2);

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