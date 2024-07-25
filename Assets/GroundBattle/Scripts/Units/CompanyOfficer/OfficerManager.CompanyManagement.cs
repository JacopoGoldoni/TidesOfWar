using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class OfficerManager : UnitManager, IVisitable
{
    //COMPANY FORMATION MANAGEMENT
    private void CheckFormation()
    {
        //PAWNS DIED
        List<int> indexes = new List<int>();

        int i = 0;
        foreach (PawnManager pm in pawns)
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
    public void SetFormation(Formation formation)
    {
        companyFormation = formation;
        companyBounds = CalculateCompanyBounds();
    }
    public void SendFormation()
    {
        for (int i = 0; i < companySize; i++)
        {
            if (pawns[i] != null)
                pawns[i].MoveTo(
                    GetFormationCoords(i) + Utility.V3toV2(transform.position),
                    um.CurrentRotation()
                    );
        }
    }
    public Vector2 GetFormationCoords(int ID)
    {
        Vector2 pos2 = companyFormation.GetPos(ID);

        pos2.y *= -1;

        Vector3 pos3 = Utility.V2toV3(pos2);

        Quaternion rotation = transform.rotation;

        pos3 = rotation * pos3;

        return Utility.V3toV2(pos3);
    }
    public bool ArePawnIdle()
    {
        foreach (PawnManager pm in pawns)
        {
            if (pm != null)
            {
                if (pm.um.IsMoving() && pm.um.IsRotating())
                {
                    return false;
                }
            }
        }
        return true;
    }
    public int GetPawnRank(int ID)
    {
        return companyFormation.GetRank(ID);
    }
    public string GetFormationType()
    {
        return companyFormation.GetType().ToString();
    }
    public bool IsObstructedAt(Vector2 pos)
    {
        Vector3 o = transform.rotation * companyBounds.center;
        Vector2 center = pos + Utility.V3toV2(o);

        //CHECK IF INTERSECT WITH COMPANY
        List<OfficerManager> allCompanies = GroundBattleUtility.GetAllCompanies();

        foreach (OfficerManager om in allCompanies)
        {
            Vector3 o1 = om.transform.rotation * om.companyBounds.center;
            Vector2 center1 = Utility.V3toV2(om.transform.position) + Utility.V3toV2(o1);

            bool coll = UtilityMath.BoxCollisionDetection(
                new Bounds(Utility.V2toV3(center), companyBounds.size),
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
    public float GetCompanyWidth()
    {
        float width = (companyFormation.Lines - 1) * companyFormation.a;
        return width;
    }

    //COMPANY FIRE MANAGEMENT
    public void SendFireMessage()
    {
        for (int i = 0; i < companySize; i++)
        {
            if (GetPawnRank(i) == 1)
            {
                //FIRE ONLY FIRST RANK
                if (pawns[i] != null)
                    pawns[i].CallFire();
            }
        }
        Ammo -= 1;
    }
    public void SendRelaodMessage()
    {
        for (int i = 0; i < companySize; i++)
        {
            if (GetPawnRank(i) == 1)
            {
                //FIRE ONLY FIRST RANK
                if (pawns[i] != null)
                    pawns[i].CallReload();
            }
        }
        Ammo -= 1;
    }
    public bool CheckLoadedStatus()
    {
        bool a = false;

        //TRUE IF ALL LOADED, FALSE IF AT LEAST ONE IS NOT LOADED
        for (int i = 0; i < companySize; i++)
        {
            //CHECK ONLY FIRST RANK
            if (GetPawnRank(i) == 1)
            {
                if (pawns[i] != null)
                {
                    a = true;
                    if (!pawns[i].Loaded)
                    {
                        return false;
                    }
                }
            }
        }
        return a;
    }
    public bool CheckUnLoadedStatus()
    {
        //TRUE IF ALL UNLOADED, FALSE IF AT LEAST ONE IS LOADED
        for (int i = 0; i < companySize; i++)
        {
            if (GetPawnRank(i) == 1)
            {
                if (pawns[i] != null)
                {
                    //CHECK ONLY FIRST RANK
                    if (pawns[i].Loaded)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    public void SetFireStatus(bool status)
    {
        FireAll = status;

        foreach(PawnManager pm in pawns)
        {
            if(status)
            {
                pm.TakeRifleDown();
            }
            else
            {
                pm.TakeRifleUp();
            }
        }
    }
}