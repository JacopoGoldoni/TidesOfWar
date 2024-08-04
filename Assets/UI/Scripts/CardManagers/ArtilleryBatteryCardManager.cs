using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ArtilleryBatteryCardManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Image bg;
    public Image flagImage;
    public Image unitImage;
    public TextMeshProUGUI nameText;
    public Slider ammoSlider;

    public ArtilleryOfficerManager artilleryBatteryRef;

    public void Initialize(ArtilleryOfficerManager artilleryBattery)
    {
        bg = GetComponent<Image>();
        artilleryBatteryRef = artilleryBattery;
        artilleryBatteryRef.artilleryBatteryCardManager = this;

        SetFlag(artilleryBattery.faction);
        SetUnit(artilleryBattery.artilleryBatteryTemplate.ArtilleryBatteryIcon);
        SetBattalionTextName(artilleryBattery.batteryNumber, artilleryBattery.batteryName, artilleryBattery.artilleryBatteryTemplate.hardness);
        SetAmmoSlide();
    }

    public void SetFlag(Factions faction)
    {
        flagImage.sprite = GFXUtility.GetFlag(faction);
    }
    public void SetAmmoSlide()
    {
        ammoSlider.value = (float)artilleryBatteryRef.Ammo / (float)artilleryBatteryRef.artilleryBatteryTemplate.MaxAmmo;
    }
    public void SetUnit(string unit)
    {
        unitImage.sprite = GFXUtility.GetUnitSprite(unit);
    }
    public void SetBattalionTextName(int artilleryBatteryNumber, string name, UnitHardness hardness)
    {
        nameText.text = artilleryBatteryNumber.ToString() + " " + hardness.ToString() + " Art.Batt. " + " " + name;
    }

    public void HighLight(bool highlight)
    {
        if (highlight)
        {
            flagImage.color = Color.yellow;
        }
        else
        {
            flagImage.color = Color.white;
        }
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        bg.color = new Color(1.2f, 1.2f, 1.2f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bg.color = new Color(1f, 1f, 1f, 1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CameraManager cameraManagerRef = Utility.Camera.GetComponent<CameraManager>();
        //SELECT COMPANY
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!cameraManagerRef.selectedArtilleryBatteries.Contains(artilleryBatteryRef))
            {
                cameraManagerRef.SelectArtilleryBattery(artilleryBatteryRef);
            }
            else
            {
                cameraManagerRef.DeselectArtilleryBattery(artilleryBatteryRef);
            }
        }
        else
        {
            cameraManagerRef.DeselectAllArtilleryBatteries();
            cameraManagerRef.SelectArtilleryBattery(artilleryBatteryRef);
        }
    }
}
