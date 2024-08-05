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
    private CountdownTimer mountTimer = new CountdownTimer(5f);
    private CountdownTimer dismountTimer = new CountdownTimer(5f);

    public ArtilleryOfficerManager masterOfficer;

    [Header("Projectile")]
    public GameObject ProjectilePrefab;
    public float projectileSpeed = 10f;

    [Header("Crew")]
    public GameObject crewPrefab;
    public List<ArtilleryCrewManager> crew = new List<ArtilleryCrewManager>();

    [Header("Artillery piece identifier")]
    public int ArtilleryLocalID;

    [Header("Crew formation")]
    public int crewSize = 4;
    //private ArtilleryCrewFormation crewFormation;

    [Header("Artillery movement")]
    public bool mounted = false;

    [Header("Artillery state")]
    public bool isReloading { get { return reloadTimer.IsRunning; } }
    private FiniteStateMachine artilleryStateMachine;
    public UnitManager targetUnit { get { return masterOfficer.targeUnit; } }
    public string stateName;

    [Header("Debug")]
    public bool ShowFormation = false;
    public bool ShowFiringLine = false;
    public bool ShowCrewFormation = false;


    public override void Initialize()
    {
        smr = GetComponentInChildren<SkinnedMeshRenderer>();
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

        smr.sharedMesh = masterOfficer.artilleryBatteryTemplate.CannonMesh;
        smr.material = m;

        //CREW FORMATION INITALIZATION

        //FINITE STATE MACHINE INITALIZATION
        artilleryStateMachine = new FiniteStateMachine();
        StateMachineInitializer();

        InitializeTimers();

        SpawnCrew();

        projectileSpeed = Mathf.Sqrt(masterOfficer.artilleryBatteryTemplate.Range * 9.81f);
    }
    private void InitializeTimers()
    {
        mountTimer.OnTimerStop = () => { Mount(); };
        dismountTimer.OnTimerStop = () => { Dismount(); };
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
        transform.rotation = Quaternion.LookRotation(
            Utility.V2toV3( ( Utility.V3toV2(targetUnit.transform.position - transform.position) ) ).normalized , 
            Vector3.up);
    }
    public void AbortFire()
    {
        fireTimer.Stop();
        fireTimer.Reset();
    }
    private void Fire()
    {
        //UNLOAD GUN
        Loaded = false;
        fireTimer.Stop();

        //PLAY AUDIO CLIP
        audioData.clip = SFXUtility.GetAudio(fireSFXName + Random.Range(1, 4));
        audioData.Play(0);

        //PLAY PARTCLE EFFECT
        particleSystem.Play();

        float d = Utility.V3toV2(transform.position - targetUnit.transform.position).magnitude;
        float angleElevation = -0.5f * Mathf.Asin(9.81f * d / Mathf.Pow(projectileSpeed, 2));
        float t = 2 * projectileSpeed * Mathf.Sin(angleElevation) / 9.81f;
        angleElevation *= Mathf.Rad2Deg;
        angleElevation += Random.Range(-5f, +5f);
        float maxAngleDir = Mathf.Atan( (targetUnit.GetWidth() / 2f) / d) * Mathf.Rad2Deg;

        //TRACE FOR HIT
        Transform muzzleTransform = transform.GetChild(0).transform;

        Vector3 FireDirection = transform.forward;
        FireDirection = Quaternion.AngleAxis(angleElevation, transform.right) * FireDirection;
        FireDirection = Quaternion.AngleAxis(Random.Range(-maxAngleDir, +maxAngleDir), transform.up) * FireDirection;

        //ARTILLERY FIRE MECHANICS
        GameObject projectile = Instantiate(ProjectilePrefab);
        projectile.transform.position = muzzleTransform.position;
        projectile.transform.rotation = muzzleTransform.rotation;
        projectile.GetComponent<Rigidbody>().velocity = FireDirection * projectileSpeed;
        projectile.name = "ArtilleryProjectile";

        projectile.GetComponent<ArtilleryBulletManager>().Initialize(t * 2f);

        Debug.DrawLine(transform.position, transform.position + FireDirection, Color.red, 1f);
    }

    //RELOAD
    public void CallReload()
    {
        reloadTimer = new CountdownTimer(masterOfficer.artilleryBatteryTemplate.ReloadTime);
        reloadTimer.OnTimerStop = Reload;
        reloadTimer.Start();
    }
    public void AbortReloading()
    {
        reloadTimer.Stop();
        reloadTimer.Reset();
    }
    private void Reload()
    {
        Loaded = true;
        reloadTimer.Stop();

        masterOfficer.Ammo--;
        masterOfficer.artilleryBatteryCardManager.SetAmmoSlide();
    }

    //MOUNT & DISMOUNT
    private void Mount()
    {
        mounted = true;
        transform.transform.localScale = new Vector3(1, 1, -1);
    }
    private void Dismount()
    {
        mounted = false;
        transform.transform.localScale = new Vector3(1, 1, 1);
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
        //MOVING
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
        //MOUNTING
        State Mounting = new State(
                "Mounting",
                () =>
                {
                    mountTimer.Reset();
                    mountTimer.Start();
                },
                null,
                () =>
                {
                    mountTimer.Tick(Time.deltaTime);
                }
            );
        artilleryStates.Add(Mounting);
        //DISMOUNTING
        State Dismounting = new State(
                "Dismounting",
                () =>
                {
                    dismountTimer.Reset();
                    dismountTimer.Start();
                },
                null,
                () =>
                {
                    dismountTimer.Tick(Time.deltaTime);
                }
            );
        artilleryStates.Add(Dismounting);
        //FIRING
        State Firing = new State(
                "Firing",
                () => {
                    CallFire();
                },
                null,
                null
            );
        artilleryStates.Add(Firing);
        //RELOADING
        State Reloading = new State(
                "Reloading",
                () => {
                    CallReload();
                },
                null,
                null
            );
        artilleryStates.Add(Reloading);

        //TRANSITIONS
        //IDLE -> MOUNTING
        Transition IdleMounting = new Transition(
                Idle,
                Mounting,
                () => {
                    return um.MovementPoints.Count != 0;
                }
            );
        artilleryTransitions.Add(IdleMounting);
        //MOUNTING -> MOVING
        Transition MountingMoving = new Transition(
                Mounting,
                Moving,
                () => {
                    return mounted;
                }
            );
        artilleryTransitions.Add(MountingMoving);
        //MOVING -> DISMOUNTING
        Transition MovingDismounting = new Transition(
                Moving,
                Dismounting,
                () => {
                    return um.MovementPoints.Count == 0;
                }
            );
        artilleryTransitions.Add(MovingDismounting);
        //DISMOUNTING -> IDLE
        Transition DismountingIdle = new Transition(
                Dismounting,
                Idle,
                () => {
                    return !mounted;
                }
            );
        artilleryTransitions.Add(DismountingIdle);
        //IDLE -> FIRING
        Transition IdleFiring = new Transition(
                Idle,
                Firing,
                () => {
                    //HAS TARGET AND HAS NO MOVEMENT AND IS LOADED
                    return targetUnit != null && um.MovementPoints.Count == 0 && Loaded;
                }
            );
        artilleryTransitions.Add(IdleFiring);
        //FIRING -> IDLE
        Transition FiringIdle1 = new Transition(
                Firing,
                Idle,
                () => {
                    //HAS NO TARGET OR IS UNLOADED
                    return targetUnit == null || !Loaded;
                }
            );
        artilleryTransitions.Add(FiringIdle1);
        //FIRING -> IDLE WITH ABORT
        Transition FiringIdle2 = new Transition(
                Firing,
                Idle,
                () => {
                    //ABORT FIRE
                    AbortFire();
                },
                () => {
                    //HAS MOVEMENT
                    return um.MovementPoints.Count != 0;
                }
            );
        artilleryTransitions.Add(FiringIdle2);
        //IDLE -> RELOADING
        Transition IdleReloading = new Transition(
                Idle,
                Reloading,
                () => {
                    //IS NOT LOADED AND HAS NO MOVEMENT
                    return !Loaded && um.MovementPoints.Count == 0 && masterOfficer.Ammo > 0;
                }
            );
        artilleryTransitions.Add(IdleReloading);
        //RELOADING -> IDLE
        Transition ReloadingIdle1 = new Transition(
                Reloading,
                Idle,
                () => {
                    //IS LOADED
                    return Loaded;
                }
            );
        artilleryTransitions.Add(ReloadingIdle1);
        //RELOADING -> IDLE WITH ABORT
        Transition ReloadingIdle2 = new Transition(
                Reloading,
                Idle,
                () => {
                    //ABORT RELOADING
                    AbortReloading();
                },
                () => {
                    //HAS MOVEMENT
                    return um.MovementPoints.Count != 0;
                }
            );
        artilleryTransitions.Add(ReloadingIdle2);


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

    public override float GetWidth()
    {
        throw new System.NotImplementedException();
    }
    public override float GetLenght()
    {
        throw new System.NotImplementedException();
    }

    //GIZMOS
    public void OnDrawGizmos()
    {
        //if (ShowFormation)
            //FormationGizmo();
    }

    public override void OnSelection()
    {
        throw new System.NotImplementedException();
    }
    public override void OnDeselection()
    {
        throw new System.NotImplementedException();
    }
}