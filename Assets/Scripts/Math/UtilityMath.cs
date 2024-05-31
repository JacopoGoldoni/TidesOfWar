using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public static class UtilityMath
{
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
}