using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "ScriptableObjects/Resource")]
public class Resource : ScriptableObject
{
    public int ID;
    public string resourceName;
    public string resourceIcon;
}