using UnityEngine;

public abstract class Sensor
{

}

public class FriendlySensor : Sensor
{
    public int unitID;
    public int unitType;

    public Vector2 pos;

    public int Size;

    public FriendlySensor(int unitID, int unitType, Vector2 pos, int Size)
    {
        this.unitID = unitID;
        this.unitType = unitType;
        this.pos = pos;
        this.Size = Size;
    }
}

public class HostileSensor : Sensor
{
    public int unitID;
    public int unitType;

    public Vector2 pos;
    
    public int Size;

    public HostileSensor(int unitID, int unitType, Vector2 pos, int Size)
    {
        this.unitID = unitID;
        this.unitType = unitType;
        this.pos = pos;
        this.Size = Size;
    }
}

public class EnviromentSensor : Sensor
{

}