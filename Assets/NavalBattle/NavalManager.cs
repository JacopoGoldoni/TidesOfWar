using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavalManager : MonoBehaviour
{
    //COMPONENTS
    NavalMovement navalMovement;
    MeshRenderer meshRenderer;

    //ARMAMENT POSTS
    public Transform[] posts; 

    //SHIP STATS
    public ShipClass shipClass;
    private Armament[] armaments;
    int hull;

    public string TAG = "FRA";
    public Vector2 target;
    public bool move = false;
    
    //FINITE STATE MACHINE
    FiniteStateMachine fsm = new FiniteStateMachine();

    private void Awake()
    {
        navalMovement = GetComponent<NavalMovement>();

        hull = shipClass.max_hull;

        Initialize();

        InitializeStateMachine();
    }

    private void Initialize()
    {
        //SHIP PREFAB INITIALIZE
        GameObject shipObject = Instantiate(shipClass.shipPrefab);
        shipObject.transform.parent = transform;
        shipObject.transform.localPosition = Vector3.zero;
        shipObject.transform.localEulerAngles = Vector3.zero;

        GameObject shipBody = shipObject.transform.GetChild(0).gameObject;
        //SHIP COLLIDER
        Bounds shipBounds = shipBody.GetComponent<MeshFilter>().mesh.bounds;
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.center = shipBounds.center * 100f;
        boxCollider.size = shipBounds.size * 100f;

        //SHIP MATERIAL
        meshRenderer = shipObject.GetComponentInChildren<MeshRenderer>();
        meshRenderer.material = shipClass.shipMaterial;
        if(Utility.Camera.GetComponent<NavalBatleCameraManager>().TAG == TAG)
        {
            meshRenderer.material.SetColor("_BaseColor", Color.green);
        }
        else
        {
            meshRenderer.material.SetColor("_BaseColor", Color.red);
        }

        //FIND POSTS
        posts = new Transform[shipObject.transform.childCount - 1];
        for(int i = 0; i < shipObject.transform.childCount; i++)
        {
            Transform post = shipObject.transform.GetChild(i);

            if(post.gameObject.name.Substring(0,4) == "Post")
            {
                char c = post.gameObject.name[post.gameObject.name.Length - 1];
                int j = Convert.ToInt32(new string(c,1));
                posts[j - 1] = post;
            }
        }

        //INSTANTIATE ARMAMENTS
        armaments = new Armament[shipObject.transform.childCount - 1];
        for (int i = 0; i < posts.Length; i++)
        {
            GameObject armament_object = Instantiate(shipClass.shipArmament[0].armamentPrefab);
            armament_object.transform.position = posts[i].position;
            armament_object.transform.rotation = posts[i].rotation;
            armament_object.transform.parent = shipObject.transform;

            armaments[i] = new Armament(posts[i], armament_object.transform, 0, shipClass);
        }
    }

    // Update is called once per frame
    void Update()
    {
         fsm.Update();
    }

    private void InitializeStateMachine()
    {
        List<State> shipStates = new List<State>();
        List<Transition> shipTransitions = new List<Transition>();

        //STATES
        AnyState anyState = new AnyState();
        shipStates.Add(anyState);
        State Idle = new State(
                    "Idle",
                    () => { navalMovement.Stop(); },
                    () => { },
                    () => { }
                );
        shipStates.Add(Idle);
        State MoveTo = new State(
                    "MoveTo",
                    null,
                    null,
                    () => {
                        navalMovement.Seek(target); 
                    }
                );
        shipStates.Add(MoveTo);
        State SeekTarget = new State(
                    "SeekTarget",
                    () => { },
                    () => { },
                    () => { }
                );
        shipStates.Add(SeekTarget);
        State FleeTarget = new State(
                    "FleeTarget",
                    () => { },
                    () => { },
                    () => { }
                );
        shipStates.Add(FleeTarget);
        State Patrol = new State(
                    "Patrol",
                    () => { },
                    () => { },
                    () => { }
                );
        shipStates.Add(Patrol);

        //TRANSITIONS
            //IDLE -> SEEKTARGET
        Transition IdleMoveTo = new Transition(
                Idle,
                MoveTo,
                () => {
                    return move;
                }
            );
        shipTransitions.Add(IdleMoveTo);

        fsm.AddStates(shipStates);
        fsm.AddTransitions(shipTransitions);

        fsm.initialState = Idle;

        fsm.Initialize();
    }

    class Armament
    {
        public Transform rest;
        public Transform transform;
        public int armamentIndex;

        private ShipClass shipClassRef;

        public bool loaded;

        private CountdownTimer timer;

        public Armament(Transform rest, Transform transform, int armamentIndex, ShipClass shipClass)
        {
            this.rest = rest;
            this.transform = transform;
            this.armamentIndex = armamentIndex;
            this.loaded = true;

            shipClassRef = shipClass;

            timer = new CountdownTimer(shipClassRef.shipArmament[armamentIndex].reloadTime);
            timer.OnTimerStop = () => 
            { 
                loaded = true;
            };
        }

        public void Fire()
        {
            //TODO:FIRE

            loaded = false;
            timer.Reset();
            timer.Start();
        }

        public void Rotate(Quaternion target)
        {
            transform.rotation = target;
        }
        public void RotateToward(Transform target)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                target.rotation,
                shipClassRef.shipArmament[armamentIndex].turningSpeed * Time.deltaTime);
        }
    }

    private void ArmamentUpdate()
    {
        for(int i = 0; i < armaments.Length; i++)
        {
            Armament a = armaments[i];
            ShipArmament sa = shipClass.shipArmament[a.armamentIndex];

            //ROTATE ARMAMENTS
            if (target != null)
            {
                //AIM AT TARGET
            }
            else
            {
                //RETURN TO REST ORIENTATION
            }

            //FIRE ARMAMENTS
            if (a.loaded)
            {
                a.Fire();
            }
        }
    }
    private void CheckForTarget()
    {

    }
}