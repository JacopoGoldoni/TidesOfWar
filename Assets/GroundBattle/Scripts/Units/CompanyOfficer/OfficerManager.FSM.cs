using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class OfficerManager : UnitManager, IVisitable
{
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
        officerStates.Add(anyState);
            //IDLE
        State Idle = new State(
                "Idle",
                Idle_Enter,
                null,
                Idle_Cycle
            );
        officerStates.Add(Idle);
            //MOVING
        State Moving = new State(
                "Moving",
                Moving_Enter,
                null,
                Moving_Cycle
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
                    SendReloadMessage();
                },
                null,
                null
            );
        officerStates.Add(Reloading);
            //FLEEING
        State Fleeing = new State(
                "Fleeing",
                Fleeing_Enter,
                null,
                Fleeing_Cycle
            );
        officerStates.Add(Fleeing);


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
        officerTransitions.Add(IdleMoving);
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
        officerTransitions.Add(MovingIdle);
        //IDLE -> FIRING
        Transition IdleFiring = new Transition(
                Idle,
                Firing,
                () => {
                    if (targetUnit != null && CheckLoadedStatus() && GetFormationType() == "Line")
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
                    if (CheckUnLoadedStatus() && um.MovementPoints.Count == 0 && FireAll)
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

    private void Idle_Enter()
    {
        CheckFormation();
        if (_formationChanged)
        {
            ReceiveMovementOrder(false, Utility.V3toV2(transform.position), transform.rotation);
            SendFormation();
            _formationChanged = false;
        }
    }
    private void Idle_Cycle()
    {
        if(FireAll)
        {
            targetUnit = EnemyInRange(Range);
        }
    }
    private void Moving_Cycle()
    {
        ((OfficerMovement)um).UpdateMovement();

        if (companyFormation.name == "Column")
        {
            if((transform.position - um.CurrentDestination()).magnitude < 0.1f)
            {
                SendFormation();
            }
            else
            {
                UpdateSnakeFormation();
                SendSnakeFormation();
            }
        }
        else
        {
            SendFormation();
        }
    }
    private void Moving_Enter()
    {
        if (companyFormation.name == "Column")
        {
            InitializeSnakeFormation();
        }
    }
    private void Fleeing_Enter()
    {
        Vector3 fleePos = (transform.position - targetUnit.transform.position).normalized + transform.position;
        Quaternion fleeRot = Quaternion.LookRotation((transform.position - targetUnit.transform.position).normalized, Vector3.up);

        um.SetDestination(Utility.V3toV2(fleePos), fleeRot);
    }
    private void Fleeing_Cycle()
    {

        ((OfficerMovement)um).UpdateMovement();
        SendFormation();
    }
}