using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{
    //COMMAND TAB CHECKS
    public void CompanyCommandTabCheck()
    {
        if (cameraManagerRef.selectedCompanies.Count > 0)
        {
            CompanyCommandTab.SetActive(true);
        }
        else
        {
            CompanyCommandTab.SetActive(false);
        }
    }
    public void BattalionCommandTabCheck()
    {
        if (cameraManagerRef.selectedBattalions.Count > 0)
        {
            BattalionCommandTab.SetActive(true);
        }
        else
        {
            BattalionCommandTab.SetActive(false);
        }
    }
    public void ArtilleryBatteryCommandTabCheck()
    {
        if (cameraManagerRef.selectedArtilleryBatteries.Count > 0)
        {
            ArtilleryBatteryCommandTab.SetActive(true);
        }
        else
        {
            ArtilleryBatteryCommandTab.SetActive(false);
        }
    }

    //COMMAND TAB POPULATE
    private void PopulateCompanyCommandTab()
    {
        int n = CompanyCommands.Count;

        Vector2 scale = CommandButtonPrefab.GetComponent<RectTransform>().sizeDelta;

        RectTransform rectTransform = CompanyCommandTab.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(n * scale.x, scale.y);

        for (int i = 0; i < n; i++)
        {
            int b;
            if (i > 0 && i < n - 1)
            {
                b = 1; //CENTER BACKGROUND
            }
            else
            {
                if (i == 0)
                {
                    b = 0; //LEFT CORNER BACKGROUND
                }
                else
                {
                    b = 2; //RIGHT CORNER BACKGROUND
                }
            }

            GameObject commandButton = Instantiate(CommandButtonPrefab);
            commandButton.name = "CompanyCommandButton_" + i;

            CommandButtonManager commandButtonManager = commandButton.GetComponent<CommandButtonManager>();
            commandButtonManager.Initialize(i, b, CompanyCommands[i]);

            commandButton.transform.parent = rectTransform;

            companyCommandButtons.Add(commandButtonManager);
        }
    }
    private void PopulateBattalionCommandTab()
    {
        int n = BattalionCommands.Count;

        Vector2 scale = CommandButtonPrefab.GetComponent<RectTransform>().sizeDelta;

        RectTransform rectTransform = BattalionCommandTab.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(n * scale.x, scale.y);

        for (int i = 0; i < n; i++)
        {
            int b;
            if (i > 0 && i < n - 1)
            {
                b = 1; //CENTER BACKGROUND
            }
            else
            {
                if (i == 0)
                {
                    b = 0; //LEFT CORNER BACKGROUND
                }
                else
                {
                    b = 2; //RIGHT CORNER BACKGROUND
                }
            }

            GameObject commandButton = Instantiate(CommandButtonPrefab);
            commandButton.name = "BattallionCommandButton_" + i;

            CommandButtonManager commandButtonManager = commandButton.GetComponent<CommandButtonManager>();
            commandButtonManager.Initialize(i, b, BattalionCommands[i]);

            commandButton.transform.parent = rectTransform;

            battalionCommandButtons.Add(commandButtonManager);
        }
    }
    private void PopulateArtilleryBatteryCommandTab()
    {
        int n = ArtilleryBatteryCommands.Count;

        RectTransform rectTransform = ArtilleryBatteryCommandTab.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(n * 100, 100);

        for (int i = 0; i < n; i++)
        {
            int b;
            if (i > 0 && i < n - 1)
            {
                b = 1; //CENTER BACKGROUND
            }
            else
            {
                if (i == 0)
                {
                    b = 0; //LEFT CORNER BACKGROUND
                }
                else
                {
                    b = 2; //RIGHT CORNER BACKGROUND
                }
            }

            GameObject commandButton = Instantiate(CommandButtonPrefab);
            commandButton.name = "ArtilleryBatteryCommandButton_" + i;

            CommandButtonManager commandButtonManager = commandButton.GetComponent<CommandButtonManager>();
            commandButtonManager.Initialize(i, b, ArtilleryBatteryCommands[i]);

            commandButton.transform.parent = rectTransform;

            artilleryBatteryCommandButtons.Add(commandButtonManager);
        }
    }

    public void UpdateCompanyCommandStatus()
    {
        if (cameraManagerRef.selectedCompanies.Any(c => !c.IsDetached()))
        {
            int n = companyCommandButtons.Count;
            for (int i = 0; i < n; i++)
            {
                if (i != 7)
                {
                    companyCommandButtons[i].SetActive(false);
                }
                else
                {
                    companyCommandButtons[i].SetActive(true);
                }
            }
        }
        else
        {
            int n = companyCommandButtons.Count;
            for (int i = 0; i < n; i++)
            {
                companyCommandButtons[i].SetActive(true);
            }
        }
    }
}