using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldMapCameraManager : MonoBehaviour
{
    public InputAction playerControls;

    public List<ArmyManager> selectedArmies = new List<ArmyManager>();

    public Factions faction = Factions.France;

    //COMPONENTS
    Camera camera;
    WorldUIManager worldUIManager;


    int UILayer;

    void Start()
    {
        UILayer = LayerMask.NameToLayer("UI");

        camera = Utility.Camera;
        worldUIManager = GetComponent<WorldUIManager>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                SelectArmy(hit.transform);
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            SendMovementOrder(TraceForDestination());
        }
    }



    public void SelectArmy(Transform target)
    {
        ArmyManager a = target.GetComponent<ArmyManager>();

        if(a == null)
        {
            selectedArmies.Clear();
            worldUIManager.ClearArmyCard();
            return;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            if(selectedArmies.Contains(a))
            {
                selectedArmies.Remove(a);
                worldUIManager.RemoveArmyCard(a.ID);
            }
            else
            {
                selectedArmies.Add(a);
                worldUIManager.AddArmyCard(a.ID);
            }
        }
        else
        {
            selectedArmies.Clear();
            worldUIManager.ClearArmyCard();

            selectedArmies.Add(a);
            worldUIManager.AddArmyCard(a.ID);
        }
    }
    private Vector2 TraceForDestination()
    {
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return Vector2.zero;
    }
    private void SendMovementOrder(Vector2 v2)
    {
        for(int i = 0; i < selectedArmies.Count; i++)
        {
            selectedArmies[i].MoveTo(v2);
        }
    }
}