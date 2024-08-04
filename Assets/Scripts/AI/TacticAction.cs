using UnityEngine;

public abstract class TacticAction
{
    public int unitID;
}


public class MoveTo : TacticAction
{
    public Vector2 pos;
    public Quaternion dir;
    public bool fastMarch;
}
public class FaceTarget : TacticAction
{
    public Quaternion dir;
}
public class AttackEnemy : TacticAction
{
    public int enemyID;
}
public class AttackNearest : TacticAction
{

}