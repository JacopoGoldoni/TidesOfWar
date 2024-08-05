using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Splines.SplineInstantiate;

public class BattalionFormation
{
    public string name;

    public List<OfficerManager> FrontCompanies = new List<OfficerManager>();
    public List<OfficerManager> LineCompanies = new List<OfficerManager>();
    public List<OfficerManager> RearCompanies = new List<OfficerManager>();

    public List<Vector2> FrontPositions;
    public List<Vector2> LinePositions;
    public List<Vector2> RearPositions;

    public float spaceX, spaceY;
    private BattalionTemplate battalionTemplate;

    public BattalionFormation(BattalionTemplate battalionTemplate)
    {
        this.battalionTemplate = battalionTemplate;
    }

    //FORMATION CHANGES
    public void AddCompany(OfficerManager company)
    {
        switch(company.companyTemplate.hardness)
        {
            case UnitHardness.Light:
                if (battalionTemplate.battalionType == BattalionType.Light) { LineCompanies.Add(company); }
                else { RearCompanies.Add(company); }
                break;
            case UnitHardness.Medium:
                LineCompanies.Add(company);
                break;
            case UnitHardness.Heavy:
                RearCompanies.Add(company);
                break;
        }
    }
    public void MoveToFront(OfficerManager company)
    {
        if (FrontCompanies.Contains(company)) { return; }

        if(LineCompanies.Contains(company))
        {
            LineCompanies.Remove(company);
        }
        else
        {
            RearCompanies.Remove(company);
        }

        switch (company.companyTemplate.hardness)
        {
            case UnitHardness.Light:
                FrontCompanies.Insert(0, company);
                break;
            case UnitHardness.Medium:
                for (int i = FrontCompanies.Count - 1; i >= 0; i--)
                {
                    if (FrontCompanies[i].companyTemplate.hardness == UnitHardness.Medium)
                    {
                        FrontCompanies.Insert(i + 1, company);
                        break;
                    }
                }
                break;
            case UnitHardness.Heavy:
                FrontCompanies.Add(company);
                break;
        }
    }
    public void MoveToLine(OfficerManager company)
    {
        if (LineCompanies.Contains(company)) { return; }

        if (FrontCompanies.Contains(company))
        {
            FrontCompanies.Remove(company);
        }
        else
        {
            RearCompanies.Remove(company);
        }

        switch (company.companyTemplate.hardness)
        {
            case UnitHardness.Light:
                LineCompanies.Insert(0, company);
                break;
            case UnitHardness.Medium:
                for(int i = LineCompanies.Count -1 ; i >= 0; i--)
                {
                    if (LineCompanies[i].companyTemplate.hardness == UnitHardness.Medium)
                    {
                        LineCompanies.Insert(i + 1, company);
                        break;
                    }
                }
                break;
            case UnitHardness.Heavy:
                LineCompanies.Add(company);
                break;
        }
    }
    public void MoveToRear(OfficerManager company)
    {
        if (RearCompanies.Contains(company)) { return; }

        if (FrontCompanies.Contains(company))
        {
            FrontCompanies.Remove(company);
        }
        else
        {
            LineCompanies.Remove(company);
        }

        switch (company.companyTemplate.hardness)
        {
            case UnitHardness.Light:
                RearCompanies.Insert(0, company);
                break;
            case UnitHardness.Medium:
                for (int i = RearCompanies.Count - 1; i >= 0; i--)
                {
                    if (RearCompanies[i].companyTemplate.hardness == UnitHardness.Medium)
                    {
                        RearCompanies.Insert(i + 1, company);
                        break;
                    }
                }
                break;
            case UnitHardness.Heavy:
                RearCompanies.Add(company);
                break;
        }
    }


    public float[] CalculateXPositions(List<OfficerManager> companies, float space)
    {
        if(companies.Count == 0)
        {
            return null;
        }

        float[] p = new float[companies.Count];;
        float w = 0f;

        //FIRST
        w += companies[0].GetWidth() / 2f;
        p[0] = w;
        w += companies[0].GetWidth() / 2f;

        for (int i = 1; i < companies.Count; i++)
        {
            w += space;
            w += companies[i].GetWidth() / 2f;
            p[i] = w;
            w += companies[i].GetWidth() / 2f;
        }
        
        for(int i = 0; i < companies.Count; i++)
        {
            p[i] = p[i] - w / 2f;
        }

        return p;
    }

