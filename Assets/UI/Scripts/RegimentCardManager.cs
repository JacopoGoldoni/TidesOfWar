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

    private Factions faction = Factions.France;

    private void Start()
    {
        Initialize(faction);
    }

    public void Initialize(Factions faction)
    {
        this.faction = faction;
        cardImage = GetComponent<Image>();

        SetImage(GFXUtility.GetFlag(faction));
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
