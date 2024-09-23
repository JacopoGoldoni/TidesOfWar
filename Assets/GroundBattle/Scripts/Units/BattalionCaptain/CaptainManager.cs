using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public partial class CaptainManager : UnitManager
{
    //COMPONENTS
    LineRenderer lineRenderer;

    public bool drawPathLine = false;

    [Header("Battalion identifier")]
    public int battalionNumber;
    public string battalionName;

    [Header("Battalion formation")]
    public BattalionTemplate battalionTemplate;
    public BattalionFormation battalionFormation;
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
    public bool InitializationDebug = false;
    public bool ShowBattalionFormation = false;
    public bool ShowCompanyFormations = false;
    public bool ShowCompaniesSightLines = false;

    //INITALIZE METHODS
    public override void Initialize()
    {
        //GET COMPONENTS
        um = GetComponent<CaptainMovement>();
        lineRenderer = GetComponent<LineRenderer>();

        InitializeMeshes();
        InitializeMaterial();
        
        InitializeFormation();
        GroundBattleUtility.RegisterBattallion(this);

        //APPEND BATTALION FLAG
        Utility.Camera.GetComponent<UIManager>().AppendBattalionFlag(this);
        //APPEND BATTALION CARD
        if (TAG == Utility.CameraManager.TAG)
        {
            Utility.Camera.GetComponent<UIManager>().AddBattalionCard(this);
        }

        //INITALIZE FINITE STATE MACHINE
        FiniteStateMachineInitializer();
    }
    private void InitializeFormation()
    {
        //INITIALIZE FORMATIO  PARAMETERS
        battalionFormation = new BattalionFormation(battalionTemplate);
        battalionFormation.spaceX = battalionTemplate.CompanyXDistance;
        battalionFormation.spaceY = battalionTemplate.CompanyYDistance;

        for (int i = 0; i < battallionSize; i++)
        {
            OfficerManager om = SpawnOfficer(transform.position, i);
            battalionFormation.AddCompany(om);
        }

        battalionFormation.CalculateAllPositions();

        for(int i = 0; i < companies.Count; i++)
        {
            Vector3 worldPos = GroundBattleUtility.GetMapPosition(GetFormationCoords(companies[i]) + Utility.V3toV2(transform.position));
            companies[i].transform.position = worldPos;
            companies[i].SpawnCompanyPawns();
        }

        UpdateFire();
    }

    //SPAWN CONTROLLED COMPANIES
    private OfficerManager SpawnOfficer(Vector3 pos, int localCompanyIndex)
    {
        if(InitializationDebug)
        {
            Debug.Log("Spawning officer: " + localCompanyIndex);
        }

        //INSTANTIATE OFFICER
        GameObject officer = Instantiate(battalionTemplate.companies[localCompanyIndex].officerPrefab);
        officer.transform.position = pos;
        officer.transform.rotation = transform.rotation;

        //GET COMPONENTS
        OfficerManager officerManager = officer.GetComponent<OfficerManager>();
        OfficerMovement officerMovememnt = officer.GetComponent<OfficerMovement>();

        //APPEND TO BATTALION COMAPNIES
        companies.Add(officerManager);

        //LOAD INFOS INTO OFFICER
        officerManager.companyTemplate = battalionTemplate.companies[localCompanyIndex];
        officerManager.masterCaptain = this;
        officerManager.TAG = TAG;
        officerManager.companyNumber = GroundBattleUtility.companiesRef.Count;
        officerManager.meshes_LODs = battalionTemplate.companies[localCompanyIndex].OfficerMesh_LODS;
        officerManager.unitMaterial = battalionTemplate.companies[localCompanyIndex].officerMaterial;

        //NAME OFFICER
        officer.name = "Officer_" + officerManager.companyNumber;

        //SET GIZMOS VISIBILITY
        officerManager.ShowFormation = ShowCompanyFormations;
        officerManager.ShowSightLines = ShowCompaniesSightLines;

        //WARP AGENT TO NAVMESH
        GroundBattleUtility.WarpAgentToNavMesh(officer);

        officerManager.Initialize();

        //DEBUG
        if (InitializationDebug)
        {
            Debug.Log("Company spawned.");
        }

        return officerManager;
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

    //STATE MACHINE
    public State GetState()
    {
        return captainStateMachine.currentState;
    }
    private void FiniteStateMachineInitializer()
    {
        captainStateMachine = new FiniteStateMachine();

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

    public override void OnSelection()
    {
        foreach(OfficerManager om in companies)
        {
            om.OnSelection();
        }
    }
    public override void OnDeselection()
    {
        foreach (OfficerManager om in companies)
        {
            om.OnDeselection();
        }
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
            Vector3 FormationSlot = Utility.V2toV3(GetFormationCoords(companies[i])) + transform.position;
            Gizmos.DrawSphere(FormationSlot, companies[i].GetWidth() / 2);
        }
    }
}