using UnityEngine;

public abstract class TacticAction
{

}
public class MoveTo : TacticAction
{
    public int unitID;

    public Vector2 pos;
    public Quaternion dir;
}
public class FaceTarget : TacticAction
{
    public int unitID;

    public Quaternion dir;
}
public class AttackEnemy : TacticAction
{
    public int unitID;
    public int enemyID;
}