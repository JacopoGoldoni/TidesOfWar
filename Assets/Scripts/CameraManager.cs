using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;

public class CameraManager : MonoBehaviour
{
    public InputAction playerControls;

    public List<OfficerManager> selectedCompanies = new List<OfficerManager>();
    public List<CaptainManager> selectedBattalions = new List<CaptainManager>();

    //3D UI ELEMENTS
    List<GameObject> projectionSights = new List<GameObject>();

    //SIGHTS VARIABLES
    List<float> sizes = new List<float>();
    List<Mesh> sm = new List<Mesh>();

    [Header("Player info")]
    public Factions faction = Factions.France;

    [Header("Formation")]
    public float battalionSpace = 4f;
    public float companySpace = 2f;

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

    //TIME SCALES
    public int timeScaleIndex = 1;
    public float[] timeScales = { 0.5f, 1f, 2f, 4f };

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
        if (selectedCompanies.Count != 0 || selectedBattalions.Count != 0)
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
                DeselectAllCompanies();
                DeselectAllBattalions();
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
        //HIDE OR SHOW UI
        if(Input.GetKeyDown(KeyCode.H))
        {
            uimanager.ToggleUI();
        }
        //CHANGE TIMESCALE
        if(Input.GetKeyDown(KeyCode.X))
        {
            timeScaleIndex++;
            timeScaleIndex = Mathf.Clamp(timeScaleIndex,0, 3);
            Time.timeScale = timeScales[timeScaleIndex];
        }
        else if(Input.GetKeyDown(KeyCode.Z))
        {
            timeScaleIndex--;
            timeScaleIndex = Mathf.Clamp(timeScaleIndex, 0, 3);
            Time.timeScale = timeScales[timeScaleIndex];
        }
    }

    //UNIT SELECTION FUNCTIONS
    public void SelectCompany(Transform target)
    {
        OfficerManager t = target.GetComponent<OfficerManager>();
        selectedCompanies.Add(t);
        t.Highlight(true);

        DeselectAllBattalions();

        uimanager.CompanyCommandTabCheck();
        uimanager.HighlightCompanyCard(t.companyNumber, true);
    }
    public void DeselectCompany(Transform target)
    {
        OfficerManager t = target.GetComponent<OfficerManager>();
        selectedCompanies.Remove(t);
        t.Highlight(false);

        uimanager.CompanyCommandTabCheck();
        uimanager.HighlightCompanyCard(t.companyNumber, false);
    }
    public void DeselectAllCompanies()
    {
        //Remove highlight to all units
        if (selectedCompanies.Count != 0)
        {
            foreach(OfficerManager t in selectedCompanies)
            {
                t.Highlight(false);
                uimanager.HighlightCompanyCard(t.companyNumber, false);
            }
        }

        selectedCompanies.Clear();
        uimanager.CompanyCommandTabCheck();
    }
    public void SelectBattalion(Transform target)
    {
        CaptainManager t = target.GetComponent<CaptainManager>();
        selectedBattalions.Add(t);
        t.Highlight(true);

        DeselectAllCompanies();

        uimanager.BattalionCommandTabCheck();
        uimanager.HighlightBattalionCard(t.battalionNumber, true);
    }
    public void DeselectBattalion(Transform target)
    {
        CaptainManager t = target.GetComponent<CaptainManager>();
        selectedBattalions.Remove(t);
        t.Highlight(false);

        uimanager.BattalionCommandTabCheck();
        uimanager.HighlightBattalionCard(t.battalionNumber, false);
    }
    public void DeselectAllBattalions()
    {
        //Remove highlight to all units
        if (selectedBattalions.Count != 0)
        {
            foreach (CaptainManager t in selectedBattalions)
            {
                t.Highlight(false);
                uimanager.HighlightBattalionCard(t.battalionNumber, false);
            }
        }

        selectedBattalions.Clear();
        uimanager.BattalionCommandTabCheck();
    }

    //SEND ORDERS
    private void SendMovementOrder()
    {
        Vector2 Orientation = Utility.V3toV2(OrderPoint2 - OrderPoint).normalized;

        if(selectedCompanies.Count != 0)
        {
            float comapnyFormationWidth = 0f;
            //CALCULATE SELECTED FORMATION WIDTH
            for (int i = 0; i < selectedCompanies.Count; i++)
            {
                comapnyFormationWidth += companySpace + selectedCompanies[i].GetCompanyWidth();
            }
            Debug.Log(comapnyFormationWidth);

            //COMPANY MOVEMENT ORDER
            for (int i = 0; i < selectedCompanies.Count; i++)
            {
                OfficerManager unit = selectedCompanies[i];

                float localX = 0f;
                for (int j = 0; j <= i; j++)
                {
                    if(j == i)
                    {
                        localX += companySpace + selectedCompanies[j].GetCompanyWidth() / 2f;
                    }
                    else
                    {
                        localX += companySpace + selectedCompanies[j].GetCompanyWidth();
                    }
                }

                Vector2 relativePos = UtilityMath.RotateVector2(Orientation) * (localX - comapnyFormationWidth / 2f);

                Vector2 pos = Utility.V3toV2(OrderPoint) + relativePos;


                if (unit.IsObstructedAt(pos))
                {
                    continue;
                }

                Quaternion rot = Quaternion.LookRotation(Utility.V2toV3(Orientation), Vector3.up);

                //SEND ORDER
                unit.ReceiveMovementOrder(Input.GetKey(KeyCode.LeftShift), pos, rot);

                //UPDATE PROJECTIONS
                //DeleteAllProjections();
                //ProjectAll();
            }

            ShowArrow = false;
        }
        else
        {
            float battalioFormationWidth = 0f;
            //CALCULATE SELECTED FORMATION WIDTH
            for(int i = 0; i < selectedBattalions.Count; i++)
            {
                battalioFormationWidth += battalionSpace + selectedBattalions[i].GetBattalionWidth(battalionSpace);
            }

            //BATTALION MOVEMENT ORDER
            for (int i = 0; i < selectedBattalions.Count; i++)
            {
                CaptainManager unit = selectedBattalions[i];

                float localX = 0f;
                for(int j = 0; j <= i; j++)
                {
                    if (j == i)
                    {
                        localX += battalionSpace + selectedBattalions[j].GetBattalionWidth(battalionSpace) / 2f;
                    }
                    else
                    {
                        localX += battalionSpace + selectedBattalions[j].GetBattalionWidth(battalionSpace);
                    }
                }

                Vector2 relativePos = UtilityMath.RotateVector2(Orientation) * (localX - battalioFormationWidth / 2f);

                Vector2 pos = Utility.V3toV2(OrderPoint) + relativePos;

                Quaternion rot = Quaternion.LookRotation(Utility.V2toV3(Orientation), Vector3.up);

                //SEND ORDER
                unit.ReceiveMovementOrder(Input.GetKey(KeyCode.LeftShift), pos, rot);

                //UPDATE PROJECTIONS
                //DeleteAllProjections();
                //ProjectAll();

                ShowArrow = false;
            }
        }
    }
    public void SendAttackOrder(GameObject target)
    {
        foreach(OfficerManager o in selectedCompanies)
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

            o.ReceiveMovementOrder(false, pos, rot);
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
        if(selectedCompanies.Count != 0)
        {
            //CHECK SIGHT MESHES NEEDED
            foreach(OfficerManager of in selectedCompanies)
            {
                int n = of.companyFormation.Lines;

                if(!sizes.Contains(n * 0.5f))
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
        foreach(OfficerManager o in GameUtility.GetAllCompanies())
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