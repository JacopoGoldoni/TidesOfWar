using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarMachine
{
    //INFOS
    int[] controlledBattalions;
    int[] enemydBattalions;

    StrategicAction[] strategicPlan;

    public void PopulateInfos(int[] controlledBattalions, int[] enemydBattalions)
    {
        this.controlledBattalions = controlledBattalions;
        this.enemydBattalions = enemydBattalions;
    }

    public void CalculateStrategy()
    {
        List<StrategicAction> strategy = new List<StrategicAction>();

        AttackEnemyTroops attackStrategy = new AttackEnemyTroops(controlledBattalions, enemydBattalions);
        Retreat retreatStrategy = new Retreat();

        if(attackStrategy.CalculateDifficulty() >= 1f)
        {
            strategy.Add(attackStrategy);
        }
        else
        {
            strategy.Add(retreatStrategy);
        }


        strategicPlan = strategy.ToArray();
    }
    public void ExecuteStrategicPlan()
    {
        foreach(StrategicAction strategicAction in strategicPlan)
        {
            strategicAction.Execute();
        }
    }
}