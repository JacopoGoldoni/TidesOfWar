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
    private List<Action> CompanyCommands = new List<Action>();
    private List<Action> BattalionCommands = new List<Action>();
    private List<Action> ArtilleryBatteryCommands = new List<Action>();

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Update()
    {
        UpdateFlagPosition();

        //UPDATE COMPANY CARD INFOS
        foreach ((OfficerManager, CompanyCardManager) r in companyCards)
        {
            r.Item2.SetAmmoSlider(r.Item1.Ammo, r.Item1.MaxAmmo);
        }
    }

    public void Initialize()
    {
        cameraManagerRef = GetComponent<CameraManager>();

        InitializeCommands();

        PopulateCompanyCommandTab();
        PopulateBattalionCommandTab();

        NotificationTab_Builder();

        CompanyCommandTabCheck();
        BattalionCommandTabCheck();
    }
    public void InitializeCommands()
    {
        //COMPANY
        CompanyCommands.Add(SendLineFormation);
        CompanyCommands.Add(SendColumnFormation);
        CompanyCommands.Add(SendStopOrder);
        CompanyCommands.Add(SendFireAll);
        CompanyCommands.Add(SendHoldFire);
        CompanyCommands.Add(SendMelee);
        CompanyCommands.Add(SendMarch);

        //BATTALION
        BattalionCommands.Add(SendLineFormation);
        BattalionCommands.Add(SendColumnFormation);

        BattalionCommands.Add(SendStopOrder);
        BattalionCommands.Add(SendFireAll);
        BattalionCommands.Add(SendHoldFire);
        BattalionCommands.Add(SendMarch);

        BattalionCommands.Add(SendLightFront);
        BattalionCommands.Add(SendHeavyFront);
        BattalionCommands.Add(SendLightRear);
        BattalionCommands.Add(SendHeavyRear);
        BattalionCommands.Add(SendLightLine);
        BattalionCommands.Add(SendHeavyLine);

        BattalionCommands.Add(SendDefend);
        BattalionCommands.Add(SendAttack);

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