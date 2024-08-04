using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArtilleryBatteryFlagManager : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ArtilleryOfficerManager artilleryBatteryRef;
    public Image flagImage;
    public Image unitIconImage;

    public void Initialize(ArtilleryOfficerManager aom)
    {
        Sprite flagSprite = GFXUtility.GetFlag(aom.faction);
        Sprite unitIconSprite = GFXUtility.GetUnitSprite(aom.artilleryBatteryTemplate.ArtilleryBatteryIcon);

        flagImage.sprite = flagSprite;
        unitIconImage.sprite = unitIconSprite;

        artilleryBatteryRef = aom;
    }

    private void ArtilleryBatteryFlagAction()
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
    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                ArtilleryBatteryFlagAction();
                break;
            case PointerEventData.InputButton.Right:
                break;
            case PointerEventData.InputButton.Middle:
                OnFlagMiddleClicked();
                break;
        }
    }
    public void OnFlagMiddleClicked()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }
    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
