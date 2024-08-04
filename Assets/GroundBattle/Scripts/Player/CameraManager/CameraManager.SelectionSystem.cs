using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        DeselectAllArtilleryBatteries();

        uimanager.CompanyCommandTabCheck();
        uimanager.HighlightCompanyCard(target.companyNumber, true);
        uimanager.UpdateCompanyCommandStatus();

        target.OnSelection();
    }
    public void DeselectCompany(OfficerManager target)
    {
        selectedCompanies.Remove(target);

        uimanager.CompanyCommandTabCheck();
        uimanager.HighlightCompanyCard(target.companyNumber, false);
        uimanager.UpdateCompanyCommandStatus();

        target.OnDeselection();
    }
    public void DeselectAllCompanies()
    {
        //Remove highlight to all units
        if (selectedCompanies.Count != 0)
        {
            foreach (OfficerManager t in selectedCompanies)
            {
                uimanager.HighlightCompanyCard(t.companyNumber, false);
                t.OnDeselection();
            }
        }

        selectedCompanies.Clear();
        uimanager.CompanyCommandTabCheck();
    }

    public void SelectBattalion(CaptainManager target)
    {
        selectedBattalions.Add(target);

        DeselectAllCompanies();
        DeselectAllArtilleryBatteries();

        uimanager.BattalionCommandTabCheck();
        uimanager.HighlightBattalionCard(target.battalionNumber, true);

        target.OnSelection();
    }
    public void DeselectBattalion(CaptainManager target)
    {
        selectedBattalions.Remove(target);

        uimanager.BattalionCommandTabCheck();
        uimanager.HighlightBattalionCard(target.battalionNumber, false);

        target.OnDeselection();
    }
    public void DeselectAllBattalions()
    {
        //Remove highlight to all units
        if (selectedBattalions.Count != 0)
        {
            foreach (CaptainManager t in selectedBattalions)
            {
                uimanager.HighlightBattalionCard(t.battalionNumber, false);
                t.OnDeselection();
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
        target.OnSelection();
    }
    public void DeselectArtilleryBattery(ArtilleryOfficerManager target)
    {
        selectedArtilleryBatteries.Remove(target);

        uimanager.ArtilleryBatteryCommandTabCheck();
        uimanager.HighlightArtilleryBatteryCard(target.batteryNumber, false);
        target.OnDeselection();
    }
    public void DeselectAllArtilleryBatteries()
    {
        //Remove highlight to all units
        if (selectedArtilleryBatteries.Count != 0)
        {
            foreach (ArtilleryOfficerManager t in selectedArtilleryBatteries)
            {
                uimanager.HighlightArtilleryBatteryCard(t.batteryNumber, false);
                t.OnDeselection();
            }
        }

        selectedArtilleryBatteries.Clear();
        uimanager.ArtilleryBatteryCommandTabCheck();
    }
}