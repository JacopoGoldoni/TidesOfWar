using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class GroundBattleUtility
{
    public static List<OfficerManager> companiesRef = new List<OfficerManager>();
    public static List<CaptainManager> battallionsRef = new List<CaptainManager>();

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

    public static int GetIDOfCompany(OfficerManager om)
    {
        return companiesRef.IndexOf(om);
    }
    public static int GetIDOfBattalion(CaptainManager cm)
    {
        return battallionsRef.IndexOf(cm);
    }
}