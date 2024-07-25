using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryManager : UnitManager, IVisitable
{
    //COMPONENTS
    LineRenderer lineRenderer;

    //STATS
    [SerializeField] public CompanyTemplate unitTemplate;
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

    public ArtilleryOfficerManager masterOfficer;

    [Header("Crew")]
    public GameObject crewPrefab;
    public List<ArtilleryCrewManager> crew = new List<ArtilleryCrewManager>();

    [Header("Artillery piece identifier")]
    public int ArtilleryLocalID;

    [Header("Crew formation")]
    public int crewSize = 4;
    //private ArtilleryCrewFormation crewFormation;

    [Header("Artillery movement")]
    public bool IsOnCarriage = false;

    [Header("Artillery state")]
    private FiniteStateMachine artilleryStateMachine;
    public string stateName;

    [Header("Debug")]
    public bool ShowFormation = false;
    public bool ShowFiringLine = false;
    public bool ShowCrewFormation = false;

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

        //CREW FORMATION INITALIZATION

        //FINITE STATE MACHINE INITALIZATION
        artilleryStateMachine = new FiniteStateMachine();
        StateMachineInitializer();

        SpawnCrew();
    }

    public void MoveTo(Vector2 dest, Quaternion quat)
    {
        um.SetDestination(dest, quat);
    }

    //CREW
    private void SpawnCrew()
    {
        for (int i = 0; i < crewSize; i++)
        {
            Vector2 v2 = GetFormationCoords(i);
            SpawnCrewPawn(Utility.V2toV3(v2) + transform.position);
        }
    }
    private void SpawnCrewPawn(Vector3 pos)
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

        crewMovement.MovementSpeed = masterOfficer.Speed + 1;

        crewManager.name = "ArtilleryRegiment" + masterOfficer.batteryNumber + "_" + ArtilleryLocalID + "_" + crewManager.ID;

        crewManager.Initialize();

        crew.Add(crewManager);
    }

    public void Update()
    {
        UpdateTimers();

        //STATE MACHINE
        artilleryStateMachine.Update();

        stateName = artilleryStateMachine.currentState.name;
    }

    //UPDATERS
    private void UpdateTimers()
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

    /*
    private Vector2 GetFormationCoords(int ID)
    {
        Vector2 pos2 = crewFormation.GetPos(ID);

        pos2.y *= -0.5f;

        Vector3 pos3 = Utility.V2toV3(pos2);

        Quaternion rotation = transform.rotation;

        pos3 = rotation * pos3;

        return Utility.V3toV2(pos3);
    }
    */

    //FIRE
    public void CallFire()
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

        //ARTILLERY FIRE MECHANICS
    }

    //RELOAD
    public void CallReload()
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
        //MOVE
        State Moving = new State(
                "Moving",
                null,
                null,
                () =>
                {
                    //MOVEMENT BEHAVIOUR
                    ((ArtilleryMovement)um).UpdateMovement();

                    //CREW MOVEMENT
                    SendFormation();
                }
            );
        artilleryStates.Add(Moving);

        //TRANSITIONS
        //IDLE -> MOVING
        Transition IdleMoving = new Transition(
                Idle,
                Moving,
                () => {
                    return um.MovementPoints.Count != 0;
                }
            );
        artilleryTransitions.Add(IdleMoving);
        //MOVING -> IDLE
        Transition MovingIdle = new Transition(
                Moving,
                Idle,
                () => {
                    return um.MovementPoints.Count == 0;
                }
            );
        artilleryTransitions.Add(MovingIdle);


        artilleryStateMachine.AddStates(artilleryStates);
        artilleryStateMachine.AddTransitions(artilleryTransitions);

        artilleryStateMachine.initialState = Idle;

        artilleryStateMachine.Initialize();
    }

    private Vector2 GetFormationCoords(int ID)
    {
        Vector2 pos2 = Vector2.up * (1 + ID);

        pos2.y *= -1;

        Vector3 pos3 = Utility.V2toV3(pos2);

        Quaternion rotation = transform.rotation;

        pos3 = rotation * pos3;

        return Utility.V3toV2(pos3);
    }

    private void SendFormation()
    {
        for (int i = 0; i < crew.Count; i++)
        {
            if (crew[i] != null)
            {
                crew[i].MoveTo(GetFormationCoords(i) + Utility.V3toV2(transform.position),um.CurrentRotation());
            } 
        }
    }

    //GIZMOS
    public void OnDrawGizmos()
    {
        //if (ShowFormation)
            //FormationGizmo();
    }
    /*
    private void FormationGizmo()
    {
        for (int i = 0; i < crewSize; i++)
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Vector3 FormationSlot = Utility.V2toV3(GetFormationCoords(i)) + transform.position;
            Gizmos.DrawSphere(FormationSlot, 0.2f);
        }
    }
    */
}