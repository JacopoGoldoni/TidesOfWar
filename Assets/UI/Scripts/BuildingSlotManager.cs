using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSlotManager : MonoBehaviour
{
    public WorldUIManager worldUIManagerRef;
    private Button buildingSlotButton;

    private void Awake()
    {
        worldUIManagerRef = FindFirstObjectByType<WorldUIManager>();

        buildingSlotButton = GetComponentInChildren<Button>();

        buildingSlotButton.onClick.AddListener(
            () => { worldUIManagerRef.OpenBuildingTab(); }
            );
    }
}
