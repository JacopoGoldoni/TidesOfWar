using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CameraManager : MonoBehaviour
{

    private void ProjectAll()
    {
        if (selectedCompanies.Count != 0)
        {
            //CHECK SIGHT MESHES NEEDED
            foreach (OfficerManager of in selectedCompanies)
            {
                int n = of.companyFormation.Lines;

                if (!sizes.Contains(n * 0.5f))
                {
                    //IF NEEDED CREATE A NEW SIGHT MESH
                    sizes.Add(n * 0.5f);
                    sm.Add(
                        SightMesh(11, n * 0.25f, of.Range, n * 0.5f)
                        );
                }
            }

            foreach (OfficerManager o in selectedCompanies)
            {
                //DRAW PATH LINES
                o.drawPathLine = true;

                ProjectSight(o);
            }
        }
        else if(selectedBattalions.Count != 0)
        {
            //CHECK SIGHT MESHES NEEDED
            foreach(CaptainManager cm in selectedBattalions)
            {
                foreach (OfficerManager om in cm.companies)
                {
                    int n = om.companyFormation.Lines;

                    if (!sizes.Contains(n * 0.5f))
                    {
                        //IF NEEDED CREATE A NEW SIGHT MESH
                        sizes.Add(n * 0.5f);
                        sm.Add(
                            SightMesh(11, n * 0.25f, om.Range, n * 0.5f)
                            );
                    }
                }
            }
            foreach (CaptainManager cm in selectedBattalions)
            {
                foreach (OfficerManager om in cm.companies)
                {
                    //DRAW PATH LINES
                    om.drawPathLine = true;

                    ProjectSight(om);
                }
            }
        }
    }
    private void ProjectSight(OfficerManager om)
    {
        bool hasSight = false;
        //PROJECT SIGHTS
        foreach (GameObject p in projectionSights)
        {
            string pNumber = p.name.Split('_')[0];

            if (om.companyNumber == Int32.Parse(pNumber))
            {
                hasSight = true;
                break;
            }
        }

        if (!hasSight)
        {
            GameObject projectionSight = new GameObject();
            MeshFilter mf = projectionSight.AddComponent<MeshFilter>();
            MeshRenderer mr = projectionSight.AddComponent<MeshRenderer>();

            projectionSight.name = om.companyNumber + "_Sight";

            int meshIndex = sizes.FindIndex(a => a == om.companyFormation.Lines / 2f);
            mf.mesh = sm[meshIndex];

            mr.material = OlogramMaterial2;

            projectionSight.transform.position = om.transform.position;
            projectionSight.transform.rotation = om.transform.rotation;
            projectionSight.transform.parent = om.transform;

            projectionSights.Add(projectionSight);
        }
    }
    private void DeleteAllProjections()
    {
        foreach (OfficerManager o in GroundBattleUtility.GetAllCompanies())
        {
            o.drawPathLine = false;
        }

        //SIGHTS
        List<GameObject> _projectionSights = projectionSights.ToList<GameObject>();
        foreach (GameObject p in _projectionSights)
        {
            string pNumber = p.name.Split('_')[0];

            bool hasRegiment = false;
            foreach (OfficerManager o in selectedCompanies)
            {
                if (o.companyNumber == Int32.Parse(pNumber))
                {
                    hasRegiment = true;
                    break;
                }
            }

            if (!hasRegiment)
            {
                projectionSights.Remove(p);
                Destroy(p);
            }
        }
    }
    private Mesh SightMesh(int arcVertices, float baseWidth, float SightRadius, float CenterRadius)
    {
        Mesh sightMesh = new Mesh
        {
            name = "Sight mesh"
        };

        float unitAngle = Mathf.Atan(baseWidth / CenterRadius) / ((arcVertices - 1) / 2f);

        Vector3 s = Vector3.forward * -CenterRadius;

        //VERTICES
        List<Vector3> vertices = new List<Vector3>
        {
            //BASE VERTICES
            Vector3.zero,                       //0
            Vector3.right * -baseWidth,         //1
            Vector3.right * baseWidth           //2
        };

        //ARC VERTICES
        for (int i = 0; i < arcVertices; i++)
        {
            float angle = (i - (arcVertices - 1) / 2f) * -unitAngle * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 v = s + rot * Vector3.forward * (CenterRadius + SightRadius);

            vertices.Add(v);
        }

        //FACES
        List<int> faces = new List<int>
        {
            //BASE FACES
            1,
            arcVertices + 2,
            0
        };

        //ARC FACES
        for (int i = 0; i < arcVertices; i++)
        {
            faces.Add(0);
            faces.Add(2 + i + 1);
            faces.Add(2 + i);
        }

        sightMesh.vertices = vertices.ToArray();
        sightMesh.triangles = faces.ToArray();

        return sightMesh;
    }
}