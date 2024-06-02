using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ship armament", menuName = "ScriptableObjects/Ship/ShipArmament")]
public class ShipArmament : ScriptableObject
{
    public float reloadTime;
    public int damage;
    public float turningSpeed;
}