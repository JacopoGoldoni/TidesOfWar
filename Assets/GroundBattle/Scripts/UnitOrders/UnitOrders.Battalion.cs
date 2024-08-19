using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class UnitOrders
{
    public static void Battalion_SendLineFormation(CaptainManager[] battalions)
    {
        foreach (CaptainManager cm in battalions)
        {
            cm.battalionFormation.AllInLine();
            cm.battalionFormation.CalculateAllPositions();
            cm.SendFormation();

            cm.UpdateFire();
        }
    }
    public static void Battalion_SendColumnFormation(CaptainManager[] battalions)
    {
        foreach (CaptainManager cm in battalions)
        {
            //ADD HERE COLUMN LOGIC
            cm.ReceiveMovementOrder(false, Utility.V3toV2(cm.transform.position), cm.transform.rotation);

            cm.UpdateFire();
        }
    }
    public static void Battalion_SendLightFront(CaptainManager[] battalions)
    {
        foreach (CaptainManager cm in battalions)
        {
            cm.battalionFormation.MoveToFrontByHardness(UnitHardness.Light);

            cm.battalionFormation.CalculateAllPositions();
            cm.SendFormation();

            cm.UpdateFire();
        }
    }
    public static void Battalion_SendHeavyFront(CaptainManager[] battalions)
    {
        foreach (CaptainManager cm in battalions)
        {
            cm.battalionFormation.MoveToFrontByHardness(UnitHardness.Heavy);

            cm.battalionFormation.CalculateAllPositions();
            cm.SendFormation();

            cm.UpdateFire();
        }
    }
    public static void Battalion_SendLightLine(CaptainManager[] battalions)
    {
        foreach (CaptainManager cm in battalions)
        {
            cm.battalionFormation.MoveToLineByHardness(UnitHardness.Light);

            cm.battalionFormation.CalculateAllPositions();
            cm.SendFormation();

            cm.UpdateFire();
        }
    }
    public static void Battalion_SendHeavyLine(CaptainManager[] battalions)
    {
        foreach (CaptainManager cm in battalions)
        {
            cm.battalionFormation.MoveToLineByHardness(UnitHardness.Heavy);

            cm.battalionFormation.CalculateAllPositions();
            cm.SendFormation();

            cm.UpdateFire();
        }
    }
    public static void Battalion_SendLightRear(CaptainManager[] battalions)
    {
        foreach (CaptainManager cm in battalions)
        {
            cm.battalionFormation.MoveToRearByHardness(UnitHardness.Light);

            cm.battalionFormation.CalculateAllPositions();
            cm.SendFormation();

            cm.UpdateFire();
        }
    }
    public static void Battalion_SendHeavyRear(CaptainManager[] battalions)
    {
        foreach (CaptainManager cm in battalions)
        {
            cm.battalionFormation.MoveToRearByHardness(UnitHardness.Heavy);

            cm.battalionFormation.CalculateAllPositions();
            cm.SendFormation();

            cm.UpdateFire();
        }
    }
    public static void Battalion_SendStopOrder(CaptainManager[] battalions)
    {
        foreach (CaptainManager cm in battalions)
        {
            cm.um.SetDestination(Utility.V3toV2(cm.transform.position), cm.transform.rotation);
        }
    }
    public static void Battalion_SendFireAll(CaptainManager[] battalions)
    {
        foreach (CaptainManager cm in battalions)
        {
            cm.FireAll = true;
            cm.UpdateFire();
        }
    }
    public static void Battalion_SendHoldFire(CaptainManager[] battalions)
    {
        foreach (CaptainManager cm in battalions)
        {
            cm.FireAll = false;
            foreach (OfficerManager om in cm.companies)
            {
                om.SetFireStatus(false);
            }
        }
    }
    public static void Battalion_SendMarch(CaptainManager[] battalions)
    {
        foreach (CaptainManager cm in battalions)
        {
            //MARCH LOGIC
        }
    }

    public static void Battalion_SendDefend(CaptainManager[] battalions)
    {

    }
    public static void Battalion_SendAttack(CaptainManager[] battalions, CaptainManager[] targets)
    {
        int n = battalions.Length;
        int m = targets.Length;

        int ratio = n / m;

        for(int i = 0; i < battalions.Length; i++)
        {
            CaptainManager battalion = battalions[i];
            CaptainManager target = targets[i / ratio];

            Vector2 pos = 
                Utility.V3toV2(target.transform.position) + 
                Utility.V3toV2((battalion.transform.position - target.transform.position).normalized) * 
                (battalion.GetMinRange() + 3f) * 0.75f;
            Quaternion rot = 
                Quaternion.LookRotation(
                    new Vector3(
                        target.transform.position.x - battalion.transform.position.x, 
                        0f, 
                        target.transform.position.z - battalion.transform.position.z).normalized, 
                    Vector3.up);

            battalion.um.SetDestination(pos, rot);
        }
    }
}
