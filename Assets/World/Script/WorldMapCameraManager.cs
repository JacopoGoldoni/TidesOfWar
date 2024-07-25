using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class WorldMapCameraManager : MonoBehaviour
{
    public PlayerInputActions playerControls;
    private InputAction timeScale_Axis;
    private InputAction pauseButton;

    public List<ArmyManager> selectedArmies = new List<ArmyManager>();

    public Factions faction = Factions.France;

    //COMPONENTS
    Camera camera;
    WorldUIManager worldUIManager;
    LineRenderer lineRenderer;

    //REFS
    public WorldManager worldManager;

    int UILayer;

    [Header("Army management paramenters")]
    public float movementPathDisplacement = 1f;

    private void OnEnable()
    {
        playerControls.Enable();

        timeScale_Axis = playerControls.Player.TimeScale;
        timeScale_Axis.Enable();

        pauseButton = playerControls.Player.Pause;
        pauseButton.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();

        timeScale_Axis.Disable();
        pauseButton.Disable();
    }

    private void Awake()
    {
        camera = Utility.Camera;
        worldUIManager = GetComponent<WorldUIManager>();
        lineRenderer = GetComponent<LineRenderer>();

        playerControls = new PlayerInputActions();
    }

    void Start()
    {
        UILayer = LayerMask.NameToLayer("UI");

        Initialize_UI();
    }

    private void Initialize_UI()
    {
        worldUIManager.CloseRegionTab();
        worldUIManager.CloseBuildingTab();
        worldUIManager.SetTimeScale(worldManager.timeScaleIndex);
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
                    Color regionColorCode = WorldUtility.GetWorldColor(mapUV);

                    Debug.Log("<color=#"+ ColorUtility.ToHtmlStringRGB(WorldUtility.GetWorldColor(mapUV)) + ">" +
                        WorldUtility.GetWorldColor(mapUV) + "</color>");

                    Debug.Log(WorldUtility.GetProvinceByColorCode(regionColorCode).name);

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
        if (timeScale_Axis.triggered)
        {
            worldManager.timeScaleIndex += (int)timeScale_Axis.ReadValue<float>();
            worldManager.timeScaleIndex = Mathf.Clamp(worldManager.timeScaleIndex, 0, 3);

            worldUIManager.SetTimeScale(worldManager.timeScaleIndex);
        }
        if (pauseButton.triggered)
        {
            worldManager.pausedGame = !worldManager.pausedGame;
            worldUIManager.SetPausePlay(worldManager.pausedGame);
        }

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

                for(int i = 0; i < navMeshPath.corners.Length; i++)
                {
                    lineRenderer.SetPosition(i, navMeshPath.corners[i] + new Vector3(0,movementPathDisplacement,0));
                }
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
                worldUIManager.RemoveArmyCard(a.GetArmyID());
            }
            else
            {
                selectedArmies.Add(a);
                worldUIManager.AddArmyCard(a.GetArmyID());
            }
        }
        else
        {
            selectedArmies.Clear();
            worldUIManager.ClearArmyCard();

            selectedArmies.Add(a);
            worldUIManager.AddArmyCard(a.GetArmyID());
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