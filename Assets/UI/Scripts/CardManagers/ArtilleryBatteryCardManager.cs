using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ArtilleryBatteryCardManager : MonoBehaviour
{
    public Image cardImage;
    public Slider ammoSlider;

    private Factions faction = Factions.France;

    private void Start()
    {
        Initialize(faction);
    }

    public void Initialize(Factions faction)
    {
        this.faction = faction;
        
        SetImage(GFXUtility.GetFlag(faction));
    }

    public void SetImage(Sprite sprite)
    {
        cardImage.sprite = sprite;
    }

    public void SetAmmoSlider(int ammo, int maxAmmo)
    {
        ammoSlider.value = (float)ammo / (float)maxAmmo;
    }

    public void HighLight(bool highlight)
    {
        if (highlight)
        {
            cardImage.color = Color.yellow;
        }
        else
        {
            cardImage.color = Color.white;
        }
    }
}
