using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardManager : MonoBehaviour
{
    public List<BillboardRenderer> billboards;

    public void UpdateBillboards()
    {
        foreach(BillboardRenderer billboardRenderer in billboards)
        {
            if(billboardRenderer.IsBillboardInView())
            {
                //UPDATE BILLBOARD POSITION
            }
        }
    }
}