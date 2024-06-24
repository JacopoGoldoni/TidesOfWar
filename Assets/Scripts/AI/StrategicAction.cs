using UnityEngine;

public abstract class StrategicAction
{
    public abstract void Execute();
}
public class Retreat : StrategicAction
{
    public override void Execute()
    {

    }
}
public class ConquerFlag : StrategicAction
{
    public int[] battalions;
    public int flag;

    public ConquerFlag(int[] battalions, int flag)
    {
        this.battalions = battalions;
        this.flag = flag;
    }

    public override void Execute()
    {

    }
}
public class DefendFlag : StrategicAction
{
    public int[] battalions;
    public int flag;

    public DefendFlag(int[] battalions, int flag)
    {
        this.battalions = battalions;
        this.flag = flag;
    }

    public override void Execute()
    {

    }
}
public class AttackEnemyTroops : StrategicAction
{
    public int[] battalions;

    public AttackEnemyTroops(int[] battalions)
    {
        this.battalions = battalions;
    }

    public override void Execute()
    {

    }
}