using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Splines.SplineInstantiate;

public class OfficerManager : UnitManager, IVisitable
{
    //COMPONENTS
    LineRenderer lineRenderer;

    //STATS
    [SerializeField] public CompanyTemplate companyTemplate;
    public Stats stats { get; private set; }
    public void Accept(IVisitor visitor) => visitor.Visit(this);


    public CaptainManager masterCaptain;

    [Header("Pawns")]
    public GameObject pawnPrefab;
    public List<PawnManager> pawns = new List<PawnManager>();

    [Header("Company identifier")]
    public int companyNumber;
    public string companyName;

    [Header("Company formation")]
    public int companySize = 120;
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

    //INITIALIZE
    public override void Initialize()
    {
        //GET COMPONENTS
        ms = GetComponent<MeshRenderer>();
        um = GetComponent<OfficerMovement>();
        lineRenderer = GetComponent<LineRenderer>();

        //SET MATERIAL
        InitializeMaterial();

        //APPEND COMPANY FLAG
        Utility.Camera.GetComponent<UIManager>().AppendCompanyFlag(this);
        //APPEND COMPANY CARD
        if (Utility.Camera.GetComponent<CameraManager>().faction == faction)
        {
            Utility.Camera.GetComponent<UIManager>().AddCompanyCard(this);
        }

        InitializeStats();
        InitializeFormation();
        SpawnCompanyPawns();

        //INITIALIZE FINITE STATE MACHINE
        OfficerStateMachine = new FiniteStateMachine();
        FiniteStateMachineInitializer();
    }
    private void InitializeStats()
    {
        //INITIALIZE STATS
        stats = new Stats(new StatsMediator(), companyTemplate);

        um.MovementSpeed = Speed;
        companySize = companyTemplate.CompanySize;
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
    private void SpawnCompanyPawns()
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
        if(stateName != "Fleeing")
        {
            targetCompany = EnemyInRange(Range);
        }

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

    //UTILITY METHODS
    private void DrawPathLine()
    {
        lineRenderer.enabled = drawPathLine;

        if (drawPathLine)
        {
            NavMeshAgent na = um.navAgent;

            lineRenderer.positionCount = na.path.corners.Length;
            lineRenderer.SetPosition(0, transform.position);

            if (na.path.corners.Length < 2)
            {
                return;
            }

            for (int i = 1; i < na.path.corners.Length; i++)
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
    private void CalculateMorale()
    {
        int n = 0;

        foreach(PawnManager p in pawns)
        {
            if(p == null)
            {
                n++;
            }
        }

        float s = (float)(pawns.Count - n) / (float)pawns.Count;

        Morale = (int)(s * companyTemplate.BaseMorale);
    }
    private Bounds CalculateCompanyBounds()
    {
        Vector3 p;
        Vector3 s;

        p = new Vector3(
            0, 
            0,
            -1 * companyFormation.Ranks * companyFormation.b / 2);
        s = new Vector3(
            companyFormation.Lines * companyFormation.a,
            1f,
            companyFormation.Ranks * companyFormation.b);

        return new Bounds(p, s);
    }
    public bool IsDetached()
    {
        return (masterCaptain == null);
    }

    //FORMATION MANAGEMENT
    private void CheckFormation()
    {
        //PAWNS DIED
        List<int> indexes = new List<int>();

        int i = 0;
        foreach (PawnManager pm in pawns)
        {
            if (pawns[i] == null && GetPawnRank(i) == 1)
            {
                indexes.Add(i);
            }

            i++;
        }

        if(indexes.Count == 0)
        {
            return;
        }

        if(companySize - indexes.Count < companyFormation.Lines)
        {
            return;
        }

        _formationChanged = true;
        int Lines = companyFormation.Lines;

        foreach (int l in indexes)
        {
            int k = 0;
            int n = 1;
            while (true)
            {
                int s = (k % 2) * ((k+1) / 2) + (-1) * ((k+1) % 2) * (k / 2);
                int m = l + n * Lines + s;

                if (m >= pawns.Count) { 
                    for(int z = companyFormation.Lines; z < pawns.Count; z ++)
                    {
                        if (pawns[z] != null)
                        {
                            pawns[l] = pawns[z];
                            pawns[l].ID = l;
                            pawns[z] = null;
                            break;
                        }
                    }
                }

                if ((k + l % Lines) >= Lines)
                {
                    k = 0;
                    s = (k % 2) * ((k + 1) / 2) + (-1) * ((k + 1) % 2) * (k / 2);
                    n++;
                }

                m = l + n * Lines + s;
                if (m >= pawns.Count) {
                    for (int z = companyFormation.Lines; z < pawns.Count; z++)
                    {
                        if (pawns[z] != null)
                        {
                            pawns[l] = pawns[z];
                            pawns[l].ID = l;
                            pawns[z] = null;
                            break;
                        }
                    }
                    break;
                }

                if (pawns[m] != null)
                {
                    pawns[l] = pawns[m];
                    pawns[l].ID = l;
                    pawns[m] = null;
                    break;
                }
                else
                {
                    k++;
                }
            }
        }
    }
    public void SetFormation(Formation formation)
    {
        companyFormation = formation;
        companyBounds = CalculateCompanyBounds();
    }
    public void SendFormation()
    {
        for (int i = 0; i < companySize; i++)
        {
            if (pawns[i] != null)
            pawns[i].MoveTo(
                GetFormationCoords(i) + Utility.V3toV2(transform.position),
                um.CurrentRotation()
                );
        }
    }
    public Vector2 GetFormationCoords(int ID)
    {
        Vector2 pos2 = companyFormation.GetPos(ID);

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
            if(pm != null)
            {
                if (pm.um.IsMoving() && pm.um.IsRotating())
                {
                    return false;
                }
            }
        }
        return true;
    }
    public int GetPawnRank(int ID)
    {
        return companyFormation.GetRank(ID);
    }
    public string GetFormationType()
    {
        return companyFormation.GetType().ToString();
    }
    public bool IsObstructedAt(Vector2 pos)
    {
        Vector3 o = transform.rotation * companyBounds.center;
        Vector2 center = pos + Utility.V3toV2(o);

        //CHECK IF INTERSECT WITH COMPANY
        List<OfficerManager> allCompanies = GameUtility.GetAllCompanies();

        foreach(OfficerManager om in allCompanies)
        {
            Vector3 o1 = om.transform.rotation * om.companyBounds.center;
            Vector2 center1 = Utility.V3toV2(om.transform.position) + Utility.V3toV2(o1);

            bool coll = UtilityMath.BoxCollisionDetection( 
                new Bounds(Utility.V2toV3(center), companyBounds.size),
                transform.rotation,
                new Bounds(Utility.V2toV3(center1), om.companyBounds.size),
                om.transform.rotation);


            if(coll)
            {
                return true;
            }
        }

        return false;
    }
    public float GetCompanyWidth()
    {
        float width = (companyFormation.Lines - 1) * companyFormation.a;
        return width;
    }

    //COMPANY FIRE MANAGEMENT
    public void SendFireMessage()
    {
        for (int i = 0; i < companySize; i++)
        {
            if(GetPawnRank(i) == 1)
            {
                //FIRE ONLY FIRST RANK
                if (pawns[i] != null)
                pawns[i].CallFire();
            }
        }
        Ammo -= 1;
    }
    public void SendRelaodMessage()
    {
        for (int i = 0; i < companySize; i++)
        {
            if (GetPawnRank(i) == 1)
            {
                //FIRE ONLY FIRST RANK
                if (pawns[i] != null)
                    pawns[i].CallReload();
            }
        }
        Ammo -= 1;
    }
    public bool CheckLoadedStatus()
    {
        bool a = false;

        //TRUE IF ALL LOADED, FALSE IF AT LEAST ONE IS NOT LOADED
        for (int i = 0; i < companySize; i++)
        {
            //CHECK ONLY FIRST RANK
            if (GetPawnRank(i) == 1)
            {
                if (pawns[i] != null)
                {
                    a = true;
                    if (!pawns[i].Loaded)
                    {
                        return false;
                    }
                }
            }
        }
        return a;
    }
    public bool CheckUnLoadedStatus()
    {
        //TRUE IF ALL UNLOADED, FALSE IF AT LEAST ONE IS LOADED
        for (int i = 0; i < companySize; i++)
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
        List<OfficerManager> Units = GameUtility.GetAllCompanies();

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
        foreach(OfficerManager om in GameUtility.GetAllCompanies())
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