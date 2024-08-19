using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ground battle scene", menuName = "ScriptableObjects/Scenes/Ground battle")]
public class GroundBattleScene : ScriptableObject
{
    public int ID;

    public string[] faction_TAG;
    public string[] armies;

    public Vector2[] armies_spawn;

    public string battleHeightmap;
}