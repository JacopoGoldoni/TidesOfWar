using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{

    List<(CaptainManager, BattalionCardManager)> battalionCards = new List<(CaptainManager, BattalionCardManager)>();

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
        battalionCardManager.Initialize(battalion);

        battalionCards.Add((battalion, battalionCardManager));

        //Displacement
        Vector2 cardSize = BattalionCardPrefab.GetComponent<RectTransform>().sizeDelta;
        //battalionCardHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(battalionCards.Count * cardSize.x + 10, cardSize.y + 10);

        BattalionCardHolderCheck();
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
        //battalionCardHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(battalionCards.Count * 100 + 10, 110);

        BattalionCardHolderCheck();
        BattalionCommandTabCheck();
    }
    public void RemoveAllBattalionCard()
    {
        foreach (Transform child in battalionCardHolder.transform)
        {
            Destroy(child.gameObject);
        }

        battalionCards.Clear();

        BattalionCardHolderCheck();
        BattalionCommandTabCheck();
    }
}