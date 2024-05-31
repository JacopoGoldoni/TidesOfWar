using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class WorldMapCameraManager : MonoBehaviour
{
    public InputAction playerControls;

    public List<ArmyManager> selectedArmies = new List<ArmyManager>();

    public Factions faction = Factions.France;

    //COMPONENTS
    Camera camera;
    WorldUIManager worldUIManager;
    LineRenderer lineRenderer;


    int UILayer;

    void Start()
    {
        UILayer = LayerMask.NameToLayer("UI");

        camera = Utility.Camera;
        worldUIManager = GetComponent<WorldUIManager>();
        lineRenderer = GetComponent<LineRenderer>();

        Initialize_UI();
    }

    private void Initialize_UI()
    {
        worldUIManager.CloseRegionTab();
        worldUIManager.CloseBuildingTab();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform.gameObject.GetComponent<ArmyManager>())
                {
                    SelectArmy(hit.transform);
                }
                else
                {
                    Vector2 mapUV = WorldUtility.GetWorldUV(hit.point);
                    
                    Debug.Log("<color=#"+ ColorUtility.ToHtmlStringRGB(WorldUtility.GetWorldColor(mapUV)) + ">" + WorldUtility.GetWorldColor(mapUV) + "</color>");

                    //worldUIManager.OpenRegionTab();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            SendMovementOrder(TraceForDestination());
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            selectedArmies.Clear();
        }

        //TODO DRAW MOVEMENT PREVIEW
        if(selectedArmies.Count != 0)
        {
            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                NavMeshPath navMeshPath = new NavMeshPath();
                NavMesh.CalculatePath(selectedArmies[0].transform.position, hit.point, NavMesh.AllAreas, navMeshPath);

                lineRenderer.enabled = true;

                lineRenderer.positionCount = navMeshPath.corners.Length;
                lineRenderer.SetPositions(navMeshPath.corners);
            }
        }
        else
        {
            worldUIManager.ClearArmyCard();
            lineRenderer.enabled = false;
        }
    }

    public void SelectArmy(Transform target)
    {
        ArmyManager a = target.GetComponent<ArmyManager>();

        if(a == null)
        {
            selectedArmies.Clear();
            worldUIManager.ClearArmyCard();
            return;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            if(selectedArmies.Contains(a))
            {
                selectedArmies.Remove(a);
                worldUIManager.RemoveArmyCard(a.ID);
            }
            else
            {
                selectedArmies.Add(a);
                worldUIManager.AddArmyCard(a.ID);
            }
        }
        else
        {
            selectedArmies.Clear();
            worldUIManager.ClearArmyCard();

            selectedArmies.Add(a);
            worldUIManager.AddArmyCard(a.ID);
        }
    }
    private Vector3 TraceForDestination()
    {
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
    private void SendMovementOrder(Vector3 v3)
    {
        for(int i = 0; i < selectedArmies.Count; i++)
        {
            if(i == 0)
            {
                selectedArmies[i].MoveTo(v3);
            }
            else
            {
                NavMeshHit hit;
                NavMesh.SamplePosition( 
                    Utility.V2toV3(UtilityMath.RandomPointInDisc(2f)) + v3, 
                    out hit, 
                    Mathf.Infinity, 
                    NavMesh.AllAreas
                    );

                selectedArmies[i].MoveTo(hit.position);
            }
        }
    }
}