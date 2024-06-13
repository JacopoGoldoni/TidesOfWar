using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LawSlotManager : MonoBehaviour
{
    public Text text;
    public Image image;
 
    public string lawCategory;

    public void SetByLaw(Law law)
    {
        text.text = law.name;
        image.sprite = law.lawSprite;
    }
}