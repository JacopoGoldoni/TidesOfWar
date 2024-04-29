using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RegimentCardManager : MonoBehaviour
{
    Image cardImage;
    public Slider ammoSlider;
    public Text cardText;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        cardImage = GetComponent<Image>();

        SetImage(Resources.Load<Sprite>("GFX/FRA_democratic"));
        
        cardText.alignment = TextAnchor.LowerCenter;
        cardText.text = "-Placeholder-";
        cardText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font; ;
    }

    public void SetImage(Sprite sprite)
    {
        cardImage.sprite = sprite;
    }

    public void SetText(string text)
    {
        cardText.text = text;
    }

    public void SetAmmoSlider(int ammo, int maxAmmo)
    {
        ammoSlider.value = (float)ammo / (float)maxAmmo;
    }
}
