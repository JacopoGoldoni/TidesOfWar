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

    public void Initialize(OfficerManager om)
    {
        Sprite flagSprite = GFXUtility.GetFlag(om.faction);
        Sprite unitIconSprite = GFXUtility.GetUnitSprite(om.companyTemplate.CompanyIcon);

        flagImage.sprite = flagSprite;
        unitIconImage.sprite = unitIconSprite;

        companyRef = om;
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
    public void OnFlagMiddleClicked()
    {
        Utility.Camera.GetComponent<CameraManager>().SendAttackOrder(companyRef.gameObject);
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
