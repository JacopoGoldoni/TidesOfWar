using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBattleManager : MonoBehaviour
{
    CountdownTimer winTimer;
    CountdownTimer matchTimer;

    public GroundBattleTerrainManager groundBattleTerrainManagerRef;

    [Header("Features")]
    public bool generateTerrain = true;
    public bool spawnArmies = true;

    [Header("Debug")]
    public bool initializationDebug = false;

    void Start()
    {
        ArmyUtility.LoadAllCompaniesTemplates();
        ArmyUtility.LoadAllBattalionsTemplates();

        ArmyUtility.LoadAllCompanies();
        ArmyUtility.LoadAllBattalions();
        ArmyUtility.LoadAllArmies();

        GroundBattleUtility.spawningPoints.Add("FRA", new Vector2(40f, 100f));
        GroundBattleUtility.spawningPoints.Add("AUS", new Vector2(40f, 20f));

        InitializeMatch();
        InitializeTimers();
    }

    //INITIALIZERS
    private void InitializeTimers()
    {
        winTimer = new CountdownTimer(5f);
        winTimer.OnTimerStart = () =>
        {
            CheckWin();

            winTimer.Reset();
            winTimer.Start();
        };
        winTimer.Start();

        matchTimer = new CountdownTimer(GameUtility.GROUND_MATCH_LENGHT);
        matchTimer.OnTimerStart = () =>
        {
            Draw();
        };
        matchTimer.Start();
    }
    private void InitializeMatch()
    {
        if(initializationDebug)
        {
            Debug.Log("Ground battle initialization.");
        }

        if(generateTerrain)
        {
            groundBattleTerrainManagerRef.CallGeneration();
        }

        if(spawnArmies)
        {
            SpawnArmies();
        }
    }

    //ARMY
    private void SpawnArmies()
    {
        foreach (Army army in ArmyUtility.armies)
        {
            SpawnArmy(army);
        }
    }
    private void SpawnArmy(Army army)
    {
        if(initializationDebug)
        {
            Debug.Log("Spawning army: " + army.name + "\n" +
                "Owned by: " + army.TAG);
        }

        //GET FACTION SPAWNING POINT
        Vector2 sp = GroundBattleUtility.spawningPoints[army.TAG];

        //SPAWN BATTALLIONS
        int i = 0;
        foreach (Battalion battalion in army.battalions)
        {
            float y = ((i % 2) * 2 - 1) * 10f;
            float x = (i / 2) + 1f;
            Vector2 pos = sp + new Vector2(x, y);
            SpawnBattalion(battalion, pos);
            i++;
        }
    }

    //BATTALION
    private void SpawnBattalion(Battalion battalion, Vector2 pos)
    {
        if (initializationDebug)
        {
            Debug.Log("Spawning battallion: " + battalion.name + "\n" +
                "Owned by: " + battalion.TAG + "\n" +
                "Template: " + battalion.template.name);
        }

        GameObject captain = Instantiate(battalion.template.captainPrefab, new Vector3(pos.x, GroundBattleUtility.GetMapHeight(pos), pos.y), Quaternion.identity);

        CaptainManager cm = captain.GetComponent<CaptainManager>();

        //LOAD INFORMATIONS INTO NEW BATTALION
        cm.battalionTemplate = battalion.template;
        cm.battalionName = battalion.name;
        cm.TAG = battalion.TAG;
        cm.battalionNumber = battalion.ID;

        GroundBattleUtility.WarpAgentToNavMesh(captain);

        cm.Initialize();
        GroundBattleUtility.RegisterBattallion(cm);
    }

    private void CheckWin()
    {
        foreach(string TAG in GroundBattleUtility.factions)
        {
            if (GroundBattleUtility.IsFactionRouting(TAG))
            {
                Win(TAG);
            }
        }
    }

    //MACTH RESULTS
    private void Draw()
    {
        
    }
    private void Win(string TAG)
    {

    }
}