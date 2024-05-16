using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WarMachine
{
    private List<Sensor> sensorsInput = new List<Sensor>();

    //INTERFACE FUNCTIONS
    public void PopulateSensors(Sensor s)
    {
        sensorsInput.Add(s);
    }
    public void ClearSensors()
    {
        sensorsInput.Clear();
    }
    public List<TacticAction> Plan()
    {
        return HLTP.PlanHighLevelTactic(sensorsInput);
    }



    //HLTP
    private class HighLevelTacticalProcesessor
    {
        public List<TacticAction> PlanHighLevelTactic(List<Sensor> sensors)
        {
            List<TacticAction> plan = new List<TacticAction>();

            //Split sensors
            List<FriendlySensor> friendlySensors = new List<FriendlySensor>();
            foreach(Sensor s in sensors)
            {
                if(s.GetType() == typeof(FriendlySensor))
                {
                    friendlySensors.Add((FriendlySensor)s);
                }
            }
            List<HostileSensor> hostileSensors = new List<HostileSensor>();
            foreach (Sensor s in sensors)
            {
                if (s.GetType() == typeof(HostileSensor))
                {
                    hostileSensors.Add((HostileSensor)s);
                }
            }

            //LOGIC
                //Attack nearest enemy
            foreach (FriendlySensor s in friendlySensors)
            {
                AttackEnemy order = new AttackEnemy();
                order.unitID = s.unitID;

                float currentDistance = float.MaxValue;
                int currentEnemyID = 0;
                foreach(HostileSensor h in hostileSensors)
                {
                    float distance = (s.pos - h.pos).magnitude;
                    if(distance < currentDistance)
                    {
                        currentDistance = distance;
                        currentEnemyID = h.unitID;
                    }
                }
                order.enemyID = currentEnemyID;

                plan.Add(order);
            }

            return plan;
        }
    }
    private HighLevelTacticalProcesessor HLTP = new HighLevelTacticalProcesessor();

    //LLTP
    private class LowLevelTacticalProcessor
    {

    }
    List<LowLevelTacticalProcessor> LLTP;
}