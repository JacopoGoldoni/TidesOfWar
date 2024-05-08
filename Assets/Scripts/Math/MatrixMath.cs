using Den.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Matrix
{
    public float[,] values;

    public int sizeX, sizeY = 0;

    public Matrix(int w, int h)
    {
        sizeX = w;
        sizeY = h;

        values = new float[w, h];
    }

    //OPERATORS
    public static Matrix operator +(Matrix a, Matrix b)
    {
        Matrix c = new Matrix(a.sizeX, a.sizeY);

        for(int i = 0; i < c.sizeX; i++)
        {
            for (int j = 0; j < c.sizeY; j++)
            {
                c.values[i, j] = a.values[i, j] + b.values[i, j];
            }
        }

        return c;
    }
    public static Matrix operator -(Matrix a, Matrix b)
    {
        Matrix c = new Matrix(a.sizeX, a.sizeY);

        for (int i = 0; i < c.sizeX; i++)
        {
            for (int j = 0; j < c.sizeY; j++)
            {
                c.values[i, j] = a.values[i, j] - b.values[i, j];
            }
        }

        return c;
    }
    public static Matrix operator *(Matrix a, int b)
    {
        Matrix c = new Matrix(a.sizeX, a.sizeY);

        for (int i = 0; i < c.sizeX; i++)
        {
            for (int j = 0; j < c.sizeY; j++)
            {
                c.values[i, j] = a.values[i, j] * b;
            }
        }

        return c;
    }
    public static Matrix operator *(Matrix a, float b)
    {
        Matrix c = new Matrix(a.sizeX, a.sizeY);

        for (int i = 0; i < c.sizeX; i++)
        {
            for (int j = 0; j < c.sizeY; j++)
            {
                c.values[i, j] = a.values[i, j] * b;
            }
        }

        return c;
    }
    public static Matrix operator *(Matrix a, Matrix b)
    {
        Matrix c = new Matrix(b.sizeX, a.sizeY);

        for (int i = 0; i < c.sizeX; i++)
        {
            for (int j = 0; j < c.sizeY; j++)
            {
                float f = 0;

                for(int k = 0; k < a.sizeX; k++)
                {
                    for (int l = 0; l < b.sizeY; l++)
                    {
                        f += a.values[k, j] * b.values[i, l];
                    }
                }

                c.values[i, j] = f;
            }
        }

        return c;
    }

    //CONVERTORS
    public static Matrix V2ToMatrix(Vector2 v2)
    {
        Matrix a = new Matrix(1, 2);

        a.values[0,0] = v2.x;
        a.values[0,1] = v2.y;

        return a;
    }
    public static Matrix V3ToMatrix(Vector3 v3)
    {
        Matrix a = new Matrix(1, 3);

        a.values[0, 0] = v3.x;
        a.values[0, 1] = v3.y;
        a.values[0, 2] = v3.z;

        return a;
    }
    public static Vector2 MatrixToVector2(Matrix m)
    {
        if(m.sizeY == 2 && m.sizeX == 1)
        {
            Vector2 v2 = new Vector2();

            v2.x = m.values[0, 0];
            v2.y = m.values[0, 1];

            return v2;
        }

        return Vector2.zero;
    }
    public static Vector3 MatrixToVector3(Matrix m)
    {
        if(m.sizeY == 3 && m.sizeX == 1)
        {
            Vector3 v3 = new Vector3();

            v3.x = m.values[0, 0];
            v3.y = m.values[0, 1];
            v3.z = m.values[0, 2];

            return v3;
        }

        return Vector3.zero;
    }

    //DECOMPOSITIONS
    public (Matrix, Matrix) QR_Decomposition()
    {
        List<Matrix> Qs = new List<Matrix>();

        //Qs matrixes calculation
        for(int t = 0; t < sizeX - 1; t++)
        {
            Matrix A = new Matrix(sizeX, sizeY);
            if(t == 0)
            {
                A = this;
            }

            Matrix x = A.Column(0);
            
            float a = 0;
            for (int i = 0; i < x.sizeY; i++)
            {
                a += Mathf.Pow(x.values[0, i], 2);
            }
            a = Mathf.Sqrt(a);

            Matrix u = x + Identity(this.sizeX).Column(0) * a;

            float b = 0;
            for (int i = 0; i < u.sizeY; i++)
            {
                b += Mathf.Pow(u.values[0, i], 2);
            }
            b = Mathf.Sqrt(b);

            Matrix v = u * (1f / b);

            Qs[t] = HouseholderMatrix(v);

            Matrix M = Qs[t] * this;
            A = M.ExtractMatrix((1, 1), (M.sizeX - 1, M.sizeY - 1));
        }

        Matrix Q = Identity(sizeX);
        for(int i = 0; i < Qs.Count; i++)
        {
            Q *= Qs[i];
        }

        Matrix R = Q.Transposed() * this;

        return (Q, R);
    }

    //EIGENVALUES
    public List<float> Eigenvalues(int n)
    {
        Matrix A = this;

        for(int k = 0; k < n; k++)
        {
            (Matrix, Matrix) QR = A.QR_Decomposition();
            A = QR.Item2 * QR.Item1;
        }

        List<float> eigenvalues = new List<float>(sizeX);

        for(int i = 0; i < sizeX; i++)
        {
            eigenvalues[i] = A.values[i,i];
        }

        return eigenvalues;
    }

    //UTILITY
    public static Matrix Join(Matrix a, Matrix b)
    {
        Matrix c = new Matrix(a.sizeX + b.sizeX, a.sizeY);
        
        //COPY MATRIX A
        for(int i = 0; i < a.sizeX; i++)
        {
            for (int j = 0; j < c.sizeY; j++)
            {
                c.values[i, j] = a.values[i, j];
            }
        }

        //COPY MATRIX B
        for (int i = 0; i < b.sizeX; i++)
        {
            for (int j = 0; j < c.sizeY; j++)
            {
                c.values[i + a.sizeX, j] = b.values[i, j];
            }
        }

        return c;
    }
    public void Resize(int newSizeX, int newSizeY)
    {
        float[,] copiedValues = new float[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                copiedValues[i, j] = values[i, j];
            }
        }

        values = new float[newSizeX, newSizeY];

        for(int i = 0; i < newSizeX; i++)
        {
            for (int j = 0; j < newSizeY; j++)
            {
                if(i < sizeX && j < sizeY)
                {
                    values[i, j] = copiedValues[i, j];
                }
                else
                {
                    values[i, j] = 0;
                }
            }
        }
    }
    public Matrix Transposed()
    {
        Matrix t = new Matrix(sizeY, sizeX);

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                t.values[j, i] = values[i, j];
            }
        }

        return t;
    }
    public Matrix Column(int n)
    {
        if(n >= 0 && n < sizeX)
        {
            Matrix c = new Matrix(1, sizeY);
            
            for(int j = 0; j < sizeY; j++)
            {
                c.values[0, j] = values[n, j];
            }
            
            return c;
        }
        else
        {
            return null;
        }
    }
    public Matrix Row(int n)
    {
        if (n >= 0 && n < sizeY)
        {
            Matrix r = new Matrix(sizeX, 1);

            for (int i = 0; i < sizeX; i++)
            {
                r.values[i, 0] = values[i, n];
            }

            return r;
        }
        else
        {
            return null;
        }
    }
    public Matrix ExtractMatrix((int, int) a, (int, int) b)
    {
        Matrix m = new Matrix(b.Item1 - a.Item1, b.Item2 - a.Item2);

        for(int i = a.Item1; i <= b.Item1; i++)
        {
            for (int j = a.Item2; j <= b.Item2; j++)
            {
                m.values[i - a.Item1,j - a.Item2] = values[i,j];
            }
        }

        return m;
    }

    //DEBUG
    public static void PrintMatrix(Matrix m)
    {
        string s = "\n";

        for (int i = 0; i < m.sizeX; i++)
        {
            for (int j = 0; j < m.sizeY; j++)
            {
                s += m.values[i, j].ToString() + ", ";
            }
            s += "\n";
        }

        Debug.Log(s);

        return;
    }

    //CONSTRUCTORS
    public static Matrix Identity(int order)
    {
        Matrix I = new Matrix(order, order);

        for(int i = 0; i < order; i++)
        {
            for (int j = 0; j < order; j++)
            {
                if(i == j)
                {
                    I.values[i, j] = 1;
                }
                else
                {
                    I.values[i, j] = 0;
                }
            }
        }

        return I;
    }
    public static Matrix PolynomMatrix(float x, int grade)
    {
        Matrix p = new Matrix(1, grade);

        for(int i = 0; i <= grade; i++)
        {
            p.values[0, i] = Mathf.Pow(x, i);
        }

        return p;
    }
    public static Matrix DerivatedPolynomialMatrix(float x, int grade)
    {
        Matrix P = PolynomMatrix(x, grade);
        Matrix P1 = new Matrix(1, grade);

        for(int i = 0; i <= grade; i ++)
        {
            P1.values[0, i] = i * Mathf.Pow(x, i - 1);
        }

        return P1;
    }
    public static Matrix HermiteMatrix()
    {
        Matrix H = new Matrix(4, 4);

        H.values[0, 0] = 1;
        H.values[1, 0] = 0;
        H.values[2, 0] = 0;
        H.values[3, 0] = 0;

        H.values[0, 1] = 0;
        H.values[1, 1] = 1;
        H.values[2, 1] = 0;
        H.values[3, 1] = 0;

        H.values[0, 2] = -3;
        H.values[1, 2] = -2;
        H.values[2, 2] = 3;
        H.values[3, 2] = -1;

        H.values[0, 3] = 2;
        H.values[1, 3] = 1;
        H.values[2, 3] = -2;
        H.values[3, 3] = 1;

        return H;
    }
    public static Matrix HouseholderMatrix(Matrix column)
    {
        Matrix A = new Matrix(column.sizeY, column.sizeY);

        A = Identity(column.sizeY);
        A = A - ((column * column.Transposed()) * 2);

        return A;
    }
    public static Matrix CubicSplineMatrix()
    {
        Matrix m = new Matrix(4, 4);

        m.values[0, 0] = 1f;
        m.values[0, 1] = 0f;
        m.values[0, 2] = 0f;
        m.values[0, 3] = 0f;

        m.values[1, 0] = -3f;
        m.values[1, 1] = 3f;
        m.values[1, 2] = 0f;
        m.values[1, 3] = 0f;

        m.values[2, 0] = 3f;
        m.values[2, 1] = -6f;
        m.values[2, 2] = 3f;
        m.values[2, 3] = 0f;

        m.values[3, 0] = -1f;
        m.values[3, 1] = 3f;
        m.values[3, 2] = -3f;
        m.values[3, 3] = 1f;

        return m;
    }
    public static Matrix CardinalSplineMatrix(float s)
    {
        Matrix M = new Matrix(4, 4);

        M.values[0, 0] = 0f;
        M.values[0, 1] = 1f;
        M.values[0, 2] = 0f;
        M.values[0, 3] = 0f;

        M.values[1, 0] = -s;
        M.values[1, 1] = 0f;
        M.values[1, 2] = s;
        M.values[1, 3] = 0f;

        M.values[2, 0] = 2*s;
        M.values[2, 1] = s - 3;
        M.values[2, 2] = 3 - 2*s;
        M.values[2, 3] = -s;

        M.values[3, 0] = -s;
        M.values[3, 1] = 2 - s;
        M.values[3, 2] = s - 2;
        M.values[3, 3] = s;

        return M;
    }
    public static Matrix CatmullRomSplineMatrix()
    {
        Matrix m = new Matrix(4, 4);

        m.values[0, 0] = 0f;
        m.values[0, 1] = 2f;
        m.values[0, 2] = 0f;
        m.values[0, 3] = 0f;

        m.values[1, 0] = -1f;
        m.values[1, 1] = 0f;
        m.values[1, 2] = 1f;
        m.values[1, 3] = 0f;

        m.values[2, 0] = 2f;
        m.values[2, 1] = -5f;
        m.values[2, 2] = 4f;
        m.values[2, 3] = -1f;

        m.values[3, 0] = -1f;
        m.values[3, 1] = 3f;
        m.values[3, 2] = -3f;
        m.values[3, 3] = 1f;

        return m;
    }
    public static Matrix BSplineMatrix()
    {
        Matrix m = new Matrix(4, 4);

        m.values[0, 0] = 1f;
        m.values[0, 1] = 4f;
        m.values[0, 2] = 1f;
        m.values[0, 3] = 0f;

        m.values[1, 0] = -3f;
        m.values[1, 1] = 0f;
        m.values[1, 2] = 3f;
        m.values[1, 3] = 0f;

        m.values[2, 0] = 3f;
        m.values[2, 1] = -6f;
        m.values[2, 2] = 3f;
        m.values[2, 3] = 0f;

        m.values[3, 0] = -1f;
        m.values[3, 1] = 3f;
        m.values[3, 2] = -3f;
        m.values[3, 3] = 1f;

        return m;
    }
}