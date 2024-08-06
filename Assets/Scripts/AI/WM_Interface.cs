using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WM_Interface : MonoBehaviour
{
    WarMachine warMachine;

    string TAG = "AUS";

    public void StrategicCall()
    {
        if(warMachine != null)
        {
            List<int> controlledBattalions = new List<int>();
            List<int> enemyBattalions = new List<int>();
            foreach (CaptainManager cm in GroundBattleUtility.battallionsRef)
            {
                if(cm.tag == TAG)
                {
                    controlledBattalions.Add(GroundBattleUtility.GetIDOfBattalion(cm));
                }
                else
                {
                    enemyBattalions.Add(GroundBattleUtility.GetIDOfBattalion(cm));
                }
            }

            warMachine.PopulateInfos(controlledBattalions.ToArray(), enemyBattalions.ToArray());
            warMachine.CalculateStrategy();
            warMachine.ExecuteStrategicPlan();
        }
    }
}