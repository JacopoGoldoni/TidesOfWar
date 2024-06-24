using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CaptainManager : UnitManager
{
    //COMPONENTS
    LineRenderer lineRenderer;

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
    public float Speed = 1.5f;
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
    public bool ShowBattalionFormation = false;
    public bool ShowCompanyFormations = false;

    private void Awake()
    {
        Initialize();
    }

    //INITALIZE METHODS
    public override void Initialize()
    {
        //GET COMPONENTS
        ms = GetComponent<MeshRenderer>();
        um = GetComponent<CaptainMovement>();
        lineRenderer = GetComponent<LineRenderer>();

        //SET MATERIAL
        InitializeMaterial();

        //APPEND BATTALION FLAG
        Utility.Camera.GetComponent<UIManager>().AppendBattalionFlag(this);
        //APPEND BATTALION CARD
        if (Utility.Camera.GetComponent<CameraManager>().faction == faction)
        {
            Utility.Camera.GetComponent<UIManager>().AddBattalionCard(this);
        }

        InitializeFormation();
        SpawnCompanies();

        //INITALIZE FINITE STATE MACHINE
        captainStateMachine = new FiniteStateMachine();
        FiniteStateMachineInitializer();
    }
    private void InitializeFormation()
    {
        Formation companyFormation = new Line(battalionTemplate.companies[1].CompanySize);

        battalionFormation = new Line((int)battallionSize);
        battalionFormation.SetSizeByRanks(battallionSize, 1);
        battalionFormation.a = companyFormation.a * companyFormation.Lines + 2f;
    }

    //SPAWN CONTROLLED COMPANIES
    private void SpawnCompanies()
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

        //SET GIZMOS VISIBILITY
        officerManager.ShowFormation = ShowCompanyFormations;

        companies.Add(officerManager);
    }

    //UPDATES METHODS
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
        //stats.Mediator.Update(Time.deltaTime);
    }

    //UTILITY METHODS
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
    public float AvarageMorale()
    {
        float avarageMorale = 0f;
        
        foreach(OfficerManager om in companies)
        {
            avarageMorale += om.Morale;
        }

        avarageMorale /= companies.Count;

        return avarageMorale;
    }
    public int GetSize()
    {
        int size = 0;

        foreach(OfficerManager company in companies)
        {
            size += company.companySize;
        }

        return size;
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
            {
                companies[i].ReceiveMovementOrder(
                    false,
                    GetFormationCoords(i) + Utility.V3toV2(transform.position),
                    um.CurrentRotation()
                    );
            }
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
    public float GetBattalionWidth(float space)
    {
        //GET LARGEST company
        float w = 0f;
        for(int i = 0; i < companies.Count; i++)
        {
            float l = companies[i].GetCompanyWidth();
            if(l > w)
            {
                w = l;
            }
        }

        float width = w * battalionFormation.Lines + (battalionFormation.Lines - 1) * space;

        return width;
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
    public int GetCompanyRank(int localID)
    {
        return battalionFormation.GetRank(localID);
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
    private void FiniteStateMachineInitializer()
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
                        ReceiveMovementOrder(false, Utility.V3toV2(transform.position), transform.rotation);
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
        if (ShowBattalionFormation)
            FormationGizmo();
    }
    private void FormationGizmo()
    {
        if (battalionFormation == null) { return; }
        for (int i = 0; i < battallionSize; i++)
        {
            Gizmos.color = new Color(0, 0, 1, 0.5f);
            Vector3 FormationSlot = Utility.V2toV3(GetFormationCoords(i)) + transform.position;
            Gizmos.DrawSphere(FormationSlot, companies[i].GetCompanyWidth() / 2);
        }
    }
}