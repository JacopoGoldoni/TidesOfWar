using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class UnitOrders
{
    public static void Company_SendLineFormation(OfficerManager[] companies)
    {
        foreach (OfficerManager om in companies)
        {
            Formation f = new Line(om.companySize);
            om.SetFormation(f);
            om.ReceiveMovementOrder(false, Utility.V3toV2(om.transform.position), om.transform.rotation);
        }
    }
    public static void Company_SendColumnFormation(OfficerManager[] companies)
    {
        foreach (OfficerManager om in companies)
        {
            Formation f = new Column(om.companySize);
            om.SetFormation(f);
        }
    }
    public static void Company_SendStopOrder(OfficerManager[] companies)
    {
        foreach (OfficerManager om in companies)
        {
            om.um.SetDestination(Utility.V3toV2(om.transform.position), om.transform.rotation);
        }
    }
    public static void Company_SendFireAll(OfficerManager[] companies)
    {
        foreach (OfficerManager om in companies)
        {
            om.FireAll = true;
        }
    }
    public static void Company_SendHoldFire(OfficerManager[] companies)
    {
        foreach (OfficerManager om in companies)
        {
            om.FireAll = false;
        }
    }
    public static void Company_SendMelee(OfficerManager[] companies)
    {
        foreach (OfficerManager om in companies)
        {
            //MELEE LOGIC
        }
    }
    public static void Company_SendMarch(OfficerManager[] companies)
    {
        foreach (OfficerManager om in companies)
        {
            //MACRH LOGIC
        }
    }
    public static void Company_SendDetach(OfficerManager[] companies)
    {
        foreach (OfficerManager om in companies)
        {
            if (!om.IsDetached())
            {
                om.Detach();
            }
        }
    }
}