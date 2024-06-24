using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using System.Linq;

public class UIManager : MonoBehaviour
{
    CameraManager cameraManagerRef;

    [Header("UI elements")]
    public GameObject UI;
    public GameObject companyCardHolder;
    public GameObject battalionCardHolder;
    public GameObject CompanyCommandTab;
    public GameObject BattalionCommandTab;
    public GameObject companyFlagParent;
    public GameObject battalionFlagParent;
    GameObject NotificationTab;

    //CARDS
    List<(OfficerManager, CompanyCardManager)> companyCards = new List<(OfficerManager, CompanyCardManager)>();
    List<(CaptainManager, BattalionCardManager)> battalionCards = new List<(CaptainManager, BattalionCardManager)>();

    [Header("UI prefsbs")]
    public GameObject CompanyCardPrefab;
    public GameObject BattalionCardPrefab;
    public GameObject OrderButtonPrefab;
    public GameObject NotificationTabPrefab;


    //COMMAND EVENT
    private UnityAction LineAction;
    private UnityAction ColumnAction;
    private UnityAction StopAction;

    //FLAG VARIABLES
    List<(GameObject, GameObject)> companyFlags = new List<(GameObject, GameObject)>();     //(target, flag)
    List<(GameObject, GameObject)> battalionFlags = new List<(GameObject, GameObject)>();   //(target, flag)
    [Header("Flag variables")]
    public float baseFlagScale = 0.5f;
    public float transitionDistanceFlag = 20f;
    public float transitionStrenghtFlag = 1f;
    public float flagCompanyTransparency = 0.5f;
    public float flagBattalionTransparency = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Update()
    {
        //UPDATE FLAG COMPANY POSITIONS
        foreach((GameObject, GameObject) o in companyFlags)
        {
            if(Utility.IsInView(o.Item1))
            {
                o.Item2.SetActive(true);

                float scale = 1f;

                float d = (o.Item1.transform.position - transform.position).magnitude;

                if (d < transitionDistanceFlag * 2f)
                {
                    scale =
                        baseFlagScale +
                        0.5f * UtilityMath.Sigmoid(
                            transitionStrenghtFlag * (o.Item1.transform.position - transform.position).magnitude - transitionDistanceFlag
                            );
                }

                o.Item2.GetComponent<RectTransform>().position = Utility.Camera.WorldToScreenPoint(o.Item1.transform.position + Vector3.up * 1f);
                
                RectTransform r = o.Item2.GetComponent<RectTransform>();
                Sprite s = o.Item2.GetComponent<Image>().sprite;

                r.sizeDelta = scale * s.texture.Size() / 10f;
            }
            else
            {
                o.Item2.SetActive(false);
            }
        }

        //UPDATE FLAG BATTALION POSITIONS
        foreach ((GameObject, GameObject) o in battalionFlags)
        {
            if (Utility.IsInView(o.Item1))
            {
                o.Item2.SetActive(true);

                float scale = 1f;

                float d = (o.Item1.transform.position - transform.position).magnitude;

                if (d < transitionDistanceFlag * 2f)
                {
                    scale =
                        baseFlagScale +
                        0.5f * UtilityMath.Sigmoid(
                            transitionStrenghtFlag * (o.Item1.transform.position - transform.position).magnitude - transitionDistanceFlag
                            );
                }

                o.Item2.GetComponent<RectTransform>().position = Utility.Camera.WorldToScreenPoint(o.Item1.transform.position + Vector3.up * 1f);

                RectTransform r = o.Item2.GetComponent<RectTransform>();
                Sprite s = o.Item2.GetComponent<Image>().sprite;

                r.sizeDelta = scale * s.texture.Size() / 10f;
            }
            else
            {
                o.Item2.SetActive(false);
            }
        }

        //UPDATE COMPANY CARD INFOS
        foreach ((OfficerManager, CompanyCardManager) r in companyCards)
        {
            r.Item2.SetAmmoSlider(r.Item1.Ammo, r.Item1.MaxAmmo);
        }
    }

