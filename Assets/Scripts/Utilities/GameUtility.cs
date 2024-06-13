using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class GameUtility
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

    public static void RegisterCompany(OfficerManager om)
    {
        companiesRef.Add(om);
    }

    public static OfficerManager GetCompanyByID(int ID)
    {
        return companiesRef[ID];
    }

    public static int GetIDOfCompany(OfficerManager om)
    {
        return companiesRef.IndexOf(om);
    }
}