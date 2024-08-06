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
    public int[] assignedBattalions;
    public int[] targetBattalions;

    public AttackEnemyTroops(int[] assignedBattalions, int[] targetBattalions)
    {
        this.assignedBattalions = assignedBattalions;
        this.targetBattalions = targetBattalions;
    }

    public override void Execute()
    {
        
    }

    public float CalculateDifficulty()
    {
        float assignedBattallionStrenght = 0f;
        float targetBattallionStrenght = 0f;

        for(int i = 0; i < assignedBattalions.Length; i++)
        {
            assignedBattallionStrenght += GroundBattleUtility.GetBattalionByID(assignedBattalions[i]).GetStrenght();
        }

        for (int i = 0; i < targetBattalions.Length; i++)
        {
            targetBattallionStrenght += GroundBattleUtility.GetBattalionByID(targetBattalions[i]).GetStrenght();
        }

        return assignedBattallionStrenght / targetBattallionStrenght;
    }
}