    public void Initialize()
    {
        cameraManagerRef = GetComponent<CameraManager>();

        PopulateCompanyCommandTab();
        PopulateBattalionCommandTab();
        NotificationTab_Builder();

        CompanyCommandTabCheck();
        BattalionCommandTabCheck();
    }

    //UI BUILDERS
    private GameObject OrderButton_Build(string name, Sprite sprite, UnityAction buttonEvent)
    {
        GameObject OrderButton = Instantiate(OrderButtonPrefab);
        OrderButton.name = "CommandButton_" + name;
        OrderButton.layer = 5;

        OrderButton.GetComponent<Image>().sprite = sprite;

        //ASSIGN BUTTON ONCLICK CALL
        OrderButton.GetComponent<Button>().onClick.AddListener(buttonEvent);

        return OrderButton;
    }
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

    //FLAG MANAGEMENT
    private GameObject CompanyFlag_Builder(GameObject target)
    {
        GameObject RegimentFlag = new GameObject();
        RegimentFlag.name = "Regiment flag";
        RegimentFlag.layer = 5;

        RegimentFlag.AddComponent<RegimentFlagManager>();

        Factions regimentFaction = target.GetComponent<OfficerManager>().faction;

        Sprite sprite = GFXUtility.GetFlag(regimentFaction);

        Image image = RegimentFlag.AddComponent<Image>();
        image.sprite = sprite;
        image.color = new Color(1f, 1f, 1f, flagCompanyTransparency);
        image.transform.parent = UI.transform;

        RectTransform rectTransform1 = RegimentFlag.GetComponent<RectTransform>();
        rectTransform1.pivot = new Vector2(0.5f, 0.5f);
        rectTransform1.sizeDelta = new Vector2(sprite.texture.width, sprite.texture.height) / 10f;
        rectTransform1.parent = companyFlagParent.transform;

        if (regimentFaction == Utility.Camera.GetComponent<CameraManager>().faction)
        {
            Button flagButton = RegimentFlag.AddComponent<Button>();
            flagButton.onClick.AddListener(() => CompanyFlagAction(target.transform));
        }


        RegimentFlag.transform.position = Utility.Camera.WorldToScreenPoint(target.transform.position + Vector3.up * 1f);

        return RegimentFlag;
    }
    private GameObject BattalionFlag_Builder(GameObject target)
    {
        GameObject BattalionFlag = new GameObject();
        BattalionFlag.name = "Battalion flag";
        BattalionFlag.layer = 5;

        BattalionFlag.AddComponent<RegimentFlagManager>();

        Factions battalionFaction = target.GetComponent<CaptainManager>().faction;

        Sprite sprite = GFXUtility.GetFlag(battalionFaction);

        Image image = BattalionFlag.AddComponent<Image>();
        image.sprite = sprite;
        image.color = new Color(1f, 1f, 1f, flagBattalionTransparency);
        image.transform.parent = UI.transform;

        RectTransform rectTransform1 = BattalionFlag.GetComponent<RectTransform>();
        rectTransform1.pivot = new Vector2(0.5f, 0.5f);
        rectTransform1.sizeDelta = new Vector2(sprite.texture.width, sprite.texture.height) / 10f;
        rectTransform1.parent = battalionFlagParent.transform;

        if (battalionFaction == Utility.Camera.GetComponent<CameraManager>().faction)
        {
            Button flagButton = BattalionFlag.AddComponent<Button>();
            flagButton.onClick.AddListener(() => BattalionFlagAction(target.transform));
        }


        BattalionFlag.transform.position = Utility.Camera.WorldToScreenPoint(target.transform.position + Vector3.up * 1f);

        return BattalionFlag;
    }
    private void CompanyFlagAction(Transform target)
    {
        //SELECT COMPANY
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!cameraManagerRef.selectedCompanies.Contains(target.GetComponent<OfficerManager>()))
            {
                cameraManagerRef.SelectCompany(target.transform);
            }
            else
            {
                cameraManagerRef.DeselectCompany(target.transform);
            }
        }
        else
        {
            cameraManagerRef.DeselectAllCompanies();
            cameraManagerRef.SelectCompany(target.transform);
        }
    }
    private void BattalionFlagAction(Transform target)
    {
        //SELECT BATTALION
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!cameraManagerRef.selectedCompanies.Contains(target.GetComponent<OfficerManager>()))
            {
                cameraManagerRef.SelectBattalion(target.transform);
            }
            else
            {
                cameraManagerRef.DeselectBattalion(target.transform);
            }
        }
        else
        {
            cameraManagerRef.DeselectAllBattalions();
            cameraManagerRef.SelectBattalion(target.transform);
        }
    }
    public void AppendCompanyFlag(OfficerManager of)
    {
        (GameObject, GameObject) tf;
        tf.Item1 = of.gameObject;
        tf.Item2 = CompanyFlag_Builder(of.gameObject);
        companyFlags.Add(tf);
    }
    public void AppendBattalionFlag(CaptainManager ca)
    {
        (GameObject, GameObject) tf;
        tf.Item1 = ca.gameObject;
        tf.Item2 = BattalionFlag_Builder(ca.gameObject);
        battalionFlags.Add(tf);
    }

    //COMPANY CARD HOLDER MANAGEMENT
    private void CompanyCardHolderCheck()
    {
        if (companyCards.Count > 0)
        {
            companyCardHolder.SetActive(true);
        }
        else
        {
            companyCardHolder.SetActive(false);
        }
    }
    public void AddCompanyCard(OfficerManager company)
    {
        GameObject CompanyCard = Instantiate(CompanyCardPrefab);
        CompanyCard.name = "CompanyCard_" + company.companyNumber.ToString();
        CompanyCard.layer = 5;

        CompanyCard.transform.parent = companyCardHolder.transform;

        CompanyCardManager companyCardManager = CompanyCard.GetComponent<CompanyCardManager>();
        companyCardManager.SetAmmoSlider(company.Ammo, company.MaxAmmo);
        companyCardManager.Initialize(company.faction);

        companyCards.Add((company, companyCardManager));

        //Displacement
        Vector2 cardSize = CompanyCardPrefab.GetComponent<RectTransform>().sizeDelta;
        companyCardHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(companyCards.Count * cardSize.x + 10, cardSize.y + 10);

        CompanyCardHolderCheck();
    }
    public void RemoveCompanyCard(int regimentN)
    {
        foreach (Transform child in companyCardHolder.transform)
        {
            if(child.name == "RegimentCard_" + regimentN)
            {
                companyCards.Remove(companyCards.Find(a => a.Item2 == child));
                Destroy(child.gameObject);
                break;
            }
        }

        //Displacement
        companyCardHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(companyCards.Count * 100 + 10, 110);

        CompanyCardHolderCheck();
        CompanyCommandTabCheck();
    }
    public void RemoveAllCompanyCard()
    {
        foreach (Transform child in companyCardHolder.transform)
        {
            Destroy(child.gameObject);
        }

        companyCards.Clear();

        CompanyCardHolderCheck();
        CompanyCommandTabCheck();
    }
    public void HighlightCompanyCard(int companyID, bool highlight)
    {
        for(int i = 0; i < companyCards.Count; i++)
        {
            if(companyCards[i].Item1.companyNumber == companyID)
            {
                companyCards[i].Item2.HighLight(highlight);
                return;
            }
        }
    }
    public void HighlightBattalionCard(int battalionID, bool highlight)
    {
        for (int i = 0; i < battalionCards.Count; i++)
        {
            if (battalionCards[i].Item1.battalionNumber == battalionID)
            {
                battalionCards[i].Item2.HighLight(highlight);
                return;
            }
        }
    }

    //BATTALION CARD HOLDER MANAGEMENT
    private void BattalionCardHolderCheck()
    {
        if (companyCards.Count > 0)
        {
            battalionCardHolder.SetActive(true);
        }
        else
        {
            battalionCardHolder.SetActive(false);
        }
    }
    public void AddBattalionCard(CaptainManager battalion)
    {
        GameObject BattalionCard = Instantiate(BattalionCardPrefab);
        BattalionCard.name = "BattalionCard_" + battalion.battalionNumber.ToString();
        BattalionCard.layer = 5;

        BattalionCard.transform.parent = battalionCardHolder.transform;

        BattalionCardManager battalionCardManager = BattalionCard.GetComponent<BattalionCardManager>();
        battalionCardManager.Initialize(battalion.faction);

        battalionCards.Add((battalion, battalionCardManager));

        //Displacement
        Vector2 cardSize = BattalionCardPrefab.GetComponent<RectTransform>().sizeDelta;
        battalionCardHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(battalionCards.Count * cardSize.x + 10, cardSize.y + 10);

        CompanyCardHolderCheck();
    }
    public void RemoveBattalionCard(int battalionN)
    {
        foreach (Transform child in battalionCardHolder.transform)
        {
            if (child.name == "BattalionCard_" + battalionN)
            {
                battalionCards.Remove(battalionCards.Find(a => a.Item2 == child));
                Destroy(child.gameObject);
                break;
            }
        }

        //Displacement
        battalionCardHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(battalionCards.Count * 100 + 10, 110);

        BattalionCardHolderCheck();
        CompanyCommandTabCheck();
    }
    public void RemoveAllBattalionCard()
    {
        foreach (Transform child in battalionCardHolder.transform)
        {
            Destroy(child.gameObject);
        }

        battalionCards.Clear();

        BattalionCardHolderCheck();
        CompanyCommandTabCheck();
    }

    //BUTTON FUNCTIONS
    public void SendLineFormation()
    {
        if(cameraManagerRef.selectedBattalions.Count != 0)
        {
            //BATTALION ORDER
            int n = cameraManagerRef.selectedBattalions.Count;

            for (int i = 0; i < n; i++)
            {
                CaptainManager cm = cameraManagerRef.selectedBattalions[i];
                Formation f = new Line(cm.battallionSize);
                f.SetSizeByRanks(cm.battallionSize, 1);
                f.a = cm.companies[1].companyFormation.Lines * cm.companies[1].companyFormation.a + 1f;
                f.b = 0;
                cm.SetFormation(f);
                cm.ReceiveMovementOrder(false, Utility.V3toV2(cm.transform.position), cm.transform.rotation);
            }
        }
        else
        {
            //COMPANY ORDER
            int n = cameraManagerRef.selectedCompanies.Count;

            for (int i = 0; i < n; i++)
            {
                OfficerManager of = cameraManagerRef.selectedCompanies[i];
                Formation f = new Line(of.companySize);
                of.SetFormation(f);
                of.ReceiveMovementOrder(false, Utility.V3toV2(of.transform.position), of.transform.rotation);
            }
        }
    }
    public void SendColumnFormation()
    {
        if (cameraManagerRef.selectedBattalions.Count != 0)
        {
            //BATTALION ORDER
            int n = cameraManagerRef.selectedBattalions.Count;

            for (int i = 0; i < n; i++)
            {
                CaptainManager cm = cameraManagerRef.selectedBattalions[i];
                Formation f = new Column(cm.battallionSize);
                f.SetSizeByLines(cm.battallionSize, 2);

                float width = 0;

                //GET LARGEST COMPANY
                for(int j = 0; j < cm.companies.Count;j++)
                {
                    float w = cm.companies[j].companyFormation.Lines * cm.companies[j].companyFormation.a;
                    if(w > width)
                    {
                        width = w;
                    }
                }

                f.a = width + 1f;
                f.b = 5;
                cm.SetFormation(f);
                cm.ReceiveMovementOrder(false, Utility.V3toV2(cm.transform.position), cm.transform.rotation);
            }
        }
        else
        {
            //COMPANY ORDER
            int n = cameraManagerRef.selectedCompanies.Count;

            for (int i = 0; i < n; i++)
            {
                OfficerManager of = cameraManagerRef.selectedCompanies[i];
                Formation f = new Column(of.companySize);
                of.SetFormation(f);
                of.ReceiveMovementOrder(false, Utility.V3toV2(of.transform.position), of.transform.rotation);
            }
        }
    }
    public void SendStopOrder()
    {
        int n = cameraManagerRef.selectedCompanies.Count;

        for (int i = 0; i < n; i++)
        {
            OfficerManager om = cameraManagerRef.selectedCompanies[i];
            om.um.SetDestination(Utility.V3toV2( om.transform.position), om.transform.rotation);
        }
    }

    //COMMAND TAB MANAGEMENT
    public void CompanyCommandTabCheck()
    {
        if (cameraManagerRef.selectedCompanies.Count > 0)
        {
            if(cameraManagerRef.selectedCompanies.Any(c => c.IsDetached()))
            {
                CompanyCommandTab.SetActive(true);
            }
            else
            {
                CompanyCommandTab.SetActive(false);
            }
        }
        else
        {
            CompanyCommandTab.SetActive(false);
        }
    }
    public void BattalionCommandTabCheck()
    {
        if (cameraManagerRef.selectedBattalions.Count > 0)
        {
            BattalionCommandTab.SetActive(true);
        }
        else
        {
            BattalionCommandTab.SetActive(false);
        }
    }
    private void PopulateCompanyCommandTab()
    {
        Sprite LineSprite = Resources.Load<Sprite>("GFX/TOW_Line");
        LineAction += SendLineFormation;
        OrderButton_Build("Line formation", LineSprite, LineAction).transform.parent = CompanyCommandTab.transform;

        Sprite ColumnSprite = Resources.Load<Sprite>("GFX/TOW_Column");
        ColumnAction += SendColumnFormation;
        OrderButton_Build("Column formation", ColumnSprite, ColumnAction).transform.parent = CompanyCommandTab.transform;

        Sprite StopSprite = Resources.Load<Sprite>("GFX/TOW_Column");
        StopAction += SendStopOrder;
        OrderButton_Build("Stop", StopSprite, StopAction).transform.parent = CompanyCommandTab.transform;

        int n = CompanyCommandTab.transform.childCount;

        RectTransform rectTransform = CompanyCommandTab.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(n * 100, 100);
    }
    private void PopulateBattalionCommandTab()
    {
        Sprite LineSprite = Resources.Load<Sprite>("GFX/TOW_Line");
        LineAction += SendLineFormation;
        OrderButton_Build("Line formation", LineSprite, LineAction).transform.parent = BattalionCommandTab.transform;

        Sprite ColumnSprite = Resources.Load<Sprite>("GFX/TOW_Column");
        ColumnAction += SendColumnFormation;
        OrderButton_Build("Column formation", ColumnSprite, ColumnAction).transform.parent = BattalionCommandTab.transform;

        Sprite StopSprite = Resources.Load<Sprite>("GFX/TOW_Column");
        StopAction += SendStopOrder;
        OrderButton_Build("Stop", StopSprite, StopAction).transform.parent = BattalionCommandTab.transform;

        int n = BattalionCommandTab.transform.childCount;

        RectTransform rectTransform = BattalionCommandTab.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(n * 100, 100);
    }

    public void OnFlagRClicked(RegimentFlagManager regimentFlagManager)
    {
        GameObject flagSender = regimentFlagManager.gameObject;

        foreach ((GameObject, GameObject) a in companyFlags)
        {
            if(a.Item2 == flagSender.gameObject)
            {
                cameraManagerRef.SendAttackOrder(a.Item1);
                break;
            }
        }
    }
}