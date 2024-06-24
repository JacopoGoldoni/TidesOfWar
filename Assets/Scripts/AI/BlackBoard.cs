using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class BlackBoard
{
    private Dictionary<string, object> dataSet;

    public void AddData(string s, object o)
    {
        if(HasData(s))
        {
            dataSet[s] = o;
        }
        else
        {
            dataSet.Add(s, o);
        }
    }
    public object GetData(string s)
    {
        if(HasData(s))
        {
            return dataSet[s];
        }
        return null;
    }
    public bool HasData(string s)
    {
        return dataSet.ContainsKey(s);
    }
    public bool RemoveData(string s)
    {
        if(HasData(s))
        {
            dataSet.Remove(s);
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool ClearData(string s)
    {
        if(HasData(s))
        {
            dataSet[s] = null;
            return true;
        }
        else
        {
            return false;
        }
    }
    public void ClearAllData()
    {
        dataSet.Clear();
    }
}
