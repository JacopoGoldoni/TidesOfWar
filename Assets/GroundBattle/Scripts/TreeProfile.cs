using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tree Profile", menuName = "ScriptableObjects/TreeProfile")]
public class TreeProfile : ScriptableObject
{
    public GameObject treePrefab;

    public float scale;

    public float density;

    public float randomScale;
}