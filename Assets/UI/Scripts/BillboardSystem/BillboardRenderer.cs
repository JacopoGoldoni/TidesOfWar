using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardRenderer : MonoBehaviour
{
    public Sprite billboardSprite;

    public bool IsBillboardInView()
    {
        return GetComponent<Renderer>().isVisible;
    }
}