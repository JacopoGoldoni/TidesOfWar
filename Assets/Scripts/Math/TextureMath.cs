using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public static class TextureMath
{
    public static Texture2D Summation(Texture2D A, Texture2D B)
    {
        Texture2D C = new Texture2D(A.width, A.height);

        for(int i = 0; i < A.width; i++)
        {
            for (int j = 0; j < A.height; j++)
            {
                Color value = A.GetPixel(i, j) + B.GetPixel(i, j);
                C.SetPixel(i, j, value);
            }
        }

        C.Apply();
        return C;
    }
    public static Texture2D Subtraction(Texture2D A, Texture2D B)
    {
        Texture2D C = new Texture2D(A.width, A.height);

        for (int i = 0; i < A.width; i++)
        {
            for (int j = 0; j < A.height; j++)
            {
                Color value = A.GetPixel(i, j) - B.GetPixel(i, j);
                C.SetPixel(i, j, value);
            }
        }

        C.Apply();
        return C;
    }
    public static Texture2D Multiplication(Texture2D A, Texture2D B)
    {
        Texture2D C = new Texture2D(A.width, A.height);

        for (int i = 0; i < A.width; i++)
        {
            for (int j = 0; j < A.height; j++)
            {
                Color value = A.GetPixel(i, j) * B.GetPixel(i, j);
                C.SetPixel(i, j, value);
            }
        }

        C.Apply();
        return C;
    }
    public static Texture2D Multiplication(Texture2D A, float s)
    {
        Texture2D C = new Texture2D(A.width, A.height);

        for (int i = 0; i < A.width; i++)
        {
            for (int j = 0; j < A.height; j++)
            {
                Color value = A.GetPixel(i, j) * s;
                C.SetPixel(i, j, value);
            }
        }

        C.Apply();
        return C;
    }
    public static Texture2D Invert(Texture2D A)
    {
        Texture2D C = new Texture2D(A.width, A.height);

        for (int i = 0; i < A.width; i++)
        {
            for (int j = 0; j < A.height; j++)
            {
                Color value = Color.white - A.GetPixel(i, j);
                C.SetPixel(i, j, value);
            }
        }

        C.Apply();
        return C;
    }
    public static Texture2D Clear(Texture2D A)
    {
        Texture2D B = new Texture2D(A.width,A.height);
        for (int i = 0; i < B.width; i++)
        {
            for (int j = 0; j < B.height; j++)
            {
                B.SetPixel(i, j, Color.black);
            }
        }
        B.Apply();
        return B;
    }

    public static Texture2D PerlinTexture(int worldSize, float scale)
    {
        Texture2D texture = new Texture2D(worldSize, worldSize);

        for(int i = 0; i < worldSize; i++)
        {
            for (int j = 0; j < worldSize; j++)
            {
                float x = ((float)i / (float)worldSize) * scale;
                float y = ((float)j / (float)worldSize) * scale;
                float value = Mathf.PerlinNoise(x, y);
                texture.SetPixel(i, j, new Color(value, value, value));
            }
        }

        texture.Apply();
        return texture;
    }
    public static Texture2D AdvancedPerlinTexture(int worldSize, float scale, int octaves, float lacunarity, float persistence)
    {
        Texture2D texture = new Texture2D(worldSize, worldSize);
        texture = Clear(texture);

        float maxHeight = 0f;
        for(int i = 0; i < octaves; i++)
        {
            maxHeight += (float)Math.Pow(persistence, i);
        }

        for (int i = 0; i < octaves; i++)
        {
            Texture2D octave = new Texture2D(worldSize, worldSize);

            octave = PerlinTexture(worldSize, scale * Mathf.Pow(lacunarity, i));
            octave = Multiplication(octave, 1f / (Mathf.Pow(persistence, i) * maxHeight));

            texture = Summation(texture, octave);
        }

        texture.Apply();
        return texture;
    }
    public static Texture2D Erode(Texture2D A, Erosion erosion,int erosionIterations, int channel)
    {
        Texture2D erodedTexture = new Texture2D(A.width, A.height);

        float[] values = new float[A.width * A.width];
        for (int i = 0; i < A.width; i++)
        {
            for (int j = 0; j < A.height; j++)
            {
                values[i + j * A.width] = A.GetPixel(i, j)[channel];
            }
        }

        erosion.Erode(values, A.width, erosionIterations, false);

        float[,] valuesMatrix = new float[A.width, A.height];
        for (int i = 0; i < A.width; i++)
        {
            for (int j = 0; j < A.height; j++)
            {
                valuesMatrix[i, j] = values[i + j * A.width];
            }
        }

        Insert(erodedTexture, valuesMatrix, channel);

        erodedTexture.Apply();
        return erodedTexture;
    }

    public static float[,] Extract(Texture2D texture, int channel)
    {
        float[,] values = new float[texture.width, texture.width];

        for(int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                values[j, i] = texture.GetPixel(i, j)[channel];
            }
        }

        return values;
    }
    public static void Insert(Texture2D texture, float[,] values,int channel)
    {
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                Color c = texture.GetPixel(i, j);
                c[channel] = values[i, j];
                texture.SetPixel(i, j, c);
            }
        }

        texture.Apply();
    }
}