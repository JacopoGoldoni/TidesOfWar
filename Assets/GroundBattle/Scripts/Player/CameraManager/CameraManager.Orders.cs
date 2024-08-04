using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CameraManager : MonoBehaviour
{
    public void SendMovementOrder()
    {
        Vector2 Orientation = Utility.V3toV2(OrderPoint2 - OrderPoint).normalized;

        if (selectedCompanies.Count != 0)
        {
            //COMPANY

            float comapnyFormationWidth = 0f;
            //CALCULATE SELECTED FORMATION WIDTH
            for (int i = 0; i < selectedCompanies.Count; i++)
            {
                comapnyFormationWidth += companySpace + selectedCompanies[i].GetCompanyWidth();
            }

            //COMPANY MOVEMENT ORDER
            for (int i = 0; i < selectedCompanies.Count; i++)
            {
                OfficerManager unit = selectedCompanies[i];

                float localX = 0f;
                for (int j = 0; j <= i; j++)
                {
                    if (j == i)
                    {
                        localX += companySpace + selectedCompanies[j].GetCompanyWidth() / 2f;
                    }
                    else
                    {
                        localX += companySpace + selectedCompanies[j].GetCompanyWidth();
                    }
                }

                Vector2 relativePos = UtilityMath.RotateVector2(Orientation) * (localX - comapnyFormationWidth / 2f);

                Vector2 pos = Utility.V3toV2(OrderPoint) + relativePos;


                if (unit.IsObstructedAt(pos))
                {
                    continue;
                }

                Quaternion rot = Quaternion.LookRotation(Utility.V2toV3(Orientation), Vector3.up);

                //SEND ORDER
                unit.ReceiveMovementOrder(Input.GetKey(KeyCode.LeftShift), pos, rot);

                //UPDATE PROJECTIONS
                //DeleteAllProjections();
                //ProjectAll();
            }

            ShowOrderArrow = false;
        }
        else if (selectedBattalions.Count != 0)
        {
            //BATTALION

            float[] battalionPositions = new float[selectedBattalions.Count]; ;
            float battalionFormationWidth = 0f;

            battalionFormationWidth += selectedBattalions[0].battalionFormation.GetWidth() / 2f;
            battalionPositions[0] = battalionFormationWidth;
            battalionFormationWidth += selectedBattalions[0].battalionFormation.GetWidth() / 2f;

            //CALCULATE SELECTED FORMATION WIDTH
            for (int i = 1; i < selectedBattalions.Count; i++)
            {
                battalionFormationWidth += battalionSpace;
                battalionFormationWidth += selectedBattalions[i].battalionFormation.GetWidth() / 2f;
                battalionPositions[i] = battalionFormationWidth;
                battalionFormationWidth += selectedBattalions[i].battalionFormation.GetWidth() / 2f;
            }

            //BATTALION MOVEMENT ORDER
            for (int i = 0; i < selectedBattalions.Count; i++)
            {
                CaptainManager unit = selectedBattalions[i];

                Vector2 relativePos = UtilityMath.RotateVector2(Orientation) * (battalionPositions[i] - battalionFormationWidth / 2f);

                Vector2 pos = Utility.V3toV2(OrderPoint) + relativePos;

                Quaternion rot = Quaternion.LookRotation(Utility.V2toV3(Orientation), Vector3.up);

                //SEND ORDER
                unit.ReceiveMovementOrder(Input.GetKey(KeyCode.LeftShift), pos, rot);
            }
            ShowOrderArrow = false;
        }
        else if (selectedArtilleryBatteries.Count != 0)
        {
            //ARTILLERY BATTERY

            float[] artilleryBatteryPositions = new float[selectedArtilleryBatteries.Count]; ;
            float artilleryBatteryFormationWidth = 0f;

            artilleryBatteryFormationWidth += selectedArtilleryBatteries[0].GetWidth() / 2f;
            artilleryBatteryPositions[0] = artilleryBatteryFormationWidth;
            artilleryBatteryFormationWidth += selectedArtilleryBatteries[0].GetWidth() / 2f;

            //CALCULATE SELECTED FORMATION WIDTH
            for (int i = 1; i < selectedArtilleryBatteries.Count; i++)
            {
                artilleryBatteryFormationWidth += battalionSpace;
                artilleryBatteryFormationWidth += selectedArtilleryBatteries[i].GetWidth() / 2f;
                artilleryBatteryPositions[i] = artilleryBatteryFormationWidth;
                artilleryBatteryFormationWidth += selectedArtilleryBatteries[i].GetWidth() / 2f;
            }

            //BATTALION MOVEMENT ORDER
            for (int i = 0; i < selectedArtilleryBatteries.Count; i++)
            {
                ArtilleryOfficerManager unit = selectedArtilleryBatteries[i];

                Vector2 relativePos = UtilityMath.RotateVector2(Orientation) * (artilleryBatteryPositions[i] - artilleryBatteryFormationWidth / 2f);

                Vector2 pos = Utility.V3toV2(OrderPoint) + relativePos;

                Quaternion rot = Quaternion.LookRotation(Utility.V2toV3(Orientation), Vector3.up);

                //SEND ORDER
                unit.ReceiveMovementOrder(Input.GetKey(KeyCode.LeftShift), pos, rot);
            }
            ShowOrderArrow = false;
        }
    }
    public void SendAttackOrder(GameObject target)
    {
        if(selectedCompanies.Count != 0)
        {
            //COMPANY ATTACK ORDER
            foreach (OfficerManager o in selectedCompanies)
            {
                Vector2 pos;
                if ((o.transform.position - target.transform.position).magnitude >= o.Range * 0.75f)
                {
                    pos = Utility.V3toV2(target.transform.position + (o.transform.position - target.transform.position).normalized * o.Range * 0.75f);
                }
                else
                {
                    pos = o.transform.position;
                }
                Quaternion rot = Quaternion.LookRotation((target.transform.position - o.transform.position).normalized, Vector3.up);

                o.ReceiveMovementOrder(false, pos, rot);
            }
        }
        else if(selectedArtilleryBatteries.Count != 0)
        {
            foreach (ArtilleryOfficerManager o in selectedArtilleryBatteries)
            {
                Vector2 pos;
                if ((o.transform.position - target.transform.position).magnitude >= o.Range * 0.75f)
                {
                    pos = Utility.V3toV2(target.transform.position + (o.transform.position - target.transform.position).normalized * o.Range * 0.75f);
                }
                else
                {
                    pos = o.transform.position;
                }
                Quaternion rot = Quaternion.LookRotation((target.transform.position - o.transform.position).normalized, Vector3.up);

                o.ReceiveMovementOrder(false, pos, rot);
            }
        }
    }
    public void AttachToBattalion(CaptainManager battalion)
    {
        foreach(OfficerManager om in selectedCompanies)
        {
            om.Attach(battalion);
        }
    }

}