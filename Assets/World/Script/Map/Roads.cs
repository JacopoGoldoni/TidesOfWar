using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roads
{
    public int[] regions;
    public int level;

    const float RadiusPerLevel = 1f;

    public float GetRadius()
    {
        return level * RadiusPerLevel;
    }
}
