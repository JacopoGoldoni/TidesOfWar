using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CaptainManager : UnitManager
{
    //BATTALLION FORMATION MANAGEMENT
    public void SendFormation()
    {
        for (int i = 0; i < companies.Count; i++)
        {
            if (companies[i] != null)
            {
                companies[i].ReceiveMovementOrder(
                    false,
                    GetFormationCoords(companies[i]) + Utility.V3toV2(transform.position),
                    um.CurrentRotation()
                    );
            }
        }
    }
    public Vector2 GetFormationCoords(OfficerManager company)
    {
        Vector2 pos2 = battalionFormation.GetCompanyPosition(company) + Vector2.up * 3f;

        Vector3 pos3 = Utility.V2toV3(pos2);

        Quaternion rotation = transform.rotation;

        pos3 = rotation * pos3;

        return Utility.V3toV2(pos3);
    }
    public bool AreCompaniesIdle()
    {
        foreach (OfficerManager om in companies)
        {
            if (om != null)
            {
                if (om.stateName != "Idle")
                {
                    return false;
                }
            }
        }
        return true;
    }
    public int GetCompanyPos(OfficerManager company)
    {
        if(battalionFormation.FrontCompanies.Contains(company))
        {
            return 0;
        }
        if (battalionFormation.LineCompanies.Contains(company))
        {
            return 1;
        }
        if (battalionFormation.RearCompanies.Contains(company))
        {
            return 2;
        }
        return 0;
    }
    public void UpdateFire()
    {
        if(battalionFormation.FrontCompanies.Count == 0)
        {
            //NO FRONT COMPANIES
            foreach(OfficerManager l in battalionFormation.LineCompanies)
            {
                l.FireAll = true;
            }
        }
        else
        {
            //HAS FRONT COMPANIES
            foreach (OfficerManager f in battalionFormation.FrontCompanies)
            {
                f.FireAll = true;
            }
            foreach (OfficerManager l in battalionFormation.LineCompanies)
            {
                l.FireAll = false;
            }
        }

        foreach(OfficerManager r in battalionFormation.RearCompanies)
        {
            r.FireAll = false;
        }
    }
}