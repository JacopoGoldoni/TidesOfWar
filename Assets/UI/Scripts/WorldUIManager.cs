using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUIManager : MonoBehaviour
{
    GameObject UI;

    List<GameObject> selectedArmiesCards = new List<GameObject>();
    List<GameObject> selectedFleetsCards = new List<GameObject>();

    [Header("UI prefab")]
    public GameObject armyCard;
    public GameObject fleetCard;
    public GameObject armySlot;
    public GameObject fleetSlot;

    [Header("UI references")]
    public GameObject regionTab;
    public GameObject buildingTab;
    public GameObject selectedArmiesTab;
    public GameObject selectedFleetsTab;
    public GameObject armiesTab;
    public GameObject fleetsTab;

    private void Awake()
    {
        UI = GameObject.Find("UI");
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

    //FLEET CARD
    public void AddFleetCard(int fleetID)
    {
        GameObject go = Instantiate(armyCard);

        go.transform.parent = selectedFleetsTab.transform;
        go.GetComponent<SelectedFleetCardManager>().FleetID = fleetID;
        //TODO add fleet name and flag
        //go.GetComponent<SelectedFleetCardManager>().PopulateCard("Fleet", null);

        selectedFleetsCards.Add(go);

        UpdateFleetTab();
    }
    public void RemoveFleetCard(int fleetID)
    {
        foreach (GameObject g in selectedFleetsCards)
        {
            SelectedFleetCardManager s = g.GetComponent<SelectedFleetCardManager>();
            if (s.FleetID == fleetID)
            {
                selectedArmiesCards.Remove(g);
                Destroy(g);
                return;
            }
        }

        UpdateFleetTab();
    }
    public void ClearFleetCard()
    {
        foreach (GameObject g in selectedFleetsCards)
        {
            Destroy(g);
        }

        selectedFleetsCards.Clear();

        UpdateFleetTab();
    }
    public void UpdateFleetTab()
    {
        RectTransform rt = selectedFleetsTab.GetComponent<RectTransform>();

        rt.sizeDelta = new Vector2(selectedFleetsCards.Count * 100f + 10f, 210);
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
    public void OpenBuildingTab()
    {
        buildingTab.SetActive(true);
    }
    public void CloseBuildingTab()
    {
        buildingTab.SetActive(false);
    }

    //ARMIES TAB
    public void UpdateArmiesTab()
    {
        Army[] myArmies = WorldUtility.GetArmiesByTAG("FRA");

        foreach(Army army in myArmies)
        {
            GameObject armySlotInstance = Instantiate(armySlot);

            armySlotInstance.transform.parent = armiesTab.transform;
        }
    }

    //FLEETS TAB
    public void UpdateFleetsTab()
    {
        Fleet[] myFleets = WorldUtility.GetFleetsByTAG("FRA");
        
        foreach (Fleet fleet in myFleets)
        {
            GameObject fleetSlotInstance = Instantiate(fleetSlot);

            fleetSlotInstance.transform.parent = fleetsTab.transform;
        }
    }
}