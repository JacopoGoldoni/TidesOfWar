using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Formation
{
    public string name { get; set; }

    public int Lines { get; set; }
    public int Ranks { get; set; }
    public float a = 0.5f;
    public float b = 1f;

    public abstract Vector2 GetPos(int ID);

    public int GetRank(int ID)
    {
        int y = (ID / Lines + 1);

        return y;
    }

    public void SetSizeByRanks(int size, int ranks)
    {
        Ranks = ranks;
        Lines = size / ranks;

        if(Lines == 0)
        {
            Lines = size;
        }
    }

    public void SetSizeByLines(int size, int lines)
    {
        Ranks = size / lines;
        Lines = lines;
    }
}

public class Line : Formation
{
    public Line(int size)
    {
        name = "Line";

        //Default 3 RANKS
        SetSizeByRanks(size, 3);
    }

    public override Vector2 GetPos(int ID)
    {
        Vector2 pos;

        if(Lines == 1)
        {
            pos.x = 0;
        }
        else
        {
            pos.x = ((ID % Lines) - (float)(Lines - 1) / 2f) * a;
        }

        pos.y = ((ID / Lines + 1) - 1.5f) * b;

        return pos;
    }
}

public class Column : Formation
{
    public Column(int size)
    {
        name = "Column";

        //DEFAULT 4 LINES
        SetSizeByLines(size, 3);
    }

    public override Vector2 GetPos(int ID)
    {
        Vector2 pos;

        if (Lines == 1)
        {
            pos.x = 0;
        }
        else
        {
            pos.x = ((ID % Lines) - (float)(Lines-1) / 2f ) * a;
        }

        pos.y = ((ID / Lines + 1)) * b;

        return pos;
    }
}