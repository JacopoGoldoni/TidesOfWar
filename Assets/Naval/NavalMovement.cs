using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavalMovement : MonoBehaviour
{
    float mass;
    Vector2 velocity;
    float max_force;
    float max_speed;

    private void Update()
    {
        UpdateMovement();
    }
    private void UpdateMovement()
    {
        transform.position += Utility.V2toV3(velocity) * Time.deltaTime;
    }
    private void ApplySteering(Vector2 steering)
    {
        velocity += steering * Time.deltaTime;

        transform.rotation = Quaternion.LookRotation(velocity.normalized ,Vector3.up);
    }

    //MOVEMENT ORDER
    public void Seek(Vector2 target)
    {
        Vector2 desired_velocity = (target - Utility.V3toV2(transform.position)).normalized * max_speed;

        Vector2 steering = desired_velocity - velocity;

        ApplySteering(steering);
    }
    public void Flee(Vector2 target)
    {
        Vector2 desired_velocity = (Utility.V3toV2(transform.position) - target).normalized * max_speed;

        Vector2 steering = desired_velocity - velocity;

        ApplySteering(steering);
    }
}