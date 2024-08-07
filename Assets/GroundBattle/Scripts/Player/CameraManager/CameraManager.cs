using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class CameraManager : MonoBehaviour
{
    public PlayerInputActions playerControls;
    private InputAction hideUI_Key;
    private InputAction timeScale_Axis;

    [Header("Player info")]
    public string TAG = "FRA";

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
    GameObject formationProjection;
    float formationProjectionOffset = 0f;

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
            OrderProjection();
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

        //RESET PROJECTION
        Destroy(formationProjection);
        formationProjection = null;
        formationProjectionOffset = 0f;
    }
    private void OrderProjection()
    {
        //ORIENTATION ARROW
        tr.enabled = true;

        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            OrderPoint3 = hit.point;
        }

        tr.SetPosition(0, OrderPoint + (GetComponent<Camera>().transform.position - OrderPoint).normalized * 0.1f);
        tr.SetPosition(1, OrderPoint3 + (GetComponent<Camera>().transform.position - OrderPoint3).normalized * 0.1f);

        //FORMATION HOLOGRAM
        if(formationProjection == null)
        {
            float selectedWidth = 0f;
            float selectedLenght = 0f;

            formationProjection = GameObject.CreatePrimitive(PrimitiveType.Cube);
            formationProjection.GetComponent<Collider>().enabled = false;
            formationProjection.GetComponent<MeshRenderer>().material = OlogramMaterial2;

            //WIDTH
            if(selectedCompanies.Count != 0)
            {
                //CALCULATE SELECTED FORMATION WIDTH
                for (int i = 0; i < selectedCompanies.Count; i++)
                {
                    selectedWidth += companySpace + selectedCompanies[i].GetWidth();
                }
            }
            else if(selectedBattalions.Count != 0)
            {
                selectedWidth += selectedBattalions[0].battalionFormation.GetWidth();
                for (int i = 1; i < selectedBattalions.Count; i++)
                {
                    selectedWidth += battalionSpace;
                    selectedWidth += selectedBattalions[i].GetWidth();
                }
            }

            //LENGHT
            if (selectedCompanies.Count != 0)
            {
                for(int i = 0; i < selectedCompanies.Count; i++)
                {
                    float l = selectedCompanies[i].GetLenght();
                    selectedLenght = Mathf.Max(selectedLenght, l);
                }
            }
            else if (selectedBattalions.Count != 0)
            {
                for (int i = 0; i < selectedBattalions.Count; i++)
                {
                    float l = selectedBattalions[i].GetLenght();
                    selectedLenght = Mathf.Max(selectedLenght, l);
                }
            }

            //OFFSET
            if(selectedCompanies.Count != 0)
            {
                for(int i = 0; i < selectedCompanies.Count; i++)
                {
                    formationProjectionOffset = Mathf.Max(formationProjectionOffset, selectedCompanies[i].companyFormation.GetCenter().y);
                }
            }
            else if(selectedBattalions.Count != 0)
            {
                formationProjectionOffset = 0f;
            }

            formationProjection.transform.localScale = new Vector3(selectedWidth, 1f, selectedLenght);
        }

        formationProjection.transform.position = OrderPoint - Utility.V2toV3((Utility.V3toV2(OrderPoint3 - OrderPoint))).normalized * formationProjectionOffset;
        formationProjection.transform.rotation = Quaternion.LookRotation(OrderPoint3 - OrderPoint, Vector3.up);
    }

    private bool AnyUnitSelected()
    {
        bool selected;

        selected = selectedBattalions.Count != 0 || selectedArtilleryBatteries.Count != 0;
        selected |= selectedCompanies.Count != 0 && selectedCompanies.TrueForAll(s => s.IsDetached());

        return selected;
    }
}