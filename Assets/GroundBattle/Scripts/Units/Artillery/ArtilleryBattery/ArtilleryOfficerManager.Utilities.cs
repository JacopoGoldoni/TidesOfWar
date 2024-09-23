using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class ArtilleryOfficerManager : UnitManager, IVisitable
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
    private void CalculateMorale()
    {
        int n = 0;

        foreach (ArtilleryManager am in cannons)
        {
            if (am == null)
            {
                n++;
            }
        }

        float s = (float)(cannons.Count - n) / (float)cannons.Count;

        Morale = (int)(s * artilleryBatteryTemplate.BaseMorale);
    }
    private Bounds CalculateCompanyBounds()
    {
        Vector3 p;
        Vector3 s;

        p = new Vector3(
            0,
            0,
            -1 * batteryFormation.Ranks * batteryFormation.b / 2);
        s = new Vector3(
            batteryFormation.Lines * batteryFormation.a,
            1f,
            batteryFormation.Ranks * batteryFormation.b);

        return new Bounds(p, s);
    }
}