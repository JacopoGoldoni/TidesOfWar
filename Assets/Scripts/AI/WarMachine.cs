using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

public class WarMachine
{
    Match_AI match_AI_Ref;

    Factions faction;

    private HighLevelTacticalProcesessor HLTP;
    List<LowLevelTacticalProcessor> LLTPs;

    private List<StrategicAction> currentStrategy;
    public List<TacticAction> currentTactics;

    public enum Stance { Aggressive, Defensive }
    public Stance stance = Stance.Aggressive;

    public void Initialize()
    {
        HLTP = new HighLevelTacticalProcesessor(this);
        LLTPs = new List<LowLevelTacticalProcessor>();
    }

    public void Populate_HLTP_BlackBoard()
    {

    }
    public void Populate_LLTP_BlackBoard()
    {
        foreach(LowLevelTacticalProcessor LLTP in LLTPs)
        {
            
        }
    }


    public void Execute_HLTP()
    {
        HLTP.EvaluateHighLevelSituation();
        HLTP.CalculatePriorities();
        currentStrategy = HLTP.PlanHighLevelTactic();
    }
    public void Execute_LLTPs()
    {
        currentTactics = new List<TacticAction>();
        foreach(LowLevelTacticalProcessor LLTP in LLTPs)
        {
            currentTactics.AddRange(LLTP.PlanLowLevelTactic());
        }
    }

    //HLTP
    private class HighLevelTacticalProcesessor
    {
        WarMachine warMachine_Ref;

        BlackBoard blackBoard = new BlackBoard();

        public HighLevelTacticalProcesessor(WarMachine warMachine)
        {
            warMachine_Ref = warMachine;
        }

        public List<StrategicAction> PlanHighLevelTactic()
        {
            List<StrategicAction> plan = new List<StrategicAction>();

            //TODO :HL LOGIC
            if((float)blackBoard.GetData("forceRatio") >= 1f)
            {
                //ATTACK
                List<int> battalions = new List<int>();
                foreach(CaptainManager cm in warMachine_Ref.match_AI_Ref.controlledBattalions)
                {
                    battalions.Add(cm.battalionNumber);
                }

                plan.Add(new AttackEnemyTroops(battalions.ToArray()));
                warMachine_Ref.stance = Stance.Aggressive;
            }
            else
            {
                //RETREAT
                plan.Add(new Retreat());
                warMachine_Ref.stance = Stance.Defensive;
            }

            return plan;
        }
        public void PopulateBlackBoard(Factions faction)
        {
            int alliedForce = 0;
            int enemyForce = 0;

            //CALCULATE FORCES STRENGHT
            foreach(OfficerManager om in GroundBattleUtility.GetAllCompanies())
            {
                if(om.faction == faction)
                {
                    alliedForce += om.companySize;
                }
                else
                {
                    enemyForce += om.companySize;
                }
            }
            blackBoard.AddData("alliedForce", alliedForce);
            blackBoard.AddData("enemyForce", enemyForce);
        }
        public void CalculatePriorities()
        {
            //TODO: calculate priority of all units and flags
        }
        public float EvaluateHighLevelSituation()
        {
            float forceRatio = (float)blackBoard.GetData("alliedForces") / (float)blackBoard.GetData("enemyForces");
            blackBoard.AddData("forceRatio", forceRatio);

            return forceRatio;
        }
    }

    //LLTP
    private class LowLevelTacticalProcessor
    {
        WarMachine warMachine_Ref;

        int battalionID;
        BlackBoard blackBoard = new BlackBoard();
        Stance stance;

        public LowLevelTacticalProcessor(int battalionID, WarMachine warMachine)
        {
            this.battalionID = battalionID;
            warMachine_Ref = warMachine;
        }
        
        public void PopulateBlackBoard()
        {
            CaptainManager cm = GroundBattleUtility.GetBattalionByID(battalionID);

            blackBoard.AddData("size", cm.GetSize());
            blackBoard.AddData("pos", Utility.V3toV2(cm.transform.position));
            blackBoard.AddData("dir", Utility.V3toV2(cm.transform.forward).normalized);
        }

        public List<TacticAction> PlanLowLevelTactic() 
        {
            List<TacticAction> plan = new List<TacticAction>();

            //TODO :TACTICAL LOGIC
            StrategicAction strategicAction = warMachine_Ref.currentStrategy[0];
            if (strategicAction.GetType() == typeof(AttackEnemyTroops))
            {
                //ATTACK NEAREST
                AttackNearest attackNearestOrder = new AttackNearest();
                attackNearestOrder.unitID = battalionID;
                plan.Add(attackNearestOrder);
            }
            else if(strategicAction.GetType() == typeof(Retreat))
            {
                //RETREAT UNIT
                MoveTo MoveToOrder = new MoveTo();
                MoveToOrder.unitID = battalionID;
                plan.Add(MoveToOrder);
            }

            return plan;
        }
    }
}