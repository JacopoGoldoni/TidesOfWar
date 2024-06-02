using Den.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavalManager : MonoBehaviour
{
    //COMPONENTS
    NavalMovement navalMovement;

    //SHIP STATS
    public ShipClass shipClass;
    private Armament[] armaments;
    int hull;

    public GameObject target;

    //FINITE STATE MACHINE
    FiniteStateMachine fsm = new FiniteStateMachine();

    private void Awake()
    {
        navalMovement = GetComponent<NavalMovement>();

        InitializeStateMachine();
    }

    void OnAwake()
    {
            
    }

    private void Initialize()
    {
        //FIND ALL ARMAMENT POSTS
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Update();
    }

    private void InitializeStateMachine()
    {
        //STATES
        State Idle = new State(
                    "Idle",
                    () => { },
                    () => { },
                    () => { }
                );
        fsm.AddState(Idle);
        State SeekTarget = new State(
                    "SeekTarget",
                    () => { },
                    () => { },
                    () => { }
                );
        fsm.AddState(SeekTarget);
        fsm.AddState(SeekTarget);
        State FleeTarget = new State(
                    "FleeTarget",
                    () => { },
                    () => { },
                    () => { }
                );
        fsm.AddState(SeekTarget);
        State Patrol = new State(
                    "Patrol",
                    () => { },
                    () => { },
                    () => { }
                );
        fsm.AddState(SeekTarget);

        //TRANSITIONS
        //IDLE -> SEEKTARGET
        fsm.AddTransition(new Transition(
                    Idle,
                    SeekTarget,
                    () => { return (target != null); }
                )    
            );
    }

    class Armament
    {
        public Transform rest;
        public Transform transform; //IN LOCAL SPACE
        public int armamentIndex;

        private ShipClass shipClassRef;

        public bool loaded;

        private CountdownTimer timer;

        public Armament(Transform rest, int armamentIndex, ShipClass shipClass)
        {
            this.rest = rest;
            this.transform = rest;
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

        public void Rotate(Transform target)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, shipClassRef.shipArmament[armamentIndex].turningSpeed * Time.deltaTime);
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
                a.Rotate(a.rest);
            }

            //FIRE ARMAMENTS
            if (a.loaded)
            {
                a.Fire();
            }
        }
    }
}