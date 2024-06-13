using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechnologySlot
{
    public enum TechState { Locked, Unlocked, Researching, Learned }
    TechState techState;

    public int techID;
    public int days = 0;

    public bool isUnlocked()
    {
        if(techState != TechState.Locked)
        {
            return true;
        }
        return false;
    }
    public void Unlock()
    {
        techState = TechState.Unlocked;
    }
    public bool isLearned()
    {
        if (techState == TechState.Learned)
        {
            return true;
        }
        return false;
    }
    public void Learn()
    {
        techState = TechState.Learned;
    }

    public void AddDay()
    {
        days++;
        if(days >= WorldUtility.GetTechByID(techID).researchDays)
        {
            Unlock();
        }
    }
}

[CreateAssetMenu(fileName = "Tech", menuName = "ScriptableObjects/Tech")]
public class Technology : ScriptableObject
{
    public int techID;
    public string name;
    public string description;
    public Sprite techImage;

    public int researchDays;

    public int[] required_techID;
}