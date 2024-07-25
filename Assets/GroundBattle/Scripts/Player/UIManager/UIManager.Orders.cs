using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{
    //FORMATION PRESETS
    public void SendLineFormation()
    {
        if (cameraManagerRef.selectedBattalions.Count != 0)
        {
            foreach (CaptainManager cm in cameraManagerRef.selectedBattalions)
            {
                cm.battalionFormation.AllInLine();
                cm.battalionFormation.CalculateAllPositions();
                cm.SendFormation();
            }
        }
        else
        {
            //COMPANY ORDER
            int n = cameraManagerRef.selectedCompanies.Count;

            for (int i = 0; i < n; i++)
            {
                OfficerManager of = cameraManagerRef.selectedCompanies[i];
                Formation f = new Line(of.companySize);
                of.SetFormation(f);
                of.ReceiveMovementOrder(false, Utility.V3toV2(of.transform.position), of.transform.rotation);
            }
        }
    }
    public void SendColumnFormation()
    {
        if (cameraManagerRef.selectedBattalions.Count != 0)
        {
            //BATTALION ORDER
            int n = cameraManagerRef.selectedBattalions.Count;

            for (int i = 0; i < n; i++)
            {
                CaptainManager cm = cameraManagerRef.selectedBattalions[i];
                //ADD HERE COLUMN LOGIC
                cm.ReceiveMovementOrder(false, Utility.V3toV2(cm.transform.position), cm.transform.rotation);
            }
        }
        else
        {
            //COMPANY ORDER
            int n = cameraManagerRef.selectedCompanies.Count;

            for (int i = 0; i < n; i++)
            {
                OfficerManager of = cameraManagerRef.selectedCompanies[i];
                Formation f = new Column(of.companySize);
                of.SetFormation(f);
                of.ReceiveMovementOrder(false, Utility.V3toV2(of.transform.position), of.transform.rotation);
            }
        }
    }

    //FORMATION MANAGEMENT
    public void SendLightFront()
    {
        foreach (CaptainManager cm in cameraManagerRef.selectedBattalions)
        {
            cm.battalionFormation.MoveToFrontByHardness(UnitHardness.Light);

            cm.battalionFormation.CalculateAllPositions();
            cm.SendFormation();
        }
    }
    public void SendHeavyFront()
    {
        foreach (CaptainManager cm in cameraManagerRef.selectedBattalions)
        {
            cm.battalionFormation.MoveToFrontByHardness(UnitHardness.Heavy);

            cm.battalionFormation.CalculateAllPositions();
            cm.SendFormation();
        }
    }
    public void SendLightRear()
    {
        foreach (CaptainManager cm in cameraManagerRef.selectedBattalions)
        {
            cm.battalionFormation.MoveToRearByHardness(UnitHardness.Light);

            cm.battalionFormation.CalculateAllPositions();
            cm.SendFormation();
        }
    }
    public void SendHeavyRear()
    {
        foreach (CaptainManager cm in cameraManagerRef.selectedBattalions)
        {
            cm.battalionFormation.MoveToRearByHardness(UnitHardness.Heavy);

            cm.battalionFormation.CalculateAllPositions();
            cm.SendFormation();
        }
    }
    public void SendLightLine()
    {
        foreach (CaptainManager cm in cameraManagerRef.selectedBattalions)
        {
            cm.battalionFormation.MoveToLineByHardness(UnitHardness.Light);

            cm.battalionFormation.CalculateAllPositions();
            cm.SendFormation();
        }
    }
    public void SendHeavyLine()
    {
        foreach (CaptainManager cm in cameraManagerRef.selectedBattalions)
        {
            cm.battalionFormation.MoveToLineByHardness(UnitHardness.Heavy);

            cm.battalionFormation.CalculateAllPositions();
            cm.SendFormation();
        }
    }

    //ORDERS
    public void SendStopOrder()
    {
        int n = cameraManagerRef.selectedCompanies.Count;
        int m = cameraManagerRef.selectedBattalions.Count;

        if (m != 0)
        {
            //BATTALION ORDER
            for (int i = 0; i < m; i++)
            {
                CaptainManager cm = cameraManagerRef.selectedBattalions[i];
                cm.um.SetDestination(Utility.V3toV2(cm.transform.position), cm.transform.rotation);
            }
        }
        else
        {
            //COMPANY ORDER
            for (int i = 0; i < n; i++)
            {
                OfficerManager om = cameraManagerRef.selectedCompanies[i];
                om.um.SetDestination(Utility.V3toV2(om.transform.position), om.transform.rotation);
            }
        }
    }
    public void SendFireAll()
    {
        foreach (CaptainManager cm in cameraManagerRef.selectedBattalions)
        {
            foreach (OfficerManager om in cm.companies)
            {
                om.SetFireStatus(true);
            }
        }
    }
    public void SendHoldFire()
    {
        foreach (CaptainManager cm in cameraManagerRef.selectedBattalions)
        {
            foreach (OfficerManager om in cm.companies)
            {
                om.SetFireStatus(false);
            }
        }
    }
    public void SendMelee()
    {

    }
    public void SendMarch()
    {

    }
    public void SendDefend()
    {

    }
    public void SendAttack()
    {

    }

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
        int n = GFXUtility.GetSpriteSheet("CompanyOrders").Length;

        RectTransform rectTransform = CompanyCommandTab.GetComponent<RectTransform>();
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
            commandButton.name = "CompanyCommandButton_" + i;

            CommandButtonManager commandButtonManager = commandButton.GetComponent<CommandButtonManager>();
            commandButtonManager.Initialize(i, false, b, CompanyCommands[i]);

            commandButton.transform.parent = rectTransform;
        }
    }
    private void PopulateBattalionCommandTab()
    {
        int n = BattalionCommands.Count;

        RectTransform rectTransform = BattalionCommandTab.GetComponent<RectTransform>();
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
            commandButton.name = "BattallionCommandButton_" + i;

            CommandButtonManager commandButtonManager = commandButton.GetComponent<CommandButtonManager>();
            commandButtonManager.Initialize(i, true, b, BattalionCommands[i]);

            commandButton.transform.parent = rectTransform;
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
            commandButtonManager.Initialize(i, true, b, ArtilleryBatteryCommands[i]);

            commandButton.transform.parent = rectTransform;
        }
    }
}