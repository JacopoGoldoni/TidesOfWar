using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WM_Interface : MonoBehaviour
{
    WarMachine warMachine;

    [Header("Army infos")]
    public string TAG = "AUS";

    [Header("Timer parameters")]
    public float strategyTimerTime = 30f;

    [Header("Debug")]
    public bool debugUnitInfos = false;
    public bool debugStrategyCall = false;

    //TIMERS
    CountdownTimer strategyTimer;

    //UNITS INFOS
    List<int> controlledBattalions = new List<int>();
    List<int> enemyBattalions = new List<int>();

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        warMachine = new WarMachine();

        strategyTimer = new CountdownTimer(strategyTimerTime);
        strategyTimer.OnTimerStop = () => {
            GetUnitInfos();
            StrategicCall();
            strategyTimer.Reset();
        };
        strategyTimer.Start();
    }

    private void Update()
    {
        if(strategyTimer != null)
        {
            strategyTimer.Tick(Time.deltaTime);
        }
    }

    public void GetUnitInfos()
    {
        foreach (CaptainManager cm in GroundBattleUtility.battallionsRef)
        {
            if (cm.TAG == TAG)
            {
                controlledBattalions.Add(GroundBattleUtility.GetIDOfBattalion(cm));
            }
            else
            {
                enemyBattalions.Add(GroundBattleUtility.GetIDOfBattalion(cm));
            }
        }

        if(debugUnitInfos)
        {
            Debug.Log("Unit infos aquired:" + 
                "\nControlled units found:" + controlledBattalions.Count +
                "\nEnemy units found:" + enemyBattalions.Count);
        }
    }
    public void StrategicCall()
    {
        if(debugStrategyCall)
        {
            Debug.Log(TAG + " called for strategy planning.");
        }

        if(warMachine != null)
        {
            warMachine.PopulateInfos(controlledBattalions.ToArray(), enemyBattalions.ToArray());
            warMachine.CalculateStrategy();
            warMachine.ExecuteStrategicPlan();
        }
    }
}