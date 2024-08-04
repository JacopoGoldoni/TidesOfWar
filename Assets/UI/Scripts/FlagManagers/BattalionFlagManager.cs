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

    private void BattalionSelection()
    {
        CameraManager cameraManagerRef = Utility.Camera.GetComponent<CameraManager>();
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
    private void BattallionAttach()
    {
        CameraManager cameraManagerRef = Utility.Camera.GetComponent<CameraManager>();

        if(cameraManagerRef.selectedCompanies.Count != 0)
        {
            foreach(OfficerManager om in cameraManagerRef.selectedCompanies)
            {
                om.Attach(battalionRef);
            }
            battalionRef.battalionFormation.CalculateAllPositions();
            battalionRef.ReceiveMovementOrder(false, Utility.V3toV2(battalionRef.transform.position), battalionRef.transform.rotation);
        }

        cameraManagerRef.uimanager.UpdateCompanyCommandStatus();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                BattalionSelection();
                break;
            case PointerEventData.InputButton.Right:
                BattallionAttach();
                break;
            case PointerEventData.InputButton.Middle:
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }
    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
