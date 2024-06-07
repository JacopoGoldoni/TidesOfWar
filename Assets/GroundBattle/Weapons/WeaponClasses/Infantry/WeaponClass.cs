using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Weapon class", menuName = "ScriptableObjects/Weapon")]
public class WeaponClass : ScriptableObject
{
    public string weaponName;
    public WeaponType weaponType;

    public Sprite weaponSprite;
    public Mesh weaponMesh;

    public float ReloadTime;
    public float Precision;
    public float Range;

    public int Cost;
}

public enum WeaponType { Rifle, Pistol, Melee }