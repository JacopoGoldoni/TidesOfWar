using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryManager : UnitManager, IVisitable
{
    //COMPONENTS
    LineRenderer lineRenderer;

    //STATS
    [SerializeField] CompanyTemplate unitTemplate;
    [SerializeField] ArtilleryClass cannonTemplate;
    public Stats stats { get; private set; }

    public void Accept(IVisitor visitor) => visitor.Visit(this);

    public bool drawPathLine = false;

    public bool Loaded = true;
    private float fireRange = 0f;

    static public string fireSFXName = "MusketFire";

    AudioSource audioData;
    ParticleSystem particleSystem;

    //TIMERS
    private CountdownTimer fireTimer;
    private CountdownTimer reloadTimer;


    [Header("Crew")]
    public GameObject crewPrefab;
    public List<ArtilleryCrewManager> crew = new List<ArtilleryCrewManager>(); //USE ARTILLERY CREW MANAGER

    [Header("Regiment identifier")]
    public int RegimentNumber;
    public string RegimentName;

    [Header("Crew formation")]
    public int crewSize = 4;
    private Formation crewFormation;
    public float Range = 20f;

    [Header("Regiment combact")]
    public float Precision { get { return stats.Precision * cannonTemplate.Precision; } }
    public int Ammo;
    public int MaxAmmo { get { return unitTemplate.MaxAmmo; } }
    
    [Header("Regiment movement")]
    public float Speed { get { return stats.Speed; } }
    public const float RunMultiplier = 1.5f;

    [Header("Abilities")]
    public bool Fortification { get { return unitTemplate.Fortification; } }

    [Header("Regiment state")]
    public bool FireAll = true;
    private FiniteStateMachine artilleryStateMachine;
    public string stateName;
    public int Morale;
    public int FleeThreashold = 25;

    [Header("Debug")]
    public bool ShowSightLines = false;
    public bool ShowFormation = false;

    void Start()
    {
        Initialize();

        SpawnRegimentPawns();
    }

    public override void Initialize()
    {
        ms = GetComponent<MeshRenderer>();
        um = GetComponent<ArtilleryMovement>();

        audioData = GetComponent<AudioSource>();
        particleSystem = GetComponentInChildren<ParticleSystem>();

        Material m = Instantiate(UnitMaterial);

        if (Utility.Camera.GetComponent<CameraManager>().faction == faction)
        {
            m.SetColor("_Color", Color.green);
        }
        else
        {
            m.SetColor("_Color", Color.red);
        }

        ms.material = m;

        crewFormation = new Column(crewSize);
        crewFormation.SetSizeByLines(crewSize, 2);
        crewFormation.a = 1.25f;

        artilleryStateMachine = new FiniteStateMachine();

        StateMachineInitializer();
    }

    public void MoveTo(Vector2 dest, Quaternion quat)
    {
        um.SetDestination(dest, quat);
    }

    //CREW
    private void SpawnRegimentPawns()
    {
        for (int i = 0; i < crewSize; i++)
        {
            Vector2 v2 = GetFormationCoords(i);
            SpawnPawn(Utility.V2toV3(v2) + transform.position);
        }
    }
    private void SpawnPawn(Vector3 pos)
    {
        GameObject crewMember = Instantiate(crewPrefab);
        crewMember.transform.position = pos;
        crewMember.transform.rotation = transform.rotation;
        crewMember.transform.localScale = Vector3.one;

        
        ArtilleryCrewManager crewManager = crewMember.GetComponent<ArtilleryCrewManager>();
        ArtilleryCrewMovement crewMovement = crewManager.GetComponent<ArtilleryCrewMovement>();

        crewManager.masterArtillery = this;
        crewManager.ID = crew.Count;
        crewManager.faction = faction;

        //crewMovement.MovementSpeed = Speed + 1;

        crewManager.name = "ArtilleryRegiment" + RegimentNumber + "_" + crewManager.ID;

        crewManager.Initialize();

        crew.Add(crewManager);
    }


    public void Update()
    {
        if (reloadTimer != null)
        {
            reloadTimer.Tick(Time.deltaTime);
        }

        if (fireTimer != null)
        {
            fireTimer.Tick(Time.deltaTime);
        }
    }

    public void Die()
    {
        Destroy(transform.gameObject);
    }

    private Vector2 GetFormationCoords(int ID)
    {
        Vector2 pos2 = crewFormation.GetPos(ID);

        pos2.y *= -0.5f;

        Vector3 pos3 = Utility.V2toV3(pos2);

        Quaternion rotation = transform.rotation;

        pos3 = rotation * pos3;

        return Utility.V3toV2(pos3);
    }

    //FIRE
    private void CallFire()
    {
        fireTimer = new CountdownTimer(Random.Range(0f, 2f));
        fireTimer.OnTimerStop = Fire;
        fireTimer.Start();
    }
    private void Fire()
    {
        //UNLOAD GUN
        Loaded = false;

        //PLAY AUDIO CLIP
        audioData.clip = SFXUtility.GetAudio(fireSFXName + Random.Range(1, 4));
        audioData.Play(0);

        //PLAY PARTCLE EFFECT
        particleSystem.Play();

        //TRACE FOR HIT
        Transform muzzleTransform = transform.GetChild(1).transform;

        Vector3 FireDirection = transform.forward;

        //OfficerManager target = masterOfficer.targetRegiment;

        //float targetWidth = target.RegimentFormation.Lines * target.RegimentFormation.a;
        //Vector3 targetPos = target.transform.position;
        //Vector3 targetRight = target.transform.right;

        //float s = (float)(ID + 1) / masterOfficer.RegimentFormation.Lines;

        //FireDirection = (targetPos - targetRight * (targetWidth * (s - 0.5f))) - transform.position;
        //FireDirection.Normalize();

        //float precision = masterOfficer.Precision;

        //FireDirection = Quaternion.Euler(Random.Range(-precision / 2f, precision / 2f), Random.Range(-precision, precision), 0f) * FireDirection;

        Ray ray = new Ray(muzzleTransform.position, FireDirection * fireRange);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * fireRange, Color.red, 2f);
        if (Physics.Raycast(ray, out hit))
        {
            PawnManager pm = hit.transform.GetComponent<PawnManager>();
            if (pm != null)
            {
                //KILL PAWN
                pm.Die();
            }
        }
    }

    //RELOAD
    private void CallReload()
    {
        reloadTimer = new CountdownTimer(10f);
        reloadTimer.OnTimerStop = Reload;
        reloadTimer.Start();
    }
    private void Reload()
    {
        Loaded = true;
    }

    //STATE MACHINE
    private void StateMachineInitializer()
    {
        List<State> artilleryStates = new List<State>();
        List<Transition> artilleryTransitions = new List<Transition>();

        //STATES
        //ANY
        AnyState anyState = new AnyState();
        artilleryStates.Add(anyState);
        //IDLE
        State Idle = new State(
                "Idle",
                null,
                null,
                null
            );
        artilleryStates.Add(Idle);
        //MOUNT
        State Mount = new State(
                "Mount",
                null,
                null,
                null
            );
        artilleryStates.Add(Mount);
        //DISMOUNT
        State Dismount = new State(
                "Mount",
                null,
                null,
                null
            );
        artilleryStates.Add(Dismount);
        //MOUNTED
        State Mounted = new State(
                "Mounted",
                null,
                null,
                null
            );
        artilleryStates.Add(Dismount);
        //MOVE
        State Move = new State(
                "Move",
                null,
                null,
                null
            );
        artilleryStates.Add(Move);
        //FLEE
        State Fleeing = new State(
                "Fleeing",
                null,
                null,
                null
            );
        artilleryStates.Add(Fleeing);

        //TRANSITIONS
        //ANY -> FLEEING
        Transition AnyFleeing = new Transition(
                anyState,
                Fleeing,
                () => {
                    if (Morale < FleeThreashold)
                    {
                        return true;
                    }
                    return false;
                }
            );
        artilleryTransitions.Add(AnyFleeing);
        //IDLE -> MOUNT
        Transition IdleMount = new Transition(
                Idle,
                Mount,
                () => {
                    return false;
                }
            );
        artilleryTransitions.Add(IdleMount);
        //MOUNT -> MOUNTED
        Transition MountMounted = new Transition(
                Mount,
                Mounted,
                () => {
                    return false;
                }
            );
        artilleryTransitions.Add(MountMounted);
        //MOUNTED -> DISMOUNT
        Transition MountedDismount = new Transition(
                Mounted,
                Dismount,
                () => {
                    return false;
                }
            );
        artilleryTransitions.Add(MountedDismount);
        //DISMOUNT -> IDLE
        Transition DismountIdle = new Transition(
                Dismount,
                Idle,
                () => {
                    return false;
                }
            );
        artilleryTransitions.Add(DismountIdle);
        //MOUNTED -> MOVE
        Transition MountedMove = new Transition(
                Mounted,
                Move,
                () => {
                    return false;
                }
            );


        artilleryStateMachine.AddStates(artilleryStates);
        artilleryStateMachine.AddTransitions(artilleryTransitions);

        artilleryStateMachine.initialState = Idle;

        artilleryStateMachine.Initialize();
    }

    //GIZMOS
    public void OnDrawGizmos()
    {
        if (ShowFormation)
            FormationGizmo();
    }
    private void FormationGizmo()
    {
        for (int i = 0; i < crewSize; i++)
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Vector3 FormationSlot = Utility.V2toV3(GetFormationCoords(i)) + transform.position;
            Gizmos.DrawSphere(FormationSlot, 0.2f);
        }
    }
}