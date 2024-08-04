using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class CaptainManager : UnitManager
{
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
    public float AvarageMorale()
    {
        float avarageMorale = 0f;

        foreach (OfficerManager om in companies)
        {
            avarageMorale += om.Morale;
        }

        avarageMorale /= companies.Count;

        return avarageMorale;
    }
    public override float GetWidth()
    {
        int size = 0;

        foreach (OfficerManager company in companies)
        {
            size += company.companySize;
        }

        return size;
    }
}