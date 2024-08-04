using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class CameraManager : MonoBehaviour
{
    public PlayerInputActions playerControls;
    private InputAction hideUI_Key;
    private InputAction timeScale_Axis;

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
    public float artilleryBatterySpace = 4f;

    //COMPONENTS
    public UIManager uimanager;
    LineRenderer tr;

    Material OlogramMaterial;
    Material OlogramMaterial2;

    Vector3 OrderPoint = new Vector3(0, 0, 0);
    Vector3 OrderPoint2 = new Vector3(0, 0, 0);
    Vector3 OrderPoint3 = new Vector3(0, 0, 0);
    bool ShowOrderArrow = false;

    int UILayer;

    //NOTIFICATION
    EventBinding<NotificationEvent> notificationBinding;

    //TIME SCALES
    public int timeScaleIndex = 1;
    public float[] timeScales = { 0.5f, 1f, 2f, 4f };
    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        playerControls.Enable();

        hideUI_Key = playerControls.Player.HideUI;
        hideUI_Key.Enable();

        timeScale_Axis = playerControls.Player.TimeScale;
        timeScale_Axis.Enable();

        //BIND NOTIFICATION BUS
        notificationBinding = new EventBinding<NotificationEvent>(() => { return; });
        EventBus<NotificationEvent>.Register(notificationBinding);
    }

    private void OnDisable()
    {
        playerControls.Disable();

        hideUI_Key.Disable();
        timeScale_Axis.Disable();

        //UNBIND NOTIFICATION BUS
        EventBus<NotificationEvent>.Deregister(notificationBinding);
    }

    private void Start()
    {
        UILayer = LayerMask.NameToLayer("UI");

        uimanager = GetComponent<UIManager>();

        tr = GetComponent<LineRenderer>();
        tr.widthMultiplier = 0.5f;

        OlogramMaterial = Resources.Load("OlogramMaterial", typeof(Material)) as Material;
        OlogramMaterial2 = Resources.Load("OlogramMaterial2", typeof(Material)) as Material;

        Time.timeScale = timeScales[timeScaleIndex];
        uimanager.SetTimeScale(timeScaleIndex);
    }

    private void Update()
    {
        //If any unit is selected
        if (AnyUnitSelected())
        {
            //Chose a destination
            if (Input.GetMouseButtonDown(1))
            {
                TraceForDestination();
                ShowOrderArrow = true;
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
                DeselectAllArtilleryBatteries();
            }
        }

        if (ShowOrderArrow)
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
        if(hideUI_Key.triggered)
        {
            uimanager.ToggleUI();
        }
        //CHANGE TIMESCALE
        if(timeScale_Axis.triggered)
        {
            timeScaleIndex += (int)timeScale_Axis.ReadValue<float>();
            timeScaleIndex = Mathf.Clamp(timeScaleIndex,0, 3);

            Time.timeScale = timeScales[timeScaleIndex];
            uimanager.SetTimeScale(timeScaleIndex);
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

    private bool AnyUnitSelected()
    {
        return selectedCompanies.Count != 0 || selectedBattalions.Count != 0 || selectedArtilleryBatteries.Count != 0;
    }
}