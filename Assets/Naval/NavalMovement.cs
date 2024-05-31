using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavalMovement : MonoBehaviour
{
    float speed;
    public float MaxSpeed;

    //0,1,2,3
    public int speedMode;

    public float windFactor;
    public float engineFactor;

    private float CalculateWind(Vector2 wind)
    {
        float w = Vector2.Dot(wind, Utility.V3toV2( transform.forward ));
        w = Mathf.Clamp01(w);

        return w;
    }

    private void UpdateMovement()
    {
        float acc = speedMode * (MaxSpeed - speed) * windFactor * CalculateWind(new Vector2(0f, 0f));

        speed += acc * Time.deltaTime;

        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void Update()
    {
        UpdateMovement();
    }
}