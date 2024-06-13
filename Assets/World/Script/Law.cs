using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Law
{
    public string name;
    public string category;
    public Sprite lawSprite;

    [Header("Effects")]
    public float conscription;
    public float tax;
    public float stability;
    public float suppression;

    [Header("Opinions")]
    public float noble_opinion;
    public float clergy_opinion;
    public float bourgeoisie_opinion;
    public float proletarian_opinion;
}