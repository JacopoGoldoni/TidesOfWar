using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NavalBatleCameraManager : MonoBehaviour
{
    public string TAG = "FRA";

    List<NavalManager> selectedShips = new List<NavalManager>();

    NavalUIManager navalUIManager;

    private void Awake()
    {
        navalUIManager = GetComponent<NavalUIManager>();

        navalUIManager.ClearAllCards();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            //TRACE FOR SHIP
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                NavalManager unitManager = hit.transform.gameObject.GetComponent<NavalManager>();
                if(unitManager != null && unitManager.TAG == TAG)
                {
                    if(Input.GetKey(KeyCode.LeftShift))
                    {
                        selectedShips.Add(unitManager);

                        navalUIManager.AddCard(unitManager);
                    }
                    else
                    {
                        selectedShips.Clear();
                        selectedShips.Add(unitManager);

                        navalUIManager.ClearAllCards();
                        navalUIManager.AddCard(unitManager);
                    }
                }
                else
                {
                    if(!Input.GetKey(KeyCode.LeftShift))
                    {
                        selectedShips.Clear();
                        navalUIManager.ClearAllCards();
                    }
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            //TRACE FOR SHIP
            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
            {
                //Move to order
                foreach (NavalManager nm in selectedShips)
                {
                    nm.target = Utility.V3toV2(hit.point);
                    nm.move = true;
                }
            }
        }
    }
}