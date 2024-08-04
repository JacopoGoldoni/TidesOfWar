using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{
    //ARTILLERY BATTERY CARD HOLDER

    List<(ArtilleryOfficerManager, ArtilleryBatteryCardManager)> artilleryBatteryCards = new List<(ArtilleryOfficerManager, ArtilleryBatteryCardManager)>();

    private void ArtilleryBatteryCardHolderCheck()
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

    public void AddArtilleryBatteryCard(ArtilleryOfficerManager battery)
    {
        GameObject ArtilleryBatteryCard = Instantiate(ArtilleryBatteryCardPrefab);
        ArtilleryBatteryCard.name = "ArtilleryBatteryCard_" + battery.batteryNumber.ToString();
        ArtilleryBatteryCard.layer = 5;

        ArtilleryBatteryCard.transform.parent = artilleryBatteryCardHolder.transform;

        ArtilleryBatteryCardManager artilleryBatteryCardManager = ArtilleryBatteryCard.GetComponent<ArtilleryBatteryCardManager>();
        artilleryBatteryCardManager.Initialize(battery);

        artilleryBatteryCards.Add((battery, artilleryBatteryCardManager));

        //Displacement
        Vector2 cardSize = ArtilleryBatteryCardPrefab.GetComponent<RectTransform>().sizeDelta;
        //artilleryBatteryCardHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(artilleryBatteryCards.Count * cardSize.x + 10, cardSize.y + 10);

        ArtilleryBatteryCardHolderCheck();
    }
    public void RemoveArtilleryBatteryCard(int batteryN)
    {
        foreach (Transform child in artilleryBatteryCardHolder.transform)
        {
            if (child.name == "ArtilleryBatteryCard_" + batteryN)
            {
                artilleryBatteryCards.Remove(artilleryBatteryCards.Find(a => a.Item2 == child));
                Destroy(child.gameObject);
                break;
            }
        }

        //Displacement
        artilleryBatteryCardHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(artilleryBatteryCards.Count * 100 + 10, 110);

        ArtilleryBatteryCardHolderCheck();
        ArtilleryBatteryCommandTabCheck();
    }
    public void RemoveAllArtilleryBatteryCard()
    {
        foreach (Transform child in companyCardHolder.transform)
        {
            Destroy(child.gameObject);
        }

        companyCards.Clear();

        ArtilleryBatteryCardHolderCheck();
        ArtilleryBatteryCommandTabCheck();
    }

    public void HighlightArtilleryBatteryCard(int BatteryID, bool highlight)
    {
        for (int i = 0; i < artilleryBatteryCards.Count; i++)
        {
            if (artilleryBatteryCards[i].Item1.batteryNumber == BatteryID)
            {
                artilleryBatteryCards[i].Item2.HighLight(highlight);
                return;
            }
        }
    }
}