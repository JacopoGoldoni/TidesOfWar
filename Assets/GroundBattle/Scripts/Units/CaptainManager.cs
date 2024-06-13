using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CaptainManager : UnitManager, IVisitable
{
    //COMPONENTS
    LineRenderer lineRenderer;

    //STATS
    public Stats stats { get; private set; }
    public void Accept(IVisitor visitor) => visitor.Visit(this);

    public bool drawPathLine = false;

    [Header("Battalion identifier")]
    public int battalionNumber;
    public string battalionName;

    [Header("Battalion formation")]
    public BattalionTemplate battalionTemplate;
    private Formation _battalionFormation;
    public Formation battalionFormation
    {
        get => _battalionFormation;
        set
        {
            if(value != _battalionFormation)
            {
                _battalionFormation = value;
                _formationChanged = true;
            }
        }
    }
    private bool _formationChanged = false;
    public int battallionSize { get{return battalionTemplate.companies.Length;} }

    [Header("Battalion movement")]
    public float Speed { get { return stats.Speed; } }
    public const float RunMultiplier = 1.5f;

    [Header("Battalion state")]
    public bool FireAll = true;
    private FiniteStateMachine captainStateMachine;
    public string stateName;
    public int Morale;
    public int FleeThreashold = 25;

    [Header("Companies")]
    public List<OfficerManager> companies = new List<OfficerManager>();

    [Header("Debug")]
    public bool ShowSightLines = false;
    public bool ShowFormation = false;

    //INITIALIZE
    private void Awake()
    {
        Initialize();
    }
    void Start()
    {
        InitializeFormation();

        SpawnCompanyPawns();
    }

    public override void Initialize()
    {
        ms = GetComponent<MeshRenderer>();
        um = GetComponent<CaptainMovement>();

        lineRenderer = GetComponent<LineRenderer>();

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

        Utility.Camera.GetComponent<UIManager>().AppendBattalionFlag(this);

        if (Utility.Camera.GetComponent<CameraManager>().faction == faction)
        {
            Utility.Camera.GetComponent<UIManager>().AddBattalionCard(this);
        }

        captainStateMachine = new FiniteStateMachine();
        StateMachineInitializer();
    }
    private void InitializeFormation()
    {
        Formation companyFormation = new Line(battalionTemplate.companies[1].CompanySize);

        battalionFormation = new Line((int)battallionSize);
        battalionFormation.SetSizeByRanks(battallionSize, 1);
        battalionFormation.a = companyFormation.a * companyFormation.Lines + 2f;
    }

    public float GetBattalionWidth(float space)
    {
        float width = 0f;
        
        for(int r = 0; r < battalionFormation.Ranks; r++)
        {
            float w = 0f;
            for (int i = 0; i < battalionFormation.Lines; i++)
            {
                OfficerManager m = companies[i];
                w += m.companyFormation.Lines * m.companyFormation.a;
            }
            if(w > width)
            {
                width = w;
            }
        }
        
        width += space * (battalionFormation.Lines - 1);

        return width;
    }

    private void SpawnCompanyPawns()
    {
        for (int i = 0; i < battallionSize; i++)
        {
            Vector2 v2 = GetFormationCoords(i);
            SpawnOfficer(Utility.V2toV3(v2) + transform.position, i);
        }
    }
    private void SpawnOfficer(Vector3 pos, int localCompanyIndex)
    {
        GameObject officer = Instantiate(battalionTemplate.companies[localCompanyIndex].officerPrefab);
        officer.transform.position = pos;
        officer.transform.rotation = transform.rotation;

        OfficerManager officerManager = officer.GetComponent<OfficerManager>();
        OfficerMovement officerMovememnt = officer.GetComponent<OfficerMovement>();

        officerManager.companyTemplate = battalionTemplate.companies[localCompanyIndex];
        officerManager.masterCaptain = this;
        officerManager.faction = faction;
        GameUtility.RegisterCompany(officerManager);
        officerManager.companyNumber = GameUtility.companiesRef.Count;

        officerManager.Initialize();

        companies.Add(officerManager);
    }

    //UPDATES
    void Update()
    {
        //STATE MACHINE
        captainStateMachine.Update();

        stateName = captainStateMachine.currentState.name;

        //UPDATE TIMER
        UpdateTimers();

        DrawPathLine();
    }
    private void UpdateTimers()
    {
        //Stats.Mediator.Update(Time.deltaTime);
    }

    private void DrawPathLine()
    {
        lineRenderer.enabled = drawPathLine;

        if(drawPathLine)
        {
            NavMeshAgent na = um.navAgent;

            lineRenderer.positionCount = na.path.corners.Length;
            lineRenderer.SetPosition(0, transform.position);

            if(na.path.corners.Length < 2)
            {
                return;
            }

            for(int i = 1; i < na.path.corners.Length; i++)
            {
                lineRenderer.SetPosition(i, na.path.corners[i] + Vector3.up * 0.5f);
            }
        }
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
    public void SendOrder(bool add, Vector2 pos, Quaternion rot)
    {
        if (add)
        {
            um.AddDestination(pos, rot);
        }
        else
        {
            um.SetDestination(pos, rot);
        }
    }

    //FORMATION MANAGEMENT
    public void SetFormation(Formation formation)
    {
        battalionFormation = formation;
    }
    public void SendFormation()
    {
        for (int i = 0; i < battallionSize; i++)
        {
            if (companies[i] != null)
                companies[i].SendOrder(
                    false,
                    GetFormationCoords(i) + Utility.V3toV2(transform.position),
                    um.CurrentRotation()
                    );
        }
    }
    public Vector2 GetFormationCoords(int ID)
    {
        Vector2 pos2 = battalionFormation.GetPos(ID) + Vector2.up * 3f;

        Vector3 pos3 = Utility.V2toV3(pos2);

        Quaternion rotation = transform.rotation;

        pos3 = rotation * pos3;

        return Utility.V3toV2(pos3);
    }
    public bool AreCompaniesIdle()
    {
        foreach(OfficerManager om in companies)
        {
            if(om != null)
            {
                if (om.stateName != "Idle")
                {
                    return false;
                }
            }
        }
        return true;
    }
    public int GetPawnRank(int ID)
    {
        return battalionFormation.GetRank(ID);
    }
    public string GetFormationType()
    {
        return battalionFormation.GetType().ToString();
    }

    //STATE MACHINE
    public State GetState()
    {
        return captainStateMachine.currentState;
    }
    private void StateMachineInitializer()
    {
        List<State> captainStates = new List<State>();
        List<Transition> captainTransitions = new List<Transition>();

        //STATES
        //ANY
        AnyState anyState = new AnyState();
        captainStates.Add( anyState );
        //IDLE
        State Idle = new State(
                "Idle",
                () => {
                    if (_formationChanged)
                    {
                        SendOrder(false, Utility.V3toV2(transform.position), transform.rotation);
                        SendFormation();
                        _formationChanged = false;
                    }
                },
                null,
                null
            );
        captainStates.Add(Idle);
        //FLEEING
        State Fleeing = new State(
                "Fleeing",
                null,
                null,
                null
            );
        captainStates.Add(Fleeing);
        //MOVING
        State Moving = new State(
                "Moving",
                null,
                null,
                () => {
                    //MOVEMENT BEHAVIOUR
                    ((CaptainMovement)um).UpdateMovement();

                    //COMPANY MOVEMENT
                    SendFormation();
                }
            );
        captainStates.Add(Moving);
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
        captainTransitions.Add(MovingIdle);


        //TRANSITIONS
        //ANY -> FLEEING
        Transition AnyFleeing = new Transition(
                anyState,
                Fleeing,
                () => {
                    return false;
                }
            );
        captainTransitions.Add(AnyFleeing);
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
        captainTransitions.Add(IdleMoving);


        captainStateMachine.AddStates(captainStates);
        captainStateMachine.AddTransitions(captainTransitions);

        captainStateMachine.initialState = Idle;

        captainStateMachine.Initialize();
    }

    
    //GIZMOS
    public void OnDrawGizmos()
    {
        if (ShowFormation)
            FormationGizmo();
    }
    private void FormationGizmo()
    {
        for (int i = 0; i < battallionSize; i++)
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Vector3 FormationSlot = Utility.V2toV3(GetFormationCoords(i)) + transform.position;
            Gizmos.DrawSphere(FormationSlot, 0.2f);
        }
    }
}