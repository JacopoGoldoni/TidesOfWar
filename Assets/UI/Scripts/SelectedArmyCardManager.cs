using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedArmyCardManager : MonoBehaviour
{
    public int ArmyID;

    Text cardText;
    Image cardImage;

    private void Awake()
    {
        BuildCard();
    }

    private void BuildCard()
    {
        GameObject cardImageOBJ = new GameObject("cardImage");
        cardImageOBJ.transform.parent = transform;
        cardImage = cardImageOBJ.AddComponent<Image>();

        GameObject cardTextOBJ = new GameObject("cardText");
        cardTextOBJ.transform.parent = transform;
        cardText = cardTextOBJ.AddComponent<Text>();
    }

    public void PopulateCard(string armyName, Sprite armyFlag)
    {
        cardText.text = armyName;
        cardImage.sprite = armyFlag;
    }
}