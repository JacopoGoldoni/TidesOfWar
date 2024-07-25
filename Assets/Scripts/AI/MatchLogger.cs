using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchLogger : MonoBehaviour
{
    public bool registrate = false;

    List<OfficerManager> trackedRegiments = new List<OfficerManager>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(registrate)
        {
            foreach(OfficerManager om in GroundBattleUtility.GetAllCompanies())
            {
                if(trackedRegiments.Contains(om))
                {
                    //ALREADY TRACKED
                    if (om.targetCompany == null)
                    {
                        trackedRegiments.Remove(om);
                    }
                    else
                    {
                        //TRACKING CYCLE
                    }
                }
                else
                {
                    //NOT ALREADY TRACKED
                    if (om.targetCompany != null)
                    {
                        trackedRegiments.Add(om);
                    }
                }
            }
        }
    }
}
