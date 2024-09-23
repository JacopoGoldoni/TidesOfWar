using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class CommandButtonManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Components")]
    public Image CommandImage;
    public Image BackgroundImage;
    private int orderType;
    Action<OfficerManager[]> commandOfficerAction;
    Action<CaptainManager[]> commandCaptainAction;
    Action<ArtilleryOfficerManager[]> commandArtilleryBatteryAction;

    private bool Active = true;

    public void Initialize(int commandID, int backgroundID, Action<OfficerManager[]> action)
    {
        Sprite commandSprite = null;

        orderType = 0;

        switch(orderType)
        {
            case 0:
                commandSprite = GFXUtility.GetSpriteSheet("CompanyOrders")[commandID];
                break;
            case 1:
                commandSprite = GFXUtility.GetSpriteSheet("BattalionOrders")[commandID];
                break;
            case 2:
                commandSprite = GFXUtility.GetSpriteSheet("ArtilleryBatteryOrders")[commandID];
                break;
        }

        CommandImage.sprite = commandSprite;
        BackgroundImage.sprite = GFXUtility.GetSpriteSheet("OrderTab")[backgroundID];

        commandOfficerAction = action;
    }
    public void Initialize(int commandID, int backgroundID, Action<CaptainManager[]> action)
    {
        Sprite commandSprite = null;

        orderType = 1;

        switch (orderType)
        {
            case 0:
                commandSprite = GFXUtility.GetSpriteSheet("CompanyOrders")[commandID];
                break;
            case 1:
                commandSprite = GFXUtility.GetSpriteSheet("BattalionOrders")[commandID];
                break;
            case 2:
                commandSprite = GFXUtility.GetSpriteSheet("ArtilleryBatteryOrders")[commandID];
                break;
        }

        CommandImage.sprite = commandSprite;
        BackgroundImage.sprite = GFXUtility.GetSpriteSheet("OrderTab")[backgroundID];

        commandCaptainAction = action;
    }
    public void Initialize(int commandID, int backgroundID, Action<ArtilleryOfficerManager[]> action)
    {
        Sprite commandSprite = null;

        orderType = 2;

        switch (orderType)
        {
            case 0:
                commandSprite = GFXUtility.GetSpriteSheet("CompanyOrders")[commandID];
                break;
            case 1:
                commandSprite = GFXUtility.GetSpriteSheet("BattalionOrders")[commandID];
                break;
            case 2:
                commandSprite = GFXUtility.GetSpriteSheet("ArtilleryBatteryOrders")[commandID];
                break;
        }

        CommandImage.sprite = commandSprite;
        BackgroundImage.sprite = GFXUtility.GetSpriteSheet("OrderTab")[backgroundID];

        commandArtilleryBatteryAction = action;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(Active)
        {
            CameraManager cameraManager = Utility.Camera.GetComponent<CameraManager>();
            switch(orderType)
            {
                case 0:
                    commandOfficerAction.Invoke(cameraManager.selectedCompanies.ToArray());
                    break;
                case 1:
                    commandCaptainAction.Invoke(cameraManager.selectedBattalions.ToArray());
                    break;
                case 2:
                    commandArtilleryBatteryAction.Invoke(cameraManager.selectedArtilleryBatteries.ToArray());
                    break;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Active)
        {
            CommandImage.color = new Color(0.8f, 0.8f, 0.8f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Active)
        {
            CommandImage.color = new Color(1f, 1f, 1f);
        }
    }

    public void SetActive(bool active)
    {
        if(Active != active)
        {
            Active = active;

            if(Active)
            {
                CommandImage.color = new Color(1f, 1f, 1f);
            }
            else
            {
                CommandImage.color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
    }
}