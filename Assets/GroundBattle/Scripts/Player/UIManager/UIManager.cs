using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using System.Linq;

public partial class UIManager : MonoBehaviour
{
    CameraManager cameraManagerRef;

    [Header("UI - Elements")]
    public GameObject UI;
    public GameObject[] timeScaleButtons;
    GameObject NotificationTab;

    [Header("UI - Command tabs")]
    public GameObject CompanyCommandTab;
    public GameObject BattalionCommandTab;
    public GameObject ArtilleryBatteryCommandTab;
    public List<CommandButtonManager> companyCommandButtons;
    public List<CommandButtonManager> battalionCommandButtons;
    public List<CommandButtonManager> artilleryBatteryCommandButtons;

    [Header("UI - Flag parents")]
    public GameObject companyFlagParent;
    public GameObject battalionFlagParent;
    public GameObject artilleryBatteryFlagParent;

    [Header("UI - Card holders")]
    public GameObject companyCardHolder;
    public GameObject battalionCardHolder;
    public GameObject artilleryBatteryCardHolder;

    [Header("UI - Card Prefabs")]
    public GameObject CompanyCardPrefab;
    public GameObject BattalionCardPrefab;
    public GameObject ArtilleryBatteryCardPrefab;

    [Header("UI prefabs")]
    public GameObject CommandButtonPrefab;
    public GameObject NotificationTabPrefab;

    //COMMAND EVENT
    private List<Action<OfficerManager[]>> CompanyCommands = new List<Action<OfficerManager[]>>();
    private List<Action<CaptainManager[]>> BattalionCommands = new List<Action<CaptainManager[]>>();
    private List<Action<ArtilleryOfficerManager[]>> ArtilleryBatteryCommands = new List<Action<ArtilleryOfficerManager[]>>();

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Update()
    {
        UpdateFlagPosition();
    }

    public void Initialize()
    {
        cameraManagerRef = GetComponent<CameraManager>();

        InitializeCommands();

        PopulateCompanyCommandTab();
        PopulateBattalionCommandTab();
        PopulateArtilleryBatteryCommandTab();

        NotificationTab_Builder();

        CompanyCommandTabCheck();
        BattalionCommandTabCheck();
        ArtilleryBatteryCommandTabCheck();
    }
    private void InitializeCommands()
    {
        //COMPANY
        CompanyCommands.Add(UnitOrders.Company_SendLineFormation);
        CompanyCommands.Add(UnitOrders.Company_SendColumnFormation);

        CompanyCommands.Add(UnitOrders.Company_SendStopOrder);
        CompanyCommands.Add(UnitOrders.Company_SendFireAll);
        CompanyCommands.Add(UnitOrders.Company_SendHoldFire);
        CompanyCommands.Add(UnitOrders.Company_SendMelee);
        CompanyCommands.Add(UnitOrders.Company_SendMarch);
        CompanyCommands.Add((OfficerManager[] companies) => { 
            UnitOrders.Company_SendDetach(companies);
            UpdateCompanyCommandStatus();
        });

        //BATTALION
        BattalionCommands.Add(UnitOrders.Battalion_SendLineFormation);
        BattalionCommands.Add(UnitOrders.Battalion_SendColumnFormation);

        BattalionCommands.Add(UnitOrders.Battalion_SendStopOrder);
        BattalionCommands.Add(UnitOrders.Battalion_SendFireAll);
        BattalionCommands.Add(UnitOrders.Battalion_SendHoldFire);
        BattalionCommands.Add(UnitOrders.Battalion_SendMarch);

        BattalionCommands.Add(UnitOrders.Battalion_SendLightFront);
        BattalionCommands.Add(UnitOrders.Battalion_SendHeavyFront);
        BattalionCommands.Add(UnitOrders.Battalion_SendLightRear);
        BattalionCommands.Add(UnitOrders.Battalion_SendHeavyRear);
        BattalionCommands.Add(UnitOrders.Battalion_SendLightLine);
        BattalionCommands.Add(UnitOrders.Battalion_SendHeavyLine);

        //BattalionCommands.Add(UnitOrders.Battalion_SendDefend);
        //BattalionCommands.Add(UnitOrders.Battalion_SendAttack);

        //ARTILLERY BATTERY
        ArtilleryBatteryCommands.Add(UnitOrders.ArtilleryBattery_SendLineFormation);
        ArtilleryBatteryCommands.Add(UnitOrders.ArtilleryBattery_SendColumnFormation);
        ArtilleryBatteryCommands.Add(UnitOrders.ArtilleryBattery_SendStopOrder);
        ArtilleryBatteryCommands.Add(UnitOrders.ArtilleryBattery_SendFireAll);
        ArtilleryBatteryCommands.Add(UnitOrders.ArtilleryBattery_SendHoldFire);
    }
    public void ToggleUI()
    {
        if (UI.activeSelf)
        {
            UI.SetActive(false);
        }
        else
        {
            UI.SetActive(true);
        }
    }

    //UI BUILDERS
    private void NotificationTab_Builder()
    {
        GameObject notificationtab = Instantiate(NotificationTabPrefab);
        notificationtab.name = "Notification Tab";
        notificationtab.layer = 5;

        notificationtab.transform.parent = UI.transform;

        RectTransform rectTransform = notificationtab.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, -300);

        NotificationTab = notificationtab;
    }
    public void SetTimeScale(int i)
    {
        for(int j = 0; j < timeScaleButtons.Length; j++)
        {
            if(i == j)
            {
                //ACTIVE
                timeScaleButtons[j].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                //INACTIVE
                timeScaleButtons[j].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }
        }
    }
}