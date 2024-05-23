using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerMenuManager : MonoBehaviour
{
    public GameObject countrySelectionPanel;
    public GameObject gameSettingsPanel;
    public GameObject countryListPanel;

    public GameObject countryCardPrefab;

    string[] countriesTAG = { "FRA", "ENG", "AUS", "PRU", "RUS" };

    private void Start()
    {
        PopulateCountrySelection();
    }

    public void OpenCountrySelection()
    {
        countrySelectionPanel.SetActive(true);
        gameSettingsPanel.SetActive(false);
    }
    public void OpenGameSettings()
    {
        countrySelectionPanel.SetActive(false);
        gameSettingsPanel.SetActive(true);
    }
    private void PopulateCountrySelection()
    {
        foreach(string TAG in countriesTAG)
        {
            GameObject countryCard = Instantiate(countryCardPrefab);

            countryCard.transform.parent = countryListPanel.transform;

            countryCard.GetComponent<CountryCardManager>().Initialize(TAG);
        }
    }
}