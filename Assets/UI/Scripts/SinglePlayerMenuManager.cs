using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerMenuManager : MonoBehaviour
{
    public GameObject countrySelectionPanel;
    public GameObject gameSettingsPanel;

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
}