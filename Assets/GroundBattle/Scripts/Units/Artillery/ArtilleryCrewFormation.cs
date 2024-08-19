using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryCrewFormation
{
    //POSITIONS
    Vector2[] FirstClassGunnerPositions;
    Vector2[] SecondClassGunnerPositions;

    public ArtilleryCrewFormation(Vector2[] FirstClassGunnerPositions, Vector2[] SecondClassGunnerPositions)
    {
        this.FirstClassGunnerPositions = FirstClassGunnerPositions;
        this.SecondClassGunnerPositions = SecondClassGunnerPositions;
    }

    public Vector2 GetPos(int GunnerClass, int index)
    {
        Vector2 pos = Vector2.zero;

        switch(GunnerClass)
        {
            case 1:
                pos = FirstClassGunnerPositions[index];
                break;
            case 2:
                pos = SecondClassGunnerPositions[index];
                break;
        }

        return pos;
    }
}