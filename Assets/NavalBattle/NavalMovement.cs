using System;
using UnityEngine;

public class NavalMovement : MonoBehaviour
{
    //COMPONENTS
    NavalManager nm;
    ShipDynamics sd;

    public float acceptanceRadius = 0.2f;

    private void Awake()
    {
        nm = GetComponent<NavalManager>();
        sd = GetComponent<ShipDynamics>();
    }

    //MOVEMENT ORDER
    public void Stop()
    {
        sd.anchored = true;
        sd.Stop();
    }
    public void Seek(Vector2 target)
    {
        sd.anchored = false;

        if(Vector2.Distance(Utility.V3toV2(transform.position), target) <= acceptanceRadius)
        {
            Stop();
            return;
        }

        Vector2 steering = target - Utility.V3toV2(transform.position);
        steering.Normalize();

        sd.thrust = steering;
    }
    /*
    public void Flee(Vector2 target)
    {
        Vector2 desired_velocity = (Utility.V3toV2(transform.position) - target).normalized * max_speed;

        Vector2 steering = desired_velocity - velocity;

        ApplySteering(steering);
    }
    */
}