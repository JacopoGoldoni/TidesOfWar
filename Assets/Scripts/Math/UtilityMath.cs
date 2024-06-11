using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public static class UtilityMath
{
    public static float SmoothFunction(float x)
    {
        return 3 * x*x - 2 * x*x*x;
    }
    public static int Factorial(int x)
    {
        int f = 1;

        if( x == 0 || x == 1)
        {
            return 1;
        }

        for(int i = 2; i <= x; i++)
        {
            f = f * i;
        }

        return f;
    }

    public static int BinomialCoefficient(int n, int k)
    {
        if(k > n)
        {
            return 0;
        }

        int binomial = 0;

        int a = Factorial(n);
        int b = Factorial(k);
        int c = Factorial(n - k);

        binomial = a / (b * c);

        return binomial;
    }

    public static float BernsteinBasisPolynomial(int n, int i, float t)
    {
        float b = 0;

        b = BinomialCoefficient(n, i) * Mathf.Pow(t, i) * Mathf.Pow(1 - t, n - i);

        return b;
    }

    public static Vector2 RotateVector2(Vector2 v2, float angle)
    {
        return new Vector2(
            v2.x * Mathf.Cos(angle) - v2.y * Mathf.Sin(angle),
            v2.x * Mathf.Sin(angle) + v2.y * Mathf.Cos(angle)
            );
    }

    public static Vector2 RotateVector2(Vector2 v2)
    {
        return new Vector2(v2.y, -v2.x);
    }

    public static float BilinearInterpolation(float x, float y, float f00, float f01, float f10, float f11)
    {
        Matrix X = Matrix.V2ToMatrix( new Vector2(1f - x, x) );
        Matrix Y = Matrix.V2ToMatrix( new Vector2(1f - y, y) );

        Matrix M = new Matrix(2, 2);

        M.values[0, 0] = f00;
        M.values[0, 1] = f01;
        M.values[1, 0] = f10;
        M.values[1, 1] = f11;

        Matrix V = X.Transposed() * ( M * Y );

        return V.values[0, 0];
    }

    public static float Vector2Product(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    public static bool IsInCircularSector(Vector2 v, float minR, float maxR, Vector2 a, Vector2 b)
    {
        return (Vector2Product(v, a) >= 0) && (Vector2Product(b, v) >= 0) && (v.magnitude < maxR) && (v.magnitude > minR);
    }

    public static float AngleBetweenQuaternions(Quaternion A, Quaternion B)
    {
        Quaternion dq = Quaternion.Inverse(A) * B;

        float angle = 2 * Mathf.Atan2((Quaternion.ToEulerAngles(dq) * Mathf.Rad2Deg).magnitude, dq.w);

        return angle;
    }

    public static float Sigmoid(float x)
    {
        float s = Mathf.Exp(x);

        return s / (1 + s);
    }

    public static float NRoot(float A, int N)
    {
        float epsilon = 0.00001f;
        float n = N;
        float x = A / n;
        while(Mathf.Abs(A - Mathf.Pow(x, N)) > epsilon)
        {
            x = (1.0f / n) * ((n - 1) * x + (A / (Mathf.Pow(x, N - 1))));
        }
        return x;
    }

    public static Vector2 RandomPointInDisc(float Radius)
    {
        return Random.insideUnitCircle * Radius;
    }

    public static bool BoxCollisionDetection(Bounds b1, Quaternion q1, Bounds b2, Quaternion q2)
    {
        float OuterRadius1 = Utility.V3toV2(b1.extents).magnitude;
        float OuterRadius2 = Utility.V3toV2(b2.extents).magnitude;
        float InnerRadius1 = Mathf.Min(b1.extents.x, b1.extents.z);
        float InnerRadius2 = Mathf.Min(b2.extents.x, b2.extents.z);

        float Distance = Utility.V3toV2(b1.center - b2.center).magnitude;

        //ARE THE BOUNDS IN RANGE
        if(Distance > OuterRadius1 + OuterRadius2)
        {
            return false;
        }
        if (Distance < InnerRadius1 + InnerRadius2)
        {
            return true;
        }

        //CHECK IF VERTICES ARE IN BOUNDS
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if(i == 0 || j == 0)
                {
                    continue;
                }
                Vector3 v = new Vector3(i * b2.extents.x, 0, j * b2.extents.z);
                Vector3 vert2 = b2.center + q2 * v;

                if(b1.Contains(vert2))
                {
                    return true;
                }
            }
        }
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 || j == 0)
                {
                    continue;
                }
                Vector3 v = new Vector3(i * b1.extents.x, 0, j * b1.extents.z);
                Vector3 vert1 = b1.center + q1 * v;

                if (b2.Contains(vert1))
                {
                    return true;
                }
            }
        }
        return false;
    }
}