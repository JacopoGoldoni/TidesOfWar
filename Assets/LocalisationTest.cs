using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalisationTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log(LocalisationUtility.GetValue("FRA"));
    }
}
