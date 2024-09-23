using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{
    //COMPANY CARD HOLDER

    List<(OfficerManager, CompanyCardManager)> companyCards = new List<(OfficerManager, CompanyCardManager)>();

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
        companyCardManager.Initialize(company);

        companyCards.Add((company, companyCardManager));

        //Displacement
        Vector2 cardSize = CompanyCardPrefab.GetComponent<RectTransform>().sizeDelta;
        //companyCardHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(companyCards.Count * cardSize.x + 10, cardSize.y + 10);

        CompanyCardHolderCheck();
    }
    public void RemoveCompanyCard(int regimentN)
    {
        foreach (Transform child in companyCardHolder.transform)
        {
            if (child.name == "RegimentCard_" + regimentN)
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
        for (int i = 0; i < companyCards.Count; i++)
        {
            if (companyCards[i].Item1.companyNumber == companyID)
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
}