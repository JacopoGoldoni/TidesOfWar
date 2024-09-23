  using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingGridSlotManager : MonoBehaviour, IPointerEnterHandler
{
    public int building_ID;

    public Image buildingSlotImage;
    public Button buildingSlotButton;

    public WorldUIManager worldUIManager;

    public void Initialize()
    {
        if(buildingSlotImage)
        {
            string spritePath = WorldUtility.GetBuildingByID(building_ID).sprite;
            buildingSlotImage.sprite = GFXUtility.GetBuildingSprite(spritePath);
        }

        if(buildingSlotButton)
        {
            buildingSlotButton.onClick.AddListener( () => {  });
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        Debug.Log("Hovering");
        worldUIManager.ShowBuildingInfos(building_ID);
    }
}