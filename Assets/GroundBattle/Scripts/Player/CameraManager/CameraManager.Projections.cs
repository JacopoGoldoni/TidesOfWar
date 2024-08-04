using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CameraManager : MonoBehaviour
{

    private void ProjectAll()
    {
        if (selectedCompanies.Count != 0)
        {
            foreach (OfficerManager o in selectedCompanies)
            {
                //DRAW PATH LINES
                o.drawPathLine = true;
            }
        }
        else if(selectedBattalions.Count != 0)
        {
            foreach (CaptainManager cm in selectedBattalions)
            {
                foreach (OfficerManager om in cm.companies)
                {
                    //DRAW PATH LINES
                    om.drawPathLine = true;
                }
            }
        }
    }
    private void DeleteAllProjections()
    {
        foreach (OfficerManager o in GroundBattleUtility.GetAllCompanies())
        {
            o.drawPathLine = false;
        }
    }
}