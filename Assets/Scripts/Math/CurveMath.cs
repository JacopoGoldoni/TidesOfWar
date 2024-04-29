using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Curve2
{
    public abstract Vector2 PointAt(float t);
    public abstract Vector2 TangentAt(float t);
    //public abstract Vector2 AccelleratioAt(float t);
    public float GetLenght(int resolution)
    {
        float l = 0f;
        for(int i = 0; i < resolution; i++)
        {
            l += (PointAt((i+1)/resolution) - PointAt(i/resolution)).magnitude;
        }

        return l;
    }

}

public class Bezier2 : Curve2
{
 
    public List<Vector2> Points = new List<Vector2>();

    public int GetOrder()
    {
        return Points.Count;
    }

    public override Vector2 PointAt(float t)
    {
        Vector2 p = new Vector2(0, 0);

        int o = GetOrder();

        for(int i = 0; i < o; i++)
        {
            p += UtilityMath.BernsteinBasisPolynomial(o, i, Mathf.Clamp01(t)) * Points[i];
        }

        return p;
    }

    public override Vector2 TangentAt(float t)
    {
        Vector2 tangent = new Vector2();

        int n = GetOrder();

        for(int i = 0; i <= n - 1; i++)
        {
            tangent += UtilityMath.BernsteinBasisPolynomial(i, n - 1, Mathf.Clamp01(t)) * (Points[i + 1] - Points[i]);
        }

        tangent *= n;

        return tangent;
    }

}

public class RationalBezier2 : Curve2
{
    public List<(Vector2, float)> Points = new List<(Vector2, float)>();

    public int GetOrder()
    {
        return Points.Count;
    }

    public override Vector2 PointAt(float t)
    {
        Vector2 p = new Vector2(0, 0);

        int o = GetOrder();

        float b = 0f;

        //Sum of weighted bernstein polynomials
        for (int i = 0; i < o; i++)
        {
            b += UtilityMath.BernsteinBasisPolynomial(o, i, Mathf.Clamp01(t)) * Points[i].Item2;
        }

        for (int i = 0; i < o; i++)
        {
            p += UtilityMath.BernsteinBasisPolynomial(o, i, Mathf.Clamp01(t)) * Points[i].Item1 * Points[i].Item2;
            p = p / b;

        }

        return p;
    }

    public override Vector2 TangentAt(float t)
    {
        throw new System.NotImplementedException();
    }
}

public class CubicSpline : Curve2
{
    public List<Vector2> Points = new List<Vector2>(4);

    public override Vector2 PointAt(float t)
    {
        Matrix T = Matrix.PolynomMatrix(t,4).Transposed();
        Matrix CS = Matrix.CubicSplineMatrix();

        Matrix P = new Matrix(2, 4);

        P.values[0, 0] = Points[0].x;
        P.values[0, 1] = Points[0].y;

        P.values[1, 0] = Points[1].x;
        P.values[1, 1] = Points[1].y;

        P.values[2, 0] = Points[2].x;
        P.values[2, 1] = Points[2].y;

        P.values[3, 0] = Points[3].x;
        P.values[3, 1] = Points[3].y;

        return Matrix.MatrixToVector2(T * CS * P);
    }

    public override Vector2 TangentAt(float t)
    {

        Matrix T = Matrix.DerivatedPolynomialMatrix(t, 4).Transposed();
        Matrix CS = Matrix.CubicSplineMatrix();

        Matrix P = new Matrix(2, 4);

        P.values[0, 0] = Points[0].x;
        P.values[0, 1] = Points[0].y;

        P.values[1, 0] = Points[1].x;
        P.values[1, 1] = Points[1].y;

        P.values[2, 0] = Points[2].x;
        P.values[2, 1] = Points[2].y;

        P.values[3, 0] = Points[3].x;
        P.values[3, 1] = Points[3].y;

        return Matrix.MatrixToVector2(T * CS * P);
    }
}

public class HermiteSpline : Curve2
{
    public Vector2 P0;
    public Vector2 P1;
    public Vector2 V0;
    public Vector2 V1;

    public Matrix H;
    public Matrix A;

    public HermiteSpline(Vector2 P0, Vector2 P1, Vector2 V0, Vector2 V1)
    {
        this.P0 = P0;
        this.P1 = P1;
        this.V0 = V0;
        this.V1 = V1;

        //INITIALIZE MATRIXES
        H = Matrix.HermiteMatrix();

        A = new Matrix(2,4);

        A.values[0, 0] = P0.x;
        A.values[0, 1] = P0.y;

        A.values[1, 0] = P1.x;
        A.values[1, 1] = P1.y;

        A.values[2, 0] = V0.x;
        A.values[2, 1] = V0.y;

        A.values[3, 0] = V1.x;
        A.values[3, 1] = V1.y;
    }

