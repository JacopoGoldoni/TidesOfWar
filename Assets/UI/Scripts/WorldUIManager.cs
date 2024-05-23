using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUIManager : MonoBehaviour
{
    GameObject UI;
    public GameObject selectedArmiesTab;
    List<GameObject> selectedArmiesCards = new List<GameObject>();

    //UI PREFAB
    public GameObject armyCard;

    private void Awake()
    {
        UI = GameObject.Find("UI");
    }

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
}