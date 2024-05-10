using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Den.Tools.GUI.Popup;
using System;

public class CameraManager : MonoBehaviour
{
    public InputAction playerControls;

    public List<OfficerManager> selectedOfficers = new List<OfficerManager>();

    //3D UI ELEMENTS
    List<GameObject> projectionSights = new List<GameObject>();

    //SIGHTS VARIABLES
    List<float> sizes = new List<float>();
    List<Mesh> sm = new List<Mesh>();

    public Factions faction = Factions.France;

    //COMPONENTS
    Camera camera;
    UIManager uimanager;
    LineRenderer tr;

    Material OlogramMaterial;
    Material OlogramMaterial2;

    Vector3 OrderPoint = new Vector3(0, 0, 0);
    Vector3 OrderPoint2 = new Vector3(0, 0, 0);
    Vector3 OrderPoint3 = new Vector3(0, 0, 0);
    bool ShowArrow = false;

    int UILayer;

    //NOTIFICATION
    EventBinding<NotificationEvent> notificationBinding;


    private void OnEnable()
    {
        playerControls.Enable();

        //BIND NOTIFICATION BUS
        notificationBinding = new EventBinding<NotificationEvent>(() => { return; });
        EventBus<NotificationEvent>.Register(notificationBinding);
    }

    private void OnDisable()
    {
        playerControls.Disable();

        //UNBIND NOTIFICATION BUS
        EventBus<NotificationEvent>.Deregister(notificationBinding);
    }

    private void Start()
    {
        UILayer = LayerMask.NameToLayer("UI");

        camera = Utility.Camera;
        uimanager = GetComponent<UIManager>();

        tr = GetComponent<LineRenderer>();
        tr.widthMultiplier = 0.5f;

        OlogramMaterial = Resources.Load("OlogramMaterial", typeof(Material)) as Material;
        OlogramMaterial2 = Resources.Load("OlogramMaterial2", typeof(Material)) as Material;
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

                SendMovementOrder();
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

        //NOTIFICATION TEST
        if(Input.GetKeyDown(KeyCode.L))
        {
            EventBus<NotificationEvent>.Raise(new NotificationEvent
            {
                name = "Notification",
                description = "Test description",
                duration = 2f
            });
        }
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

    private void SendMovementOrder()
    {
        Vector2 Orientation = Utility.V3toV2(OrderPoint2 - OrderPoint).normalized;

        for (int i = 0; i < selectedOfficers.Count; i++)
        {
            OfficerManager unit = selectedOfficers[i];

            float space = 1f;

            Vector2 pos = 
                new Vector2(OrderPoint.x, OrderPoint.z) + 
                UtilityMath.RotateVector2(Orientation) * (((float)i - (float)(selectedOfficers.Count - 1) * 0.5f) * (unit.RegimentFormation.Lines / 2f + space));

            Quaternion rot = Quaternion.LookRotation(Utility.V2toV3(Orientation), Vector3.up);

            //SEND ORDER
            unit.SendOrder(Input.GetKey(KeyCode.LeftShift), pos, rot);

            //UPDATE PROJECTIONS
            DeleteAllProjections();
            ProjectAll();

            ShowArrow = false;
        }
    }
    public void SendAttackOrder(GameObject target)
    {
        foreach(OfficerManager o in selectedOfficers)
        {
            Vector2 pos;
            if ((o.transform.position - target.transform.position).magnitude >= o.Range * 0.75f)
            {
                pos = Utility.V3toV2(target.transform.position + (o.transform.position - target.transform.position).normalized * o.Range * 0.75f);
            }
            else
            {
                pos = o.transform.position;
            }
            Quaternion rot = Quaternion.LookRotation((target.transform.position - o.transform.position).normalized, Vector3.up);

            o.SendOrder(false, pos, rot);
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
        if(selectedOfficers.Count != 0)
        {
            //CHECK SIGHT MESHES NEEDED
            foreach(OfficerManager of in selectedOfficers)
            {
                int n = of.RegimentFormation.Lines;

                if(!sizes.Contains(n * 0.5f))
                {
                    //IF NEEDED CREATE A NEW SIGHT MESH
                    sizes.Add(n * 0.5f);
                    sm.Add(
                        SightMesh(11, n * 0.25f, of.Range, n * 0.5f)
                        );
                }
            }

            foreach (OfficerManager o in selectedOfficers)
            {
                //DRAW PATH LINES
                o.drawPathLine = true;

                ProjectSight(o);
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

            if (om.RegimentNumber == Int32.Parse(pNumber))
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

            projectionSight.name = om.RegimentNumber + "_Sight";

            int meshIndex = sizes.FindIndex(a => a == om.RegimentFormation.Lines / 2f);
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
        foreach(OfficerManager o in GameUtility.FindAllRegiments())
        {
            o.drawPathLine = false;
        }

        //SIGHTS
        foreach (GameObject p in projectionSights)
        {
            string pNumber = p.name.Split('_')[0];
            
            bool hasRegiment = false;
            foreach (OfficerManager o in selectedOfficers)
            {
                if (o.RegimentNumber == Int32.Parse(pNumber))
                {
                    hasRegiment = true;
                    break;
                }
            }

            if(!hasRegiment)
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