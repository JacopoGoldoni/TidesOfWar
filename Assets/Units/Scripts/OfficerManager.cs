using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class OfficerManager : UnitManager, IVisitable
{
    //STATS
    [SerializeField] BaseStats UnitTemplate;
    public Stats Stats { get; private set; }
    public void Accept(IVisitor visitor) => visitor.Visit(this);


    public int RegimentNumber;

    public GameObject PawnPrefab;

    [HideInInspector] public List<PawnManager> pawns = new List<PawnManager>();
    

    [Header("Regiment formation")]
    public int RegimentSize = 120;
    private Formation _regimentFormation;
    public Formation RegimentFormation
    {
        get => _regimentFormation;
        set
        {
            if(value != _regimentFormation)
            {
                _regimentFormation = value;
                _formationChanged = true;
            }
        }
    }
    private bool _formationChanged = false;
    public float Range = 20f;

    [Header("Regiment weapons")]
    public int Ammo = 50;
    public int MaxAmmo = 50;

    [Header("Regiment state")]
    public bool FireAll = true;
    public bool MultipleLineFire = false;
    private OfficerManager targetRegiment = null;
    private FiniteStateMachine OfficerStateMachine;
    public string stateName;
    public uint Morale = 100;
    public uint FleeThreashold = 25;

    [Header("Debug")]
    public bool ShowSightLines = false;
    public bool ShowFormation = false;

    private void Awake()
    {
        Stats = new Stats(new StatsMediator(), UnitTemplate);
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        InitializeFormation();


        SpawnRegimentSpawns();
    }

    private void SpawnRegimentSpawns()
    {
        for (int i = 0; i < RegimentSize; i++)
        {
            Vector2 v2 = GetFormationCoords(i);
            SpawnPawn(Utility.V2toV3(v2) + transform.position);
        }
    }
    public override void Initialize()
    {
        ms = GetComponent<MeshRenderer>();
        um = GetComponent<OfficerMovement>();

        m = Instantiate(UnitMaterial);

        if (Utility.Camera.GetComponent<CameraManager>().faction == faction)
        {
            m.SetColor("_Color", Color.green);
        }
        else
        {
            m.SetColor("_Color", Color.red);
        }

        ms.material = m;

        OfficerStateMachine = new FiniteStateMachine();

        StateMachineInitializer();
    }
    private void InitializeFormation()
    {
        RegimentFormation = new Line(RegimentSize);
    }

    // Update is called once per frame
    void Update()
    {
        targetRegiment = EnemyInRange(Range);

        //STATE MACHINE
        OfficerStateMachine.Update();

        stateName = OfficerStateMachine.currentState.name;

        //UPDATE TIMER
        UpdateTimers();
    }

    private void UpdateTimers()
    {
        Stats.Mediator.Update(Time.deltaTime);
    }


    public void Highlight(bool highlight)
    {
        if(highlight)
        {
            m.SetInt("_Hightlight", 1);
        }
        else
        {
            m.SetInt("_Hightlight", 0);
        }
    }

    private void SpawnPawn(Vector3 pos)
    {
        GameObject pawn = Instantiate(PawnPrefab);
        pawn.transform.position = pos;

        PawnManager pm = pawn.GetComponent<PawnManager>();

        pawns.Add(pm);
        pm.masterOfficer = this;
        pm.ID = pawns.Count - 1;
        pm.faction = faction;

        pm.name = "Regiment" + RegimentNumber.ToString() + "_" + pm.ID;

        pm.Initialize();
    }

    //FORMATION MANAGEMENT
    private void CheckFormation()
    {
        pawns.RemoveAll(item => item == null);

        if(RegimentSize != pawns.Count)
        {
            RegimentSize = pawns.Count;

            string t = GetFormationType();

            switch(t)
            {
                case "Line":
                    RegimentFormation = new Line(RegimentSize);
                    break;
                case "Column":
                    RegimentFormation = new Column(RegimentSize);
                    break;
                default:
                    RegimentFormation = new Line(RegimentSize);
                    break;
            }
        }
    }
    public void SetFormation(Formation formation)
    {
        RegimentFormation = formation;
    }
    public void SendFormation()
    {
        for (int i = 0; i < pawns.Count; i++)
        {
            pawns[i].MoveTo(
                GetFormationCoords(i) + Utility.V3toV2(transform.position),
                um.CurrentRotation()
                );
        }
    }
    public Vector2 GetFormationCoords(int ID)
    {
        Vector2 pos2 = RegimentFormation.GetPos(ID);

        pos2.y *= -1;

        Vector3 pos3 = Utility.V2toV3(pos2);

        Quaternion rotation = transform.rotation;

        pos3 = rotation * pos3;

        return Utility.V3toV2(pos3);
    }
    public bool ArePawnIdle()
    {
        foreach(PawnManager pm in pawns)
        {
            if(pm.um.IsMoving())
            {
                return false;
            }
            
        }
        return true;
    }
    public int GetPawnRank(int ID)
    {
        return RegimentFormation.GetRank(ID);
    }
    public string GetFormationType()
    {
        return RegimentFormation.GetType().ToString();
    }

    //FIRE MANAGEMENT
    public void SendFireMessage()
    {
        for (int i = 0; i < pawns.Count; i++)
        {
            if(GetPawnRank(i) == 1)
            {
                //FIRE ONLY FIRST RANK
                pawns[i].CallFire();
            }
        }
        Ammo -= 1;
    }
    public void SendRelaodMessage()
    {
        for (int i = 0; i < pawns.Count; i++)
        {
            if (GetPawnRank(i) == 1)
            {
                //FIRE ONLY FIRST RANK
                pawns[i].CallReload();
            }
        }
        Ammo -= 1;
    }
    public bool CheckLoadedStatus()
    {
        //TRUE IF ALL LOADED, FALSE IF AT LEAST ONE IS NOT LOADED
        for (int i = 0; i < pawns.Count; i++)
        {
            if (GetPawnRank(i) == 1)
            {
                if (pawns[i] != null)
                {
                    //CHECK ONLY FIRST RANK
                    if (!pawns[i].Loaded)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    public bool CheckUnLoadedStatus()
    {
        //TRUE IF ALL UNLOADED, FALSE IF AT LEAST ONE IS LOADED
        for (int i = 0; i < pawns.Count; i++)
        {
            if (GetPawnRank(i) == 1)
            {
                if (pawns[i] != null)
                {
                    //CHECK ONLY FIRST RANK
                    if (pawns[i].Loaded)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    //STATE MACHINE
    public State GetState()
    {
        return OfficerStateMachine.currentState;
    }
    private void StateMachineInitializer()
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
                null,
                null,
                () => {
                    CheckFormation();

                    if (um.MovementPoints.Count != 0 || _formationChanged)
                    {
                        SendFormation();
                        um.SetDestination(Utility.V3toV2(transform.position), transform.rotation);
                        _formationChanged = false;
                    }
                }
            );
        officerStates.Add(Idle);
        //MOVING
        State Moving = new State(
                "Moving",
                null,
                null,
                () => {
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
                    SendRelaodMessage();
                },
                null,
                null
            );
        officerStates.Add(Reloading);
        //FLEE
        State Fleeing = new State(
                "Fleeing",
                () => { 
                    um.ClearDestination();
                    um.SetDestination(Utility.V3toV2(transform.position), transform.rotation);
                },
                null,
                () => 
                {
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
                    if (um.MovementPoints.Count != 0 || !ArePawnIdle())
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
                    if (um.MovementPoints.Count == 0 && ArePawnIdle())
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
                    if (targetRegiment != null && CheckLoadedStatus() && GetFormationType() == "Line")
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
                    if (CheckUnLoadedStatus())
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

        float d = (RegimentFormation.Lines/2 * (Range + R2)) / (2* R2);

        Vector2 a = Utility.V3toV2((transform.forward * (R2 + Range) + transform.right * d).normalized * (R2 + Range));
        Vector2 b = Utility.V3toV2((transform.forward * (R2 + Range) + transform.right * -d).normalized * (R2 + Range));

        if(faction == Factions.France && ShowSightLines)
        {
            Debug.DrawLine(Start, Start + transform.forward * (R2 + Range), Color.red, 0f, true);
            Debug.DrawLine(Start, Start + Utility.V2toV3(a), Color.red, 0f, true);
            Debug.DrawLine(Start, Start + Utility.V2toV3(b), Color.red, 0f, true);
        }
        List<OfficerManager> Units = GameUtility.FindAllRegiments();

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
        for (int i = 0; i < RegimentSize; i++)
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Vector3 FormationSlot = Utility.V2toV3(GetFormationCoords(i)) + transform.position;
            Gizmos.DrawSphere(FormationSlot, 0.2f);
        }
    }
}