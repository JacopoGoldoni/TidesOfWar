using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class UnitOrders
{
    public static void ArtilleryBattery_SendLineFormation(ArtilleryOfficerManager[] artilleryBatteries)
    {
        foreach (ArtilleryOfficerManager aof in artilleryBatteries)
        {
            Formation f = new Line(aof.batterySize);
            f.SetSizeByRanks(aof.batterySize, 1);
            f.a = 2f;
            aof.SetFormation(f);
            aof.ReceiveMovementOrder(false, Utility.V3toV2(aof.transform.position), aof.transform.rotation);
        }
    }
    public static void ArtilleryBattery_SendColumnFormation(ArtilleryOfficerManager[] artilleryBatteries)
    {
        foreach (ArtilleryOfficerManager aof in artilleryBatteries)
        {
            Formation f = new Column(aof.batterySize);
            f.SetSizeByLines(aof.batterySize, 1);
            f.b = 6f;
            aof.SetFormation(f);
            aof.ReceiveMovementOrder(false, Utility.V3toV2(aof.transform.position), aof.transform.rotation);
        }
    }
    public static void ArtilleryBattery_SendStopOrder(ArtilleryOfficerManager[] artilleryBatteries)
    {
        foreach (ArtilleryOfficerManager aom in artilleryBatteries)
        {
            aom.um.SetDestination(Utility.V3toV2(aom.transform.position), aom.transform.rotation);
        }
    }
    public static void ArtilleryBattery_SendFireAll(ArtilleryOfficerManager[] artilleryBatteries)
    {
        foreach (ArtilleryOfficerManager aof in artilleryBatteries)
        {
            aof.FireAll = true;
        }
    }
    public static void ArtilleryBattery_SendHoldFire(ArtilleryOfficerManager[] artilleryBatteries)
    {
        foreach (ArtilleryOfficerManager aof in artilleryBatteries)
        {
            aof.FireAll = false;
        }
    }
}