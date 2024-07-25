using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CommandButtonManager : MonoBehaviour
{
    [Header("Components")]
    public Image CommandImage;
    public Image BackgroundImage;
    public Button CommandButton;

    public void Initialize(int commandID, bool type, int backgroundID, Action action)
    {
        Sprite commandSprite;

        if(type)
        {
            //BATTALION
            commandSprite = GFXUtility.GetSpriteSheet("BattalionOrders")[commandID];
        }
        else
        {
            //COMPANY
            commandSprite = GFXUtility.GetSpriteSheet("CompanyOrders")[commandID];
        }

        CommandImage.sprite = commandSprite;
        BackgroundImage.sprite = GFXUtility.GetSpriteSheet("OrderTab")[backgroundID];

        CommandButton.onClick.AddListener(action.Invoke);
    }
}