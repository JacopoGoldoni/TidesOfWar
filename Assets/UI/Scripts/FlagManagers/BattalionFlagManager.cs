using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattalionFlagManager : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler 
{
    public CaptainManager battalionRef;
    public Image flagImage;
    public Image unitIconImage;

    public void Initialize(CaptainManager cm)
    {
        Sprite flagSprite = GFXUtility.GetFlag(cm.faction);
        Sprite unitIconSprite = GFXUtility.GetUnitSprite(cm.battalionTemplate.BattalionIcon);

        flagImage.sprite = flagSprite;
        unitIconImage.sprite = unitIconSprite;

        battalionRef = cm;
    }

    private void BattalionFlagAction()
    {
        CameraManager cameraManagerRef = Utility.Camera.GetComponent<CameraManager>();
        //SELECT COMPANY
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!cameraManagerRef.selectedBattalions.Contains(battalionRef))
            {
                cameraManagerRef.SelectBattalion(battalionRef);
            }
            else
            {
                cameraManagerRef.DeselectBattalion(battalionRef);
            }
        }
        else
        {
            cameraManagerRef.DeselectAllBattalions();
            cameraManagerRef.SelectBattalion(battalionRef);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                BattalionFlagAction();
                break;
            case PointerEventData.InputButton.Right:
                break;
            case PointerEventData.InputButton.Middle:
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        flagImage.color = new Color(1f, 1f, 1f, 0.8f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        flagImage.color = new Color(1f, 1f, 1f, 0.5f);
    }
}
