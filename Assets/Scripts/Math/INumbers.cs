using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CNumber
{
    float real;
    float imaginary;

    public CNumber()
    {
        real = 0;
        imaginary = 0;
    }

    public CNumber(float real, float imaginary)
    {
        this.real = real;
        this.imaginary = imaginary;
    }
    public CNumber FromPolarForm(float magnitude, float phase)
    {
        float real = magnitude * Mathf.Cos(phase);
        float imaginary = magnitude * Mathf.Sin(phase);

        return new CNumber(real, imaginary);
    }
    public (float, float) GetPolarForm()
    {
        float magnitude = this.Abs();
        float phase = Mathf.Atan(this.imaginary / this.real);

        if(this.imaginary >= 0)
        {
            if (this.real >= 0)
            {

            }
            else
            {
                phase = phase + Mathf.PI;
            }
        }
        else
        {
            if (this.real >= 0)
            {
                phase = phase + 2 * Mathf.PI;
            }
            else
            {
                phase = phase + Mathf.PI;
            }
        }

        return (magnitude, phase);
    }

    public static CNumber operator +(CNumber a, CNumber b)
    {
        CNumber c = new CNumber(a.real + b.real, a.imaginary + b.imaginary);

        return c;
    }
    public static CNumber operator -(CNumber a, CNumber b)
    {
        CNumber c = new CNumber(a.real - b.real, a.imaginary - b.imaginary);

        return c;
    }
    public static CNumber operator *(CNumber a, CNumber b)
    {
        CNumber c = new CNumber(a.real * b.real - a.imaginary * b.imaginary, a.real * b.imaginary + a.imaginary * b.real);

        return c;
    }
    public static CNumber operator *(CNumber a, float s)
    {
        CNumber c = new CNumber(a.real * s, a.imaginary * s);
        return c;
    }
    public static CNumber operator ^(CNumber a, int n)
    {
        if (n == 0)
        {
            return new CNumber(1,0);
        }
        if (n == 1)
        {
            return a;
        }

        CNumber c = new CNumber(a.real, a.imaginary);

        for(int i = 2; i <= n; i++)
        {
            c = c * c;
        }

        return c;
    }
    public static CNumber operator /(CNumber a, CNumber b)
    {
        return a * b.Reciprocal();
    }

    public CNumber Exp(int n)
    {
        CNumber z = new CNumber();

        for(int i = 0; i <= n; i++)
        {
            z += (z ^ i) * (1f/(i ^ 2));
        }

        return z;
    }
    public List<CNumber> Roots(int n)
    {
        (float, float) polarForm = this.GetPolarForm();

        float f = UtilityMath.NRoot(polarForm.Item1, n);
        float eps = polarForm.Item2;

        List<float> phases = new List<float>();

        for(int k = 0; k <= n - 1; k++)
        {
            phases.Add((eps + k * 2 * Mathf.PI) / n);
        }

        List<CNumber> solutions = new List<CNumber>();

        foreach (float p in phases)
        {
            solutions.Add(FromPolarForm(f, p));
        }

        return solutions;
    }

    public CNumber Conjugate()
    {
        return new CNumber(real, -1 * imaginary);
    }
    public float Abs()
    {
        float a = Mathf.Sqrt(Mathf.Pow(real, 2) + Mathf.Pow(imaginary, 2));
        return a;
    }
    public CNumber Reciprocal()
    {
        CNumber c = Conjugate();

        float a = 1 / Mathf.Pow(Abs(), 2);

        return c * a;
    }
}