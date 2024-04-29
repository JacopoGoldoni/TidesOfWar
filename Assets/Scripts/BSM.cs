using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BATTLE STRATEGY MIND
public class BSM : MonoBehaviour
{
    Factions faction = Factions.Austria;

    //Decision weights
    Dictionary<string, float> Weights = new Dictionary<string, float>();

    List<OfficerManager> regiments = new List<OfficerManager>();

    // Start is called before the first frame update
    void Start()
    {
        InitializeWeights();
        GetAllRegiments();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitializeWeights()
    {
        Weights.Add("offensive_base", 50);
        Weights.Add("defensive_base", 50);
    }

    private float GetAttitude(string attitude)
    {
        float o = 0f;

        foreach (KeyValuePair<string, float> w in Weights)
        {
            if (w.Key.Contains(attitude))
            {
                o += w.Value;
            }
        }

        return o;
    }

    private void GetAllRegiments()
    {
        OfficerManager[] om = FindObjectsOfType<OfficerManager>();

        for(int i = 0; i < om.Length; i++)
        {
            if (om[i].faction == faction)
            {
                regiments.Add(om[i]);
            }
        }
    }
}