using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CompanyCardManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Image bg;
    public Image flagImage;
    public Image unitImage;
    public TextMeshProUGUI nameText;
    public Slider ammoSlider;

    public OfficerManager companyRef;

    public void Initialize(OfficerManager company)
    {
        bg = GetComponent<Image>();
        companyRef = company;
        companyRef.companyCardRef = this;

        SetFlag(company.faction);
        SetUnit(company.companyTemplate.CompanyIcon);
        SetBattalionTextName(company.companyNumber, company.companyName, company.companyTemplate.hardness);
        SetAmmoSlide();
    }

    public void SetFlag(Factions faction)
    {
        flagImage.sprite = GFXUtility.GetFlag(faction);
    }
    public void SetAmmoSlide()
    {
        ammoSlider.value = (float)companyRef.Ammo / (float)companyRef.companyTemplate.MaxAmmo;
    }

    public void SetUnit(string unit)
    {
        unitImage.sprite = GFXUtility.GetUnitSprite(unit);
    }
    public void SetBattalionTextName(int battalioNumber, string name, UnitHardness hardness)
    {
        nameText.text = battalioNumber.ToString() + " " + hardness.ToString() + " Comp. " + " " + name;
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
}
