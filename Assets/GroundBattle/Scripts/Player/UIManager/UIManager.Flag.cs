using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{
    List<(GameObject, GameObject)> companyFlags = new List<(GameObject, GameObject)>();                 //(target, flag)
    List<(GameObject, GameObject)> battalionFlags = new List<(GameObject, GameObject)>();               //(target, flag)
    List<(GameObject, GameObject)> artilleryBattalionFlags = new List<(GameObject, GameObject)>();      //(target, flag)
    
    [Header("Flag variables")]
    public float baseFlagScale = 0.5f;
    public float transitionDistanceFlag = 20f;
    public float transitionStrenghtFlag = 1f;
    public float flagCompanyTransparency = 0.5f;
    public float flagBattalionTransparency = 0.5f;

    [Header("Flag prefabs")]
    public GameObject CompanyFlag_Prefab;
    public GameObject BattalionFlag_Prefab;
    public GameObject ArtillleryBatteryFlag_Prefab;

    private GameObject CompanyFlag_Builder(OfficerManager target)
    {
        GameObject CompanyFlag = Instantiate(CompanyFlag_Prefab);
        CompanyFlag.name = "RegimentFlag_" + target.companyNumber;
        CompanyFlag.layer = 5;

        CompanyFlagManager cfm = CompanyFlag.GetComponent<CompanyFlagManager>();
        cfm.Initialize(target);

        RectTransform rectTransform1 = CompanyFlag.GetComponent<RectTransform>();
        rectTransform1.parent = companyFlagParent.transform;

        CompanyFlag.transform.position = Utility.Camera.WorldToScreenPoint(target.transform.position + Vector3.up * 1f);

        return CompanyFlag;
    }
    private GameObject BattalionFlag_Builder(CaptainManager target)
    {
        GameObject BattalionFlag = Instantiate(BattalionFlag_Prefab);
        BattalionFlag.name = "BattalionFlag_" + target.battalionNumber;
        BattalionFlag.layer = 5;

        //CHANGE TO BATTALION FLAG MANAGER
        BattalionFlagManager cfm = BattalionFlag.GetComponent<BattalionFlagManager>();
        cfm.Initialize(target);

        RectTransform rectTransform1 = BattalionFlag.GetComponent<RectTransform>();
        rectTransform1.parent = battalionFlagParent.transform;

        BattalionFlag.transform.position = Utility.Camera.WorldToScreenPoint(target.transform.position + Vector3.up * 1f);

        return BattalionFlag;
    }
    private GameObject ArtilleryBatteryFlag_Builder(ArtilleryOfficerManager target)
    {
        GameObject ArtilleryBatteryFlag = Instantiate(ArtillleryBatteryFlag_Prefab);
        ArtilleryBatteryFlag.name = "ArtilleryBatteryFlag_" + target.batteryNumber;
        ArtilleryBatteryFlag.layer = 5;

        ArtilleryBatteryFlagManager cfm = ArtilleryBatteryFlag.GetComponent<ArtilleryBatteryFlagManager>();
        cfm.Initialize(target);

        RectTransform rectTransform1 = ArtilleryBatteryFlag.GetComponent<RectTransform>();
        rectTransform1.parent = artilleryBatteryFlagParent.transform;

        ArtilleryBatteryFlag.transform.position = Utility.Camera.WorldToScreenPoint(target.transform.position + Vector3.up * 1f);

        return ArtilleryBatteryFlag;
    }
    
    public void AppendCompanyFlag(OfficerManager of)
    {
        (GameObject, GameObject) tf;
        tf.Item1 = of.gameObject;
        tf.Item2 = CompanyFlag_Builder(of);
        companyFlags.Add(tf);
    }
    public void AppendBattalionFlag(CaptainManager ca)
    {
        (GameObject, GameObject) tf;
        tf.Item1 = ca.gameObject;
        tf.Item2 = BattalionFlag_Builder(ca);
        battalionFlags.Add(tf);
    }
    public void AppendArtilleryBatteryFlag(ArtilleryOfficerManager aom)
    {
        (GameObject, GameObject) tf;
        tf.Item1 = aom.gameObject;
        tf.Item2 = ArtilleryBatteryFlag_Builder(aom);
        artilleryBattalionFlags.Add(tf);
    }

    private void UpdateFlagPosition()
    {
        //UPDATE FLAG COMPANY POSITIONS
        foreach ((GameObject, GameObject) o in companyFlags)
        {
            if (Utility.IsInView(o.Item1))
            {
                o.Item2.SetActive(true);

                float scale = 1f;

                float d = (o.Item1.transform.position - transform.position).magnitude;

                if (d < transitionDistanceFlag * 2f)
                {
                    scale =
                        baseFlagScale +
                        0.5f * UtilityMath.Sigmoid(
                            transitionStrenghtFlag * (o.Item1.transform.position - transform.position).magnitude - transitionDistanceFlag
                            );
                }

                o.Item2.GetComponent<RectTransform>().position = Utility.Camera.WorldToScreenPoint(o.Item1.transform.position + Vector3.up * 1f);

                RectTransform r = o.Item2.GetComponent<RectTransform>();

                r.localScale = scale * Vector3.one;
            }
            else
            {
                o.Item2.SetActive(false);
            }
        }

        //UPDATE FLAG BATTALION POSITIONS
        foreach ((GameObject, GameObject) o in battalionFlags)
        {
            if (Utility.IsInView(o.Item1))
            {
                o.Item2.SetActive(true);

                float scale = 1f;

                float d = (o.Item1.transform.position - transform.position).magnitude;

                if (d < transitionDistanceFlag * 2f)
                {
                    scale =
                        baseFlagScale +
                        0.5f * UtilityMath.Sigmoid(
                            transitionStrenghtFlag * (o.Item1.transform.position - transform.position).magnitude - transitionDistanceFlag
                            );
                }

                o.Item2.GetComponent<RectTransform>().position = Utility.Camera.WorldToScreenPoint(o.Item1.transform.position + Vector3.up * 1f);

                RectTransform r = o.Item2.GetComponent<RectTransform>();

                r.localScale = scale * Vector3.one;
            }
            else
            {
                o.Item2.SetActive(false);
            }
        }

        //UPDATE FLAG BATTALION POSITIONS
        foreach ((GameObject, GameObject) o in artilleryBattalionFlags)
        {
            if (Utility.IsInView(o.Item1))
            {
                o.Item2.SetActive(true);

                float scale = 1f;

                float d = (o.Item1.transform.position - transform.position).magnitude;

                if (d < transitionDistanceFlag * 2f)
                {
                    scale =
                        baseFlagScale +
                        0.5f * UtilityMath.Sigmoid(
                            transitionStrenghtFlag * (o.Item1.transform.position - transform.position).magnitude - transitionDistanceFlag
                            );
                }

                o.Item2.GetComponent<RectTransform>().position = Utility.Camera.WorldToScreenPoint(o.Item1.transform.position + Vector3.up * 1f);

                RectTransform r = o.Item2.GetComponent<RectTransform>();

                r.localScale = scale * Vector3.one;
            }
            else
            {
                o.Item2.SetActive(false);
            }
        }
    }
}