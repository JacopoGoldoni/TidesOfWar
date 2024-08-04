using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArtilleryBulletManager : MonoBehaviour
{
    CountdownTimer lifeTimer;

    public void Initialize(float t)
    {
        lifeTimer = new CountdownTimer(t);
        lifeTimer.OnTimerStop = () => { Destroy(gameObject); };
        lifeTimer.Start();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Unit")
        {
            other.GetComponent<PawnManager>().Die();
        }
    }
}