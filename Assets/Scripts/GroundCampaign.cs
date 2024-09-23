using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ground battle scene", menuName = "ScriptableObjects/Scenes/Ground campaign")]
public class GroundCampaign : ScriptableObject
{
    public int[] battle_ID;

    public int difficultyLevel;
}