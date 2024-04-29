using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    public Vector2[] vertices = new Vector2[3];

    //CENTROID
    private bool centroidCalc = false;
    private Vector2 centroid;
    //INCENTER
    private bool incenterCalc = false;
    private Vector2 incenter;
    private float incenterRadius;
    //CIRCUMCENTER
    private bool circumcenterCalc = false;
    private Vector2 circumcenter;
    private float circumRadius;

    //SPECIAL POINTS
    public Vector2 Centroid()
    {
        if(centroidCalc)
        {
            return centroid;
        }
        else
        {
            Vector2 c = new Vector2();

            c.x = (vertices[0].x + vertices[1].x + vertices[2].x) / 3f;
            c.y = (vertices[0].y + vertices[1].y + vertices[2].y) / 3f;

            centroidCalc = true;
            centroid = c;

            return c;
        }
    }
    public Vector2 Incenter()
    {
        if (incenterCalc)
        {
            return incenter;
        }
        else
        {
            Vector2 i = new Vector2();

            float a, b, c;
            a = (vertices[1] - vertices[2]).magnitude;
            b = (vertices[0] - vertices[2]).magnitude;
            c = (vertices[0] - vertices[1]).magnitude;

            i.x = a * vertices[0].x + b * vertices[1].x + c * vertices[2].x;
            i.y = a * vertices[0].y + b * vertices[1].y + c * vertices[2].y;

            i = i / (a + b + c);

            incenterCalc = true;
            incenter = i;

            return i;
        }
    }
    public float IncircleRadius()
    {
        if (incenterCalc)
        {
            return incenterRadius;
        }
        else
        {
            Vector2 i = new Vector2();

            float a, b, c;
            a = (vertices[1] - vertices[2]).magnitude;
            b = (vertices[0] - vertices[2]).magnitude;
            c = (vertices[0] - vertices[1]).magnitude;

            i.x = a * vertices[0].x + b * vertices[1].x + c * vertices[2].x;
            i.y = a * vertices[0].y + b * vertices[1].y + c * vertices[2].y;

            i = i / (a + b + c);

            float s = 0.5f * (a + b + c);
            float r = Mathf.Sqrt((s - a) * (s - b) * (s - c)/s);

            incenterCalc = true;
            incenter = i;
            incenterRadius = r;

            return r;
        }
    }
    public Vector2 Circumcenter()
    {
        if (circumcenterCalc)
        {
            return circumcenter;
        }
        else
        {
            Vector2 u = new Vector2();

            Vector2 BA = vertices[1] - vertices[0];
            Vector2 CA = vertices[2] - vertices[0];

            float D = 2 *(BA.x * CA.y - BA.y * CA.x);

            Vector2 U = new Vector2();
            U.x = (CA.y * BA.sqrMagnitude - BA.y * CA.sqrMagnitude) / D;
            U.y = (BA.x * CA.sqrMagnitude - CA.x * BA.sqrMagnitude) / D;

            u = U + vertices[0];

            circumcenterCalc = true;
            circumcenter = u;
            circumRadius = U.magnitude;

            return u;
        }
    }
    public float CircumRadius()
    {
        if (circumcenterCalc)
        {
            return circumRadius;
        }
        else
        {
            Vector2 u = new Vector2();

            Vector2 BA = vertices[1] - vertices[0];
            Vector2 CA = vertices[2] - vertices[0];

            float D = 2 * (BA.x * CA.y - BA.y * CA.x);

            Vector2 U = new Vector2();
            U.x = (CA.y * BA.sqrMagnitude - BA.y * CA.sqrMagnitude) / D;
            U.y = (BA.x * CA.sqrMagnitude - CA.x * BA.sqrMagnitude) / D;

            u = U + vertices[0];

            circumcenterCalc = true;
            circumcenter = u;
            circumRadius = U.magnitude;

            return circumRadius;
        }
    }

    public bool IsPointInTriangle(Vector2 point)
    {
        if((point - Circumcenter()).magnitude > circumRadius)
        {
            //OUT OF CIRCUMCIRCLE
            return false;
        }
        else
        {
            //IN CIRCUMCIRCLE
            Vector2 a, b, c;

            a = vertices[1] - vertices[0];
            b = vertices[2] - vertices[1];
            c = vertices[3] - vertices[2];

            Vector2 f, g, h;

            f = point - vertices[0];
            g = point - vertices[1];
            h = point - vertices[2];

            return UtilityMath.Vector2Product(f, a) > 0 && UtilityMath.Vector2Product(g, b) > 0 && UtilityMath.Vector2Product(h, c) > 0;
        }
    }
}