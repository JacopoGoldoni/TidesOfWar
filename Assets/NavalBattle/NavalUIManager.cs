using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavalUIManager : MonoBehaviour
{
    public GameObject shipSelectedTab;
    public List<GameObject> selectedShipCards = new List<GameObject>();

    [Header("Prefab")]
    public GameObject shipCardPrefab;

    public void AddCard(NavalManager shipRef)
    {
        shipSelectedTab.SetActive(true);

        GameObject shipCard = Instantiate(shipCardPrefab);

        selectedShipCards.Add(shipCard);
        shipCard.transform.SetParent(shipSelectedTab.transform, false);

        ResizeCardTab();
    }
    public void RemoveCard(NavalManager shipRef)
    {
        ResizeCardTab();

        if (shipCardPrefab.transform.childCount == 0)
        {
            shipSelectedTab.SetActive(false);
        }
    }
    public void ClearAllCards()
    {
        foreach(GameObject go in selectedShipCards)
        {
            Destroy(go);
        }
        
        selectedShipCards.Clear();

        shipSelectedTab.SetActive(false);
    }

    private void ResizeCardTab()
    {
        shipSelectedTab.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(selectedShipCards.Count * 120f + 20f, 200f);
    }
}