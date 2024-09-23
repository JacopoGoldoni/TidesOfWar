using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Structure_Battalion
{
    public int ID;
    public string name;
    public string TAG;

    public string template_name;
    public int[] companies;
}