    public override Vector2 PointAt(float t)
    {
        Matrix T = Matrix.PolynomMatrix(Mathf.Clamp01(t), 4).Transposed();

        return Matrix.MatrixToVector2((T * H * A).Transposed());
    }

    public override Vector2 TangentAt(float t)
    {
        Matrix T = Matrix.DerivatedPolynomialMatrix(Mathf.Clamp01(t), 4).Transposed();

        return Matrix.MatrixToVector2((T * H * A).Transposed());
    }
}

public class CardinalSpline : Curve2
{
    public Vector2 P0;
    public Vector2 P1;
    public Vector2 P2;
    public Vector2 P3;

    public Matrix H;
    public Matrix A;

    public CardinalSpline(Vector2 P0, Vector2 P1, Vector2 P2, Vector2 P3, float s)
    {
        this.P0 = P0;
        this.P1 = P1;
        this.P2 = P2;
        this.P3 = P3;

        //INITIALIZE MATRIXES
        H = Matrix.CardinalSplineMatrix(s);

        A = new Matrix(2, 4);

        A.values[0, 0] = P0.x;
        A.values[0, 1] = P0.y;

        A.values[1, 0] = P1.x;
        A.values[1, 1] = P1.y;

        A.values[2, 0] = P2.x;
        A.values[2, 1] = P2.y;

        A.values[3, 0] = P3.x;
        A.values[3, 1] = P3.y;
    }

    public override Vector2 PointAt(float t)
    {
        Matrix T = Matrix.PolynomMatrix(Mathf.Clamp01(t), 4).Transposed();

        return Matrix.MatrixToVector2((T * H * A).Transposed());
    }

    public override Vector2 TangentAt(float t)
    {
        Matrix T = Matrix.DerivatedPolynomialMatrix(Mathf.Clamp01(t), 4).Transposed();

        return Matrix.MatrixToVector2((T * H * A).Transposed());
    }
}

public class CatmullRomSpline : Curve2
{
    public Vector2 P0;
    public Vector2 P1;
    public Vector2 P2;
    public Vector2 P3;

    public Matrix H;
    public Matrix A;

    public CatmullRomSpline(Vector2 P0, Vector2 P1, Vector2 P2, Vector2 P3, float s)
    {
        this.P0 = P0;
        this.P1 = P1;
        this.P2 = P2;
        this.P3 = P3;

        //INITIALIZE MATRIXES
        H = Matrix.CatmullRomSplineMatrix() * 0.5f;

        A = new Matrix(2, 4);

        A.values[0, 0] = P0.x;
        A.values[0, 1] = P0.y;

        A.values[1, 0] = P1.x;
        A.values[1, 1] = P1.y;

        A.values[2, 0] = P2.x;
        A.values[2, 1] = P2.y;

        A.values[3, 0] = P3.x;
        A.values[3, 1] = P3.y;
    }

    public override Vector2 PointAt(float t)
    {
        Matrix T = Matrix.PolynomMatrix(Mathf.Clamp01(t), 4).Transposed();

        return Matrix.MatrixToVector2((T * H * A).Transposed());
    }

    public override Vector2 TangentAt(float t)
    {
        Matrix T = Matrix.DerivatedPolynomialMatrix(Mathf.Clamp01(t), 4).Transposed();

        return Matrix.MatrixToVector2((T * H * A).Transposed());
    }
}

public class BSpline : Curve2
{
    public Vector2 P0;
    public Vector2 P1;
    public Vector2 P2;
    public Vector2 P3;

    public Matrix H;
    public Matrix A;

    public BSpline(Vector2 P0, Vector2 P1, Vector2 P2, Vector2 P3, float s)
    {
        this.P0 = P0;
        this.P1 = P1;
        this.P2 = P2;
        this.P3 = P3;

        //INITIALIZE MATRIXES
        H = Matrix.BSplineMatrix() * (1 / 6f);

        A = new Matrix(2, 4);

        A.values[0, 0] = P0.x;
        A.values[0, 1] = P0.y;

        A.values[1, 0] = P1.x;
        A.values[1, 1] = P1.y;

        A.values[2, 0] = P2.x;
        A.values[2, 1] = P2.y;

        A.values[3, 0] = P3.x;
        A.values[3, 1] = P3.y;
    }

    public override Vector2 PointAt(float t)
    {
        Matrix T = Matrix.PolynomMatrix(Mathf.Clamp01(t), 4).Transposed();

        return Matrix.MatrixToVector2((T * H * A).Transposed());
    }

    public override Vector2 TangentAt(float t)
    {
        Matrix T = Matrix.DerivatedPolynomialMatrix(Mathf.Clamp01(t), 4).Transposed();

        return Matrix.MatrixToVector2((T * H * A).Transposed());
    }
}