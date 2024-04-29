using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CameraManager : MonoBehaviour
{
    public InputAction playerControls;

    public List<OfficerManager> selectedOfficers = new List<OfficerManager>();
    List<GameObject> projectionDests = new List<GameObject>();
    List<GameObject> projectionSights = new List<GameObject>();

    public Factions faction = Factions.France;

    //COMPONENTS
    Camera camera;
    UIManager uimanager;
    LineRenderer tr;

    Vector3 OrderPoint = new Vector3(0, 0, 0);
    Vector3 OrderPoint2 = new Vector3(0, 0, 0);
    Vector3 OrderPoint3 = new Vector3(0, 0, 0);
    bool ShowArrow = false;

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Start()
    {
        camera = Utility.Camera;
        uimanager = GetComponent<UIManager>();

        tr = GetComponent<LineRenderer>();
        tr.widthMultiplier = 0.5f;
    }

    private void Update()
    {
        //If any unit is selected
        if (selectedOfficers.Count != 0)
        {
            //Chose a destination
            if (Input.GetMouseButtonDown(1))
            {
                TraceForDestination();
                ShowArrow = true;
            }
            //And orientantion
            if (Input.GetMouseButtonUp(1))
            {
                TraceForOrientation();

                SendOrder();
            }
            //Deselect all with E
            if(Input.GetKey(KeyCode.E))
            {
                DeselectAllUnit();
            }
        }

        if (ShowArrow)
        {
            tr.enabled = true;

            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                OrderPoint3 = hit.point;
            }

            tr.SetPosition(0, OrderPoint + (GetComponent<Camera>().transform.position - OrderPoint).normalized * 0.1f);
            tr.SetPosition(1, OrderPoint3 + (GetComponent<Camera>().transform.position - OrderPoint3).normalized * 0.1f);
        }
        else
        {
            tr.enabled = false;
        }

        DeleteAllProjections();
        ProjectAll();
    }

    //UNIT SELECTION FUNCTIONS
    public void SelectUnit(Transform target)
    {
        OfficerManager t = target.GetComponent<OfficerManager>();
        selectedOfficers.Add(t);
        t.Highlight(true);

        uimanager.AddRegimentCard(t);
    }
    public void DeselectUnit(Transform target)
    {
        OfficerManager t = target.GetComponent<OfficerManager>();
        selectedOfficers.Remove(t);
        t.Highlight(false);

        uimanager.RemoveRegimentCard(t.RegimentNumber);
    }
    public void DeselectAllUnit()
    {
        //Remove highlight to all units
        if (selectedOfficers.Count != 0)
        {
            foreach(OfficerManager t in selectedOfficers)
            {
                t.Highlight(false);
            }
        }

        uimanager.RemoveAllRegimentCard();

        selectedOfficers.Clear();
    }

    private void SendOrder()
    {
        Vector2 Orientation = Utility.V3toV2(OrderPoint2 - OrderPoint).normalized;

        for (int i = 0; i < selectedOfficers.Count; i++)
        {
            OfficerManager unit = selectedOfficers[i];

            float space = 1f;

            Vector2 pos = new Vector2(OrderPoint.x, OrderPoint.z) + UtilityMath.RotateVector2(Orientation) * (((float)i - (float)(selectedOfficers.Count - 1) * 0.5f) * (unit.RegimentFormation.Lines / 2f + space));

            Quaternion rot = Quaternion.LookRotation(Utility.V2toV3(Orientation), Vector3.up);

            if(Input.GetKey(KeyCode.LeftShift))
            {
                //ADD ORDER
                unit.um.AddDestination(pos, rot);
            }
            else
            {
                //SET ORDER
                unit.um.SetDestination(pos, rot);
            }

            //UPDATE PROJECTIONS
            DeleteAllProjections();
            ProjectAll();

            ShowArrow = false;
        }
    }

    //TRACE FUNCIONS
    private void TraceForDestination()
    {
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            OrderPoint = hit.point;
        }
    }
    private void TraceForOrientation()
    {
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            OrderPoint2 = hit.point;
        }
    }


    //PROJECTIONS
    private void ProjectAll()
    {
        List<float> sizes = new List<float>();
        List<Mesh> sm = new List<Mesh>();
        
        if(selectedOfficers.Count != 0)
        {
            foreach(OfficerManager of in selectedOfficers)
            {
                int n = of.RegimentFormation.Lines;

                if(!sizes.Contains(n / 2f))
                {
                    sizes.Add(n / 2f);
                    sm.Add(
                        SightMesh(11, n / 4f, of.Range, n * 0.5f)
                        );
                }
            }
        }

        foreach (OfficerManager o in selectedOfficers)
        {
            //PROJECT DESTINATIONS
            if(o.um.MovementPoints.Count != 0)
            {
                GameObject projectionDest = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                projectionDest.GetComponent<MeshRenderer>().material = Resources.Load("OlogramMaterial", typeof(Material)) as Material;
                projectionDest.GetComponent<Collider>().enabled = false;

                projectionDest.transform.position = o.um.MovementPoints[0].pos;

                projectionDests.Add(projectionDest);
            }

            //PROJECT SIGHTS
            GameObject projectionSight = new GameObject();
            MeshFilter mf = projectionSight.AddComponent<MeshFilter>();
            MeshRenderer mr = projectionSight.AddComponent<MeshRenderer>();

            int meshIndex = sizes.FindIndex(a => a == o.RegimentFormation.Lines / 2f);
            mf.mesh = sm[meshIndex];

            mr.material = Resources.Load("OlogramMaterial2", typeof(Material)) as Material;
            projectionSight.transform.position = o.transform.position;
            projectionSight.transform.rotation = o.transform.rotation;
            projectionSight.transform.parent = o.transform;

            projectionDests.Add(projectionSight);
        }
    }
    private void DeleteAllProjections()
    {
        foreach(GameObject p in projectionDests)
        {
            Destroy(p);
        }
        projectionDests.Clear();

        foreach (GameObject p in projectionSights)
        {
            Destroy(p);
        }
        projectionSights.Clear();
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
        List<Vector3> vertices = new List<Vector3>();

        //BASE VERTICES
        vertices.Add(Vector3.zero);                     //0
        vertices.Add(Vector3.right * -baseWidth);       //1
        vertices.Add(Vector3.right * baseWidth);        //2

        //ARC VERTICES
        for (int i = 0; i < arcVertices; i++)
        {
            float angle = (i - (arcVertices - 1) / 2f) * -unitAngle * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 v = s + rot * Vector3.forward * (CenterRadius + SightRadius);

            vertices.Add(v);
        }

        //FACES
        List<int> faces = new List<int>();

        //BASE FACES
        faces.Add(1);
        faces.Add(arcVertices + 2);
        faces.Add(0);

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