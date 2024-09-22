using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voronoi
{
    Vector2[] points;
    Color[] colors;

    public Voronoi(Vector2[] points)
    {
        this.points = points;
    }
    public Voronoi(Vector2[] points, Color[] colors)
    {
        this.points = points;
        this.colors = colors;
    }

    private int RegionAt(Vector2 v2)
    {
        float d = Mathf.Infinity;
        int index = 0;
        for(int i = 0; i < points.Length; i++)
        {
            float _d = (v2 - points[i]).magnitude;
            if(d < _d)
            {
                d = _d;
                index = i;
            }
        }

        return index;
    }
}