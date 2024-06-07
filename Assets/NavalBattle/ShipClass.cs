using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ship class", menuName = "ScriptableObjects/Ship/ShipClass")]
public class ShipClass : ScriptableObject
{
    public string fullName;

    public int max_hull;
    public int agility;
    public int max_thrust;
    public int max_speed;

    public GameObject shipPrefab;
    public Material shipMaterial;

    public ShipArmament[] shipArmament;
}