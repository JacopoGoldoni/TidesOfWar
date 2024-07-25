using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CameraManager : MonoBehaviour
{
    public List<OfficerManager> selectedCompanies = new List<OfficerManager>();
    public List<CaptainManager> selectedBattalions = new List<CaptainManager>();
    public List<ArtilleryOfficerManager> selectedArtilleryBatteries = new List<ArtilleryOfficerManager>();

    public void SelectCompany(OfficerManager target)
    {
        selectedCompanies.Add(target);

        DeselectAllBattalions();

        uimanager.CompanyCommandTabCheck();
        uimanager.HighlightCompanyCard(target.companyNumber, true);
    }
    public void DeselectCompany(OfficerManager target)
    {
        selectedCompanies.Remove(target);

        uimanager.CompanyCommandTabCheck();
        uimanager.HighlightCompanyCard(target.companyNumber, false);
    }
    public void DeselectAllCompanies()
    {
        //Remove highlight to all units
        if (selectedCompanies.Count != 0)
        {
            foreach (OfficerManager t in selectedCompanies)
            {
                uimanager.HighlightCompanyCard(t.companyNumber, false);
            }
        }

        selectedCompanies.Clear();
        uimanager.CompanyCommandTabCheck();
    }

    public void SelectBattalion(CaptainManager target)
    {
        selectedBattalions.Add(target);

        DeselectAllCompanies();

        uimanager.BattalionCommandTabCheck();
        uimanager.HighlightBattalionCard(target.battalionNumber, true);
    }
    public void DeselectBattalion(CaptainManager target)
    {
        selectedBattalions.Remove(target);

        uimanager.BattalionCommandTabCheck();
        uimanager.HighlightBattalionCard(target.battalionNumber, false);
    }
    public void DeselectAllBattalions()
    {
        //Remove highlight to all units
        if (selectedBattalions.Count != 0)
        {
            foreach (CaptainManager t in selectedBattalions)
            {
                uimanager.HighlightBattalionCard(t.battalionNumber, false);
            }
        }

        selectedBattalions.Clear();
        uimanager.BattalionCommandTabCheck();
    }

    public void SelectArtilleryBattery(ArtilleryOfficerManager target)
    {
        selectedArtilleryBatteries.Add(target);

        DeselectAllCompanies();
        DeselectAllBattalions();

        uimanager.ArtilleryBatteryCommandTabCheck();
        uimanager.HighlightArtilleryBatteryCard(target.batteryNumber, true);
    }
    public void DeselectArtilleryBattery(ArtilleryOfficerManager target)
    {
        selectedArtilleryBatteries.Remove(target);

        uimanager.ArtilleryBatteryCommandTabCheck();
        uimanager.HighlightArtilleryBatteryCard(target.batteryNumber, false);
    }
    public void DeselectAllArtilleryBatteries()
    {
        //Remove highlight to all units
        if (selectedBattalions.Count != 0)
        {
            foreach (CaptainManager t in selectedBattalions)
            {
                uimanager.HighlightArtilleryBatteryCard(t.battalionNumber, false);
            }
        }

        selectedArtilleryBatteries.Clear();
        uimanager.ArtilleryBatteryCommandTabCheck();
    }
}