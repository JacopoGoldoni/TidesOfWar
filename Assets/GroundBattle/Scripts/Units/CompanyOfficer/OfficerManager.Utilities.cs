using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class OfficerManager : UnitManager, IVisitable
{
    public override float GetWidth()
    {
        return (companyFormation.Lines - 1) * companyFormation.a;
    }
    public override float GetLenght()
    {
        return (companyFormation.Ranks - 1) * companyFormation.b;
    }
    private void DrawPathLine()
    {
        lineRenderer.enabled = drawPathLine;

        if (drawPathLine)
        {
            NavMeshAgent na = um.navAgent;

            lineRenderer.positionCount = na.path.corners.Length;
            lineRenderer.SetPosition(0, transform.position);

            if (na.path.corners.Length < 2)
            {
                return;
            }

            for (int i = 1; i < na.path.corners.Length; i++)
            {
                lineRenderer.SetPosition(i, na.path.corners[i] + Vector3.up * 0.5f);
            }
        }
    }
    public void Highlight(bool highlight)
    {
        if (highlight)
        {
            m.SetInt("_Hightlight", 1);
        }
        else
        {
            m.SetInt("_Hightlight", 0);
        }
    }
    private void CalculateMorale()
    {
        int n = 0;

        foreach (PawnManager p in pawns)
        {
            if (p == null)
            {
                n++;
            }
        }

        float s = (float)(pawns.Count - n) / (float)pawns.Count;

        Morale = (int)(s * companyTemplate.BaseMorale);
    }
    private Bounds CalculateCompanyBounds()
    {
        Vector3 p;
        Vector3 s;

        p = new Vector3(
            0,
            0,
            -1 * companyFormation.Ranks * companyFormation.b / 2);
        s = new Vector3(
            companyFormation.Lines * companyFormation.a,
            1f,
            companyFormation.Ranks * companyFormation.b);

        return new Bounds(p, s);
    }
    public bool IsDetached()
    {
        return (masterCaptain == null);
    }
}