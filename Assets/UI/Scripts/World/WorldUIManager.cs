using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldUIManager : MonoBehaviour
{
    GameObject UI;
    public GameObject selectedArmiesTab;
    List<GameObject> selectedArmiesCards = new List<GameObject>();

    [Header("UI prefab")]
    public GameObject armyCard;
    public GameObject buildingGridSlotPrefab;

    [Header("UI references")]
    public GameObject regionTab;
    public GameObject buildingTab;
    public GameObject buildingText;
    public GameObject buildindGrid;
    public GameObject DateTimeText;
    public GameObject[] timeScaleButtons;
    public GameObject[] PlayPauseButton;

    private void Awake()
    {
        UI = GameObject.Find("UI");
    }

    public void Initialize()
    {
        Initialize_BuildingTab();
        OpenBuildingTab();
    }

    //TIME
    public void SetDateTime(DateTime dateTime)
    {
        TextMeshProUGUI tmp = DateTimeText.GetComponent<TextMeshProUGUI>();
        tmp.text = dateTime.ToString("HH") + ":00 " + dateTime.ToString("MMMM") + " " + dateTime.Day + ", " + dateTime.Year;
    }
    public void SetTimeScale(int i)
    {
        for (int j = 0; j < timeScaleButtons.Length; j++)
        {
            if (i == j)
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
    public void SetPausePlay(bool isPaused)
    {
        if(isPaused)
        {
            PlayPauseButton[1].SetActive(true);
            PlayPauseButton[0].SetActive(false);
        }
        else
        {
            PlayPauseButton[0].SetActive(true);
            PlayPauseButton[1].SetActive(false);
        }
    }

    //ARMY CARD
    public void AddArmyCard(int armyID)
    {
        GameObject go = Instantiate(armyCard);

        go.transform.parent = selectedArmiesTab.transform;
        go.GetComponent<SelectedArmyCardManager>().ArmyID = armyID;
        //TODO add army name and flag
        //go.GetComponent<SelectedArmyCardManager>().PopulateCard("Army", null);

        selectedArmiesCards.Add(go);

        UpdateArmyTab();
    }
    public void RemoveArmyCard(int armyID)
    {
        foreach(GameObject g in selectedArmiesCards)
        {
            SelectedArmyCardManager s = g.GetComponent<SelectedArmyCardManager>();
            if(s.ArmyID == armyID)
            {
                selectedArmiesCards.Remove(g);
                Destroy(g);
                return;
            }
        }

        UpdateArmyTab();
    }
    public void ClearArmyCard()
    {
        foreach(GameObject g in selectedArmiesCards)
        {
            Destroy(g);
        }

        selectedArmiesCards.Clear();

        UpdateArmyTab();
    }
    public void UpdateArmyTab()
    {
        RectTransform rt = selectedArmiesTab.GetComponent<RectTransform>();

        rt.sizeDelta = new Vector2(selectedArmiesCards.Count * 100f + 10f, 210);
    }

    //REGION TAB
    public void OpenRegionTab()
    {
        regionTab.SetActive(true);
    }
    public void CloseRegionTab()
    {
        regionTab.SetActive(false);
    }

    //BUILDING TAB
    public void Initialize_BuildingTab()
    {
        int n = WorldUtility.GetBuildingsCount();
        for (int i = 0; i < n; i++)
        {
            GameObject buildingGridSlot = Instantiate(buildingGridSlotPrefab);
            buildingGridSlot.transform.parent = buildindGrid.transform;

            BuildingGridSlotManager buildingGridSlotManager = buildingGridSlot.GetComponent<BuildingGridSlotManager>();
            buildingGridSlotManager.building_ID = i;
            buildingGridSlotManager.worldUIManager = this;

            buildingGridSlot.name = "BudildingGridSlot_" + WorldUtility.GetBuildingByID(i).name;

            buildingGridSlotManager.Initialize();
        }
    }
    public void OpenBuildingTab()
    {
        buildingTab.SetActive(true);
    }
    public void CloseBuildingTab()
    {
        buildingTab.SetActive(false);
    }
    public void ShowBuildingInfos(int ID)
    {
        buildingText.GetComponent<Text>().text = WorldUtility.GetBuildingByID(ID).description;
    }
}