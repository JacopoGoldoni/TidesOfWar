using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CompanyFlagManager : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public OfficerManager companyRef;
    public Image flagImage;
    public Image unitIconImage;
    public Image unitFireStatusImage;

    public void Initialize(OfficerManager om)
    {
        Sprite flagSprite = GFXUtility.GetFlag(om.TAG);
        Sprite unitIconSprite = GFXUtility.GetUnitSprite(om.companyTemplate.CompanyIcon);

        flagImage.sprite = flagSprite;
        unitIconImage.sprite = unitIconSprite;

        companyRef = om;

        SetFireStatusImage();
    }

    public void SetFireStatusImage()
    {
        if(companyRef.FireAll)
        {
            //FIRE ALL
            unitFireStatusImage.sprite = GFXUtility.GetSpriteSheet("CompanyOrders")[3];
        }
        else
        {
            //HOLD FIRE
            unitFireStatusImage.sprite = GFXUtility.GetSpriteSheet("CompanyOrders")[4];
        }
    }

    private void CompanyFlagAction()
    {
        CameraManager cameraManagerRef = Utility.Camera.GetComponent<CameraManager>();
        //SELECT COMPANY
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!cameraManagerRef.selectedCompanies.Contains(companyRef))
            {
                cameraManagerRef.SelectCompany(companyRef);
            }
            else
            {
                cameraManagerRef.DeselectCompany(companyRef);
            }
        }
        else
        {
            cameraManagerRef.DeselectAllCompanies();
            cameraManagerRef.SelectCompany(companyRef);
        }
    }
    public void OnFlagMiddleClicked()
    {
        Utility.Camera.GetComponent<CameraManager>().SendAttackOrder(companyRef.gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                CompanyFlagAction();
                break;
            case PointerEventData.InputButton.Right:
                break;
            case PointerEventData.InputButton.Middle:
                OnFlagMiddleClicked();
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
