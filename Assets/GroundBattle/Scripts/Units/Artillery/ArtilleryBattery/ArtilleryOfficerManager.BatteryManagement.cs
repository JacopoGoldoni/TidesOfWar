using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class ArtilleryOfficerManager : UnitManager, IVisitable
{
    //BATTERY FORMATION MANAGEMENT

    /*
    private void CheckFormation()
    {
        //PAWNS DIED
        List<int> indexes = new List<int>();

        int i = 0;
        foreach (ArtilleryManager am in cannons)
        {
            if (pawns[i] == null && GetPawnRank(i) == 1)
            {
                indexes.Add(i);
            }

            i++;
        }

        if (indexes.Count == 0)
        {
            return;
        }

        if (companySize - indexes.Count < companyFormation.Lines)
        {
            return;
        }

        _formationChanged = true;
        int Lines = companyFormation.Lines;

        foreach (int l in indexes)
        {
            int k = 0;
            int n = 1;
            while (true)
            {
                int s = (k % 2) * ((k + 1) / 2) + (-1) * ((k + 1) % 2) * (k / 2);
                int m = l + n * Lines + s;

                if (m >= pawns.Count)
                {
                    for (int z = companyFormation.Lines; z < pawns.Count; z++)
                    {
                        if (pawns[z] != null)
                        {
                            pawns[l] = pawns[z];
                            pawns[l].ID = l;
                            pawns[z] = null;
                            break;
                        }
                    }
                }

                if ((k + l % Lines) >= Lines)
                {
                    k = 0;
                    s = (k % 2) * ((k + 1) / 2) + (-1) * ((k + 1) % 2) * (k / 2);
                    n++;
                }

                m = l + n * Lines + s;
                if (m >= pawns.Count)
                {
                    for (int z = companyFormation.Lines; z < pawns.Count; z++)
                    {
                        if (pawns[z] != null)
                        {
                            pawns[l] = pawns[z];
                            pawns[l].ID = l;
                            pawns[z] = null;
                            break;
                        }
                    }
                    break;
                }

                if (pawns[m] != null)
                {
                    pawns[l] = pawns[m];
                    pawns[l].ID = l;
                    pawns[m] = null;
                    break;
                }
                else
                {
                    k++;
                }
            }
        }
    }
    */

    private void CheckFormation()
    {

    }
    public void SetFormation(Formation formation)
    {
        batteryFormation = formation;
        batteryBounds = CalculateCompanyBounds();
    }
    public void SendFormation()
    {
        for (int i = 0; i < artilleryBatteryTemplate.BatterySize; i++)
        {
            if (cannons[i] != null)
                cannons[i].MoveTo(
                    GetFormationCoords(i) + Utility.V3toV2(transform.position),
                    um.CurrentRotation()
                    );
        }
        for (int i = 0; i < carriages.Count; i++)
        {
            if (carriages[i] != null)
                carriages[i].MoveTo(
                    CarriagePosition(i) + Utility.V3toV2(transform.position),
                    um.CurrentRotation()
                    );
        }
    }
    public Vector2 GetFormationCoords(int ID)
    {
        Vector2 pos2 = batteryFormation.GetPos(ID);

        pos2.y *= -1;

        Vector3 pos3 = Utility.V2toV3(pos2);

        Quaternion rotation = transform.rotation;

        pos3 = rotation * pos3;

        return Utility.V3toV2(pos3);
    }
    public bool AreCannonsIdle()
    {
        foreach (ArtilleryManager am in cannons)
        {
            if (am != null)
            {
                if (am.um.IsMoving() && am.um.IsRotating())
                {
                    return false;
                }
            }
        }
        return true;
    }
    public int GetCannonRank(int ID)
    {
        return batteryFormation.GetRank(ID);
    }
    public string GetFormationType()
    {
        return batteryFormation.GetType().ToString();
    }
    public bool IsObstructedAt(Vector2 pos)
    {
        Vector3 o = transform.rotation * batteryBounds.center;
        Vector2 center = pos + Utility.V3toV2(o);

        //CHECK IF INTERSECT WITH COMPANY
        List<OfficerManager> allCompanies = GroundBattleUtility.GetAllCompanies();

        foreach (OfficerManager om in allCompanies)
        {
            Vector3 o1 = om.transform.rotation * om.companyBounds.center;
            Vector2 center1 = Utility.V3toV2(om.transform.position) + Utility.V3toV2(o1);

            bool coll = UtilityMath.BoxCollisionDetection(
                new Bounds(Utility.V2toV3(center), batteryBounds.size),
                transform.rotation,
                new Bounds(Utility.V2toV3(center1), om.companyBounds.size),
                om.transform.rotation);


            if (coll)
            {
                return true;
            }
        }

        return false;
    }
    public override float GetWidth()
    {
        float width = (batteryFormation.Lines - 1) * batteryFormation.a;
        return width;
    }

    //BATTERY FIRE MANAGEMENT
    public bool CheckLoadedStatus()
    {
        //TRUE IF ALL LOADED, FALSE IF AT LEAST ONE IS NOT LOADED
        for (int i = 0; i < artilleryBatteryTemplate.BatterySize; i++)
        {
            if (GetCannonRank(i) == 1)
            {
                if (cannons[i] != null)
                {
                    if (!cannons[i].Loaded)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    public bool CheckUnLoadedStatus()
    {
        //TRUE IF ALL UNLOADED, FALSE IF AT LEAST ONE IS LOADED
        for (int i = 0; i < cannons.Count; i++)
        {
            if (GetCannonRank(i) == 1)
            {
                //CHECK ONLY FIRST RANK
                if (cannons[i] != null)
                {
                    if (cannons[i].Loaded)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    public bool AllMounted()
    {
        foreach(ArtilleryManager am in cannons)
        {
            if(!am.mounted)
            {
                return false;
            }
        }

        return true;
    }
    public bool AllDismounted()
    {
        foreach (ArtilleryManager am in cannons)
        {
            if (am.mounted)
            {
                return false;
            }
        }

        return true;
    }
    public void SetFireStatus(bool status)
    {
        FireAll = status;
    }
}