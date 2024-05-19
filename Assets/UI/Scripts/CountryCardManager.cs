using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountryCardManager : MonoBehaviour
{
    Country cardCountry;

    public SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Initialize()
    {
        spriteRenderer.sprite = cardCountry.GetFlag();
    }
}
