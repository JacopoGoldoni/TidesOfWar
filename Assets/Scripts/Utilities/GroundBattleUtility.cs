using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.AI;

public static class GroundBattleUtility
{
    public static List<Army> armies = new List<Army>();
    public static List<string> factions = new List<string>();

    public static Dictionary<string, Vector2> spawningPoints = new Dictionary<string, Vector2>();

    public static List<OfficerManager> companiesRef = new List<OfficerManager>();
    public static List<CaptainManager> battallionsRef = new List<CaptainManager>();

    public static Terrain terrainRef;

    public static List<OfficerManager> GetAllCompanies()
    {
        if(companiesRef == null || companiesRef.Count == 0)
        {
            companiesRef = (Object.FindObjectsByType<OfficerManager>(FindObjectsSortMode.None)).ToList<OfficerManager>();
        }

        return companiesRef;
    }
    public static List<CaptainManager> GetAllBattalions()
    {
        if (battallionsRef == null || battallionsRef.Count == 0)
        {
            battallionsRef = (Object.FindObjectsByType<CaptainManager>(FindObjectsSortMode.None)).ToList<CaptainManager>();
        }

        return battallionsRef;
    }

    public static void RegisterCompany(OfficerManager om)
    {
        companiesRef.Add(om);
        om.ID = companiesRef.Count;
    }
    public static void RegisterBattallion(CaptainManager cm)
    {
        battallionsRef.Add(cm);
        //cm.ID = battallionsRef.Count;
    }
    public static bool IsCompany(OfficerManager om1, OfficerManager om2)
    {
        return om1.ID == om2.ID;
    }

    public static OfficerManager GetCompanyByID(int ID)
    {
        return companiesRef[ID];
    }
    public static CaptainManager GetBattalionByID(int ID)
    {
        return battallionsRef[ID];
    }
    public static CaptainManager[] GetBattalionsByID(int[] ID)
    {
        List<CaptainManager> list = new List<CaptainManager>();
        
        foreach(int index in ID)
        {
            list.Add(GetBattalionByID(index));
        }

        return list.ToArray();
    }

    public static int GetIDOfCompany(OfficerManager om)
    {
        return companiesRef.IndexOf(om);
    }
    public static int GetIDOfBattalion(CaptainManager cm)
    {
        return battallionsRef.IndexOf(cm);
    }

    public static bool IsFactionRouting(string TAG)
    {
        foreach(OfficerManager om in companiesRef)
        {
            if(!om.isRouting)
            {
                return false;
            }
        }
        return true;
    }

    public static float GetMapHeight(Vector2 worldPos)
    {
        if(terrainRef != null)
        {
            float height = terrainRef.SampleHeight(Utility.V3toV2(worldPos));
            return height;
        }
        return 0f;
    }
    public static Vector3 GetMapPosition(Vector2 worldPos)
    {
        if(terrainRef != null)
        {
            Vector3 pos = new Vector3(worldPos.x, GetMapHeight(worldPos), worldPos.y);
            return pos;
        }
        return Utility.V2toV3(worldPos);
    }
    public static void WarpAgentToNavMesh(GameObject agent)
    {
        if(terrainRef != null)
        {
            NavMeshAgent navMeshAgent = agent.GetComponent<NavMeshAgent>();
            NavMeshHit hit;
            if (NavMesh.SamplePosition(navMeshAgent.transform.position, out hit, 10.0f, NavMesh.AllAreas))
            {
                navMeshAgent.Warp(hit.position); // Warp the agent to the closest point on the NavMesh
            }
        }

        return;
    }
}