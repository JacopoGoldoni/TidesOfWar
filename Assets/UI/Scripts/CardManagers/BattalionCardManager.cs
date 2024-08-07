using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattalionCardManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Image bg;
    public Image flagImage;
    public Image unitImage;
    public TextMeshProUGUI nameText;

    public CaptainManager battallionRef;

    public void Initialize(CaptainManager battalion)
    {
        bg = GetComponent<Image>();
        battallionRef = battalion;

        SetFlag(battalion.TAG);
        SetUnit(battalion.battalionTemplate.BattalionIcon);
        SetBattalionTextName(battalion.battalionNumber ,battalion.battalionName);
    }

    public void SetFlag(string TAG)
    {
        flagImage.sprite = GFXUtility.GetFlag(TAG);
    }
    public void SetUnit(string unit)
    {
        unitImage.sprite = GFXUtility.GetUnitSprite(unit);
    }
    public void SetBattalionTextName(int battalioNumber, string name)
    {
        nameText.text = battalioNumber + " Batt. " + name;
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
        bg.color = new Color(1.2f, 1.2f, 1.2f , 1f);
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
            if (!cameraManagerRef.selectedBattalions.Contains(battallionRef))
            {
                cameraManagerRef.SelectBattalion(battallionRef);
            }
            else
            {
                cameraManagerRef.DeselectBattalion(battallionRef);
            }
        }
        else
        {
            cameraManagerRef.DeselectAllBattalions();
            cameraManagerRef.SelectBattalion(battallionRef);
        }
    }
}