    public void CalculateAllPositions()
    {
        FrontPositions = new List<Vector2>();
        if(FrontCompanies.Count != 0)
        {
            float[] fx = CalculateXPositions(FrontCompanies, spaceX);
            foreach (float x in fx)
            {
                FrontPositions.Add(new Vector2(x, spaceY));
            }
        }

        LinePositions = new List<Vector2>();
        if (LineCompanies.Count != 0)
        {
            float[] lx = CalculateXPositions(LineCompanies, spaceX);
            foreach (float x in lx)
            {
                LinePositions.Add(new Vector2(x, 0f));
            }
        }

        RearPositions = new List<Vector2>();
        if (RearCompanies.Count != 0)
        {
            float[] rx = CalculateXPositions(RearCompanies, spaceX);
            foreach (float x in rx)
            {
                RearPositions.Add(new Vector2(x, -1 * spaceY));
            }
        }
    }
    public Vector2 GetCompanyPosition(OfficerManager company)
    {
        for (int i = 0; i < LineCompanies.Count; i++)
        {
            if (LineCompanies[i] == company)
            {
                return LinePositions[i];
            }
        }

        for (int i = 0; i < FrontCompanies.Count; i++)
        {
            if (FrontCompanies[i] == company)
            {
                return FrontPositions[i];
            }
        }

        for (int i = 0; i < RearCompanies.Count; i++)
        {
            if (RearCompanies[i] == company)
            {
                return RearPositions[i];
            }
        }

        return Vector2.zero;
    }

    public float GetWidth()
    {
        float f = 0;
        float l = 0;
        float r = 0;

        //FRONT
        if(FrontCompanies.Count != 0)
        {
            for (int i = 0; i < FrontCompanies.Count; i++)
            {
                f += FrontCompanies[i].GetWidth();
            }
            f += (FrontCompanies.Count - 1) * spaceX;
        }

        //LINE
        if (LineCompanies.Count != 0)
        {
            for (int i = 0; i < LineCompanies.Count; i++)
            {
                l += LineCompanies[i].GetWidth();
            }
            l += (LineCompanies.Count - 1) * spaceX;
        }

        //REAR
        if (RearCompanies.Count != 0)
        {
            for (int i = 0; i < RearCompanies.Count; i++)
            {
                r += RearCompanies[i].GetWidth();
            }
            r += (RearCompanies.Count - 1) * spaceX;
        }

        return Mathf.Max(f,l,r);
    }
    public float GetLenght()
    {
        float l = 0;

        if(FrontCompanies.Count != 0)
        {
            l += spaceY;
        }
        if (LineCompanies.Count != 0)
        {
            l += spaceY;
        }
        if (RearCompanies.Count != 0)
        {
            l += spaceY;
        }

        return l;
    }

    //FORMATION MANAGFEMENT
    public void AllInLine()
    {
        List<OfficerManager> f = new List<OfficerManager>(FrontCompanies);
        foreach (OfficerManager om in f)
        {
            MoveToLine(om);
        }
        List<OfficerManager> r = new List<OfficerManager>(RearCompanies);
        foreach (OfficerManager om in r)
        {
            MoveToLine(om);
        }
    }
    public void MoveToRearByHardness(UnitHardness unitHardness)
    {
        List<OfficerManager> l = new List<OfficerManager>(LineCompanies);
        foreach (OfficerManager om in l)
        {
            if (om.companyTemplate.hardness == unitHardness)
            {
                MoveToRear(om);
            }
        }
        List<OfficerManager> f = new List<OfficerManager>(FrontCompanies);
        foreach (OfficerManager om in f)
        {
            if (om.companyTemplate.hardness == unitHardness)
            {
                MoveToRear(om);
            }
        }
    }
    public void MoveToFrontByHardness(UnitHardness unitHardness)
    {
        List<OfficerManager> l = new List<OfficerManager>(LineCompanies);
        foreach (OfficerManager om in l)
        {
            if (om.companyTemplate.hardness == unitHardness)
            {
                MoveToFront(om);
            }
        }
        List<OfficerManager> r = new List<OfficerManager>(RearCompanies);
        foreach (OfficerManager om in r)
        {
            if (om.companyTemplate.hardness == unitHardness)
            {
                MoveToFront(om);
            }
        }
    }
    public void MoveToLineByHardness(UnitHardness unitHardness)
    {
        List<OfficerManager> f = new List<OfficerManager>(FrontCompanies);
        foreach (OfficerManager om in f)
        {
            if (om.companyTemplate.hardness == unitHardness)
            {
                MoveToLine(om);
            }
        }
        List<OfficerManager> r = new List<OfficerManager>(RearCompanies);
        foreach (OfficerManager om in r)
        {
            if (om.companyTemplate.hardness == unitHardness)
            {
                MoveToLine(om);
            }
        }
    }
}