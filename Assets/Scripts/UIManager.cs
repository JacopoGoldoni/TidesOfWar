using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    CameraManager cameraManagerRef;

    GameObject UI;
    GameObject regimentCardHolder;
    GameObject CommandTab;

    List<GameObject> regimentCards = new List<GameObject>();

    public GameObject RegimentCardPrefab;
    public GameObject OrderButtonPrefab;

    //COMMAND EVENT
    private UnityAction LineAction;
    private UnityAction ColumnAction;

    //FLAG VARIABLES
    //(target, flag)
    List<(GameObject, GameObject)> regimentFlags = new List<(GameObject, GameObject)>();

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Update()
    {
        //UPDATE FLAG POSITIONS
        foreach((GameObject, GameObject) o in regimentFlags)
        {
            o.Item2.transform.position = Utility.Camera.WorldToScreenPoint(o.Item1.transform.position + Vector3.up * 1f);
        }
    }

    public void Initialize()
    {
        cameraManagerRef = GetComponent<CameraManager>();

        //UI CANVAS
        Canvas_Builder();

        RegimentCardHolder_Builder();

        CommandTab_Builder();

        PopulateCommandTab();

        foreach (OfficerManager of in FindObjectsOfType<OfficerManager>())
        {
            (GameObject, GameObject) tf;
            tf.Item1 = of.gameObject;
            tf.Item2 = RegimentFlag_Builder(of.gameObject);

            regimentFlags.Add(tf);
        }
    }

    //UI BUILDERS
    private void Canvas_Builder()
    {
        UI = new GameObject();
        UI.name = "UI";
        UI.layer = 5;

        RectTransform rectTransform = UI.AddComponent<RectTransform>();

        Canvas canvas = UI.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.targetDisplay = 0;

        CanvasScaler canvasScaler = UI.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        canvasScaler.scaleFactor = 1;
        canvasScaler.referencePixelsPerUnit = 100;

        GraphicRaycaster graphicRaycaster = UI.AddComponent<GraphicRaycaster>();
    }
    private void RegimentCardHolder_Builder()
    {
        GameObject RegimentCardHolder = new GameObject();
        RegimentCardHolder.name = "RegimentCardHolder";
        RegimentCardHolder.layer = 5;

        Image image = RegimentCardHolder.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.5f);
        image.transform.parent = UI.transform;

        RectTransform rectTransform1 = RegimentCardHolder.GetComponent<RectTransform>();
        rectTransform1.pivot = new Vector2(0.5f, 0.5f);
        rectTransform1.anchoredPosition = new Vector2(0, -340);
        rectTransform1.sizeDelta = new Vector2(100, 100);

        HorizontalLayoutGroup horizontalLayoutGroup = RegimentCardHolder.AddComponent<HorizontalLayoutGroup>();
        horizontalLayoutGroup.spacing = 5;
        horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        horizontalLayoutGroup.padding = new RectOffset(5,5,5,5);

        regimentCardHolder = RegimentCardHolder;

        regimentCardHolder.SetActive(false);
    }
    private void OrderButton_Build(string name, Sprite sprite, UnityAction buttonEvent)
    {
        GameObject OrderButton = Instantiate(OrderButtonPrefab);
        OrderButton.name = "CommandButton_" + name;
        OrderButton.layer = 5;

        OrderButton.GetComponent<Image>().sprite = sprite;

        OrderButton.transform.parent = CommandTab.transform;

        //ASSIGN BUTTON ONCLICK CALL
        OrderButton.GetComponent<Button>().onClick.AddListener(buttonEvent);
    }
    private void CommandTab_Builder()
    {
        CommandTab = new GameObject();
        CommandTab.name = "Command Tab";
        CommandTab.layer = 5;

        HorizontalLayoutGroup horizontalLayoutGroup = CommandTab.AddComponent<HorizontalLayoutGroup>();
        horizontalLayoutGroup.spacing = 5;
        horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        horizontalLayoutGroup.padding = new RectOffset(5, 5, 5, 5);

        Image image = CommandTab.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.5f);
        image.transform.parent = UI.transform;

        RectTransform rectTransform1 = CommandTab.GetComponent<RectTransform>();
        rectTransform1.pivot = new Vector2(0.5f, 0.5f);
        rectTransform1.anchoredPosition = new Vector2(0, -450);
        rectTransform1.sizeDelta = new Vector2(200, 100);

        CommandTab.SetActive(false);
    }
    private GameObject RegimentFlag_Builder(GameObject target)
    {
        GameObject RegimentFlag = new GameObject();
        RegimentFlag.name = "Regiment flag";
        RegimentFlag.layer = 5;

        Sprite sprite = Resources.Load<Sprite>("GFX/FRA_democratic");

        Image image = RegimentFlag.AddComponent<Image>();
        image.sprite = sprite;
        image.color = new Color(1f, 1f, 1f, 0.5f);
        image.transform.parent = UI.transform;

        RectTransform rectTransform1 = RegimentFlag.GetComponent<RectTransform>();
        rectTransform1.pivot = new Vector2(0.5f, 0.5f);
        rectTransform1.sizeDelta = new Vector2(sprite.texture.width, sprite.texture.height) / 10f;

        Button flagButton = RegimentFlag.AddComponent<Button>();
        flagButton.onClick.AddListener(() => FlagAction(target.transform));


        RegimentFlag.transform.position = Utility.Camera.WorldToScreenPoint(target.transform.position + Vector3.up * 1f);

        return RegimentFlag;
    }

    private void FlagAction(Transform target)
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            if (!cameraManagerRef.selectedOfficers.Contains(target.GetComponent<OfficerManager>()))
            {
                cameraManagerRef.SelectUnit(target.transform);
            }
            else
            {
                cameraManagerRef.DeselectUnit(target.transform);
            }
        }
        else
        {
            cameraManagerRef.DeselectAllUnit();
            cameraManagerRef.SelectUnit(target.transform);
        }
    }

    //REGIMENT CARD HOLDER MANAGEMENT
    private void RegimentCardHolderCheck()
    {
        if (regimentCards.Count > 0)
        {
            regimentCardHolder.SetActive(true);
        }
        else
        {
            regimentCardHolder.SetActive(false);
        }
    }
    public void AddRegimentCard(OfficerManager regiment)
    {
        GameObject RegimentCard = Instantiate(RegimentCardPrefab);
        RegimentCard.name = "RegimentCard_" + regiment.RegimentNumber.ToString();
        RegimentCard.layer = 5;

        RegimentCard.transform.parent = regimentCardHolder.transform;

        regimentCards.Add(RegimentCard);

        RegimentCardManager regimentCardManager = RegimentCard.GetComponent<RegimentCardManager>();
        regimentCardManager.SetAmmoSlider(regiment.Ammo, regiment.MaxAmmo);

        //Displacement
        regimentCardHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(regimentCards.Count * 100 + 10, 110);

        RegimentCardHolderCheck();
        CommandTabCheck();
    }
    public void RemoveRegimentCard(int regimentN)
    {
        foreach (Transform child in regimentCardHolder.transform)
        {
            if(child.name == "RegimentCard_" + regimentN)
            {
                regimentCards.Remove(child.gameObject);
                Destroy(child.gameObject);
                break;
            }
        }

        //Displacement
        regimentCardHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(regimentCards.Count * 100 + 10, 110);

        RegimentCardHolderCheck();
        CommandTabCheck();
    }
    public void RemoveAllRegimentCard()
    {
        foreach (Transform child in regimentCardHolder.transform)
        {
            Destroy(child.gameObject);
        }

        regimentCards.Clear();

        RegimentCardHolderCheck();
        CommandTabCheck();
    }
    
    //BUTTON FUNCTIONS
    public void SendLineFormation()
    {
        int n = cameraManagerRef.selectedOfficers.Count;
    
        for(int i = 0; i < n; i++)
        {
            Formation f = new Line(cameraManagerRef.selectedOfficers[i].pawns.Count);
            cameraManagerRef.selectedOfficers[i].SetFormation(
                f
                );
        }
    }
    public void SendColumnFormation()
    {
        int n = cameraManagerRef.selectedOfficers.Count;

        for (int i = 0; i < n; i++)
        {
            Formation f = new Column(cameraManagerRef.selectedOfficers[i].pawns.Count);
            cameraManagerRef.selectedOfficers[i].SetFormation(
                f
                );
        }
    }


    //COMMAND TAB MANAGEMENT
    private void CommandTabCheck()
    {
        if (regimentCards.Count > 0)
        {
            CommandTab.SetActive(true);
        }
        else
        {
            CommandTab.SetActive(false);
        }
    }
    private void PopulateCommandTab()
    {
        Sprite LineSprite = Resources.Load<Sprite>("GFX/TOW_Line");
        LineAction += SendLineFormation;
        OrderButton_Build("Line formation", LineSprite, LineAction);

        Sprite ColumnSprite = Resources.Load<Sprite>("GFX/TOW_Column");
        ColumnAction += SendColumnFormation;
        OrderButton_Build("Column formation", ColumnSprite, ColumnAction);
    }
}