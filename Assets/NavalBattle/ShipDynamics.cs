using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ShipDynamics : MonoBehaviour
{
    [Header("Dynamics")]
    public float mass;
    public float dragCoefficient;
    public float windFactor;
    public float minThrust;
    public float turningSpeed;
    public int sailLevel = 0;
    public bool anchored = true;


    [Header("Cinematics")]
    public float speed = 0;
    public float accelleration = 0;
    private float initialPhase;
    public float swingingSpeed = 30f;
    public float swingingAmplitude = 10f;

    [Header("Inputs")]
    public Vector2 thrust;
    private float rotation;
    public Vector2 wind;

    //TRANSFORM
    private Vector2 forward
    {
        get { return Utility.V3toV2(transform.forward).normalized; }
    }
    private Vector2 right
    {
        get { return Utility.V3toV2(transform.right).normalized; }
    }

    private void Awake()
    {
        initialPhase = UnityEngine.Random.Range(0, 360f);
    }

    private void FixedUpdate()
    {
        if(!anchored)
        {
            //GET ACCELLERATION
            accelleration = CalculateAccelleration();

            //DO CINEMATICS
            speed += accelleration * Time.deltaTime;
            transform.Translate(Utility.V2toV3(forward) * speed * Time.deltaTime, Space.World);
            transform.rotation = Quaternion.Slerp(
                RootRotation(),
                CalculateRotation() * RootRotation(),
                turningSpeed * Time.deltaTime
                );
        }

        Oscillate();
    }

    private float CalculateAccelleration()
    {
        float totalForce = DragForce()
            + Vector2.Dot(forward, thrust) * minThrust;

        return totalForce / mass;
    }
    private Quaternion CalculateRotation()
    {
        Quaternion newRotation = Quaternion.Euler(0, Vector2.Dot(right, thrust), 0);
        return newRotation;
    }

    private Quaternion RootRotation()
    {
        return Quaternion.LookRotation(Utility.V2toV3(forward), Vector3.up);
    }
    private void Oscillate()
    {
        float theta = Mathf.Sin(Time.time * swingingSpeed * Mathf.Deg2Rad + initialPhase) * swingingAmplitude;
        Vector3 up = Quaternion.AngleAxis(theta, transform.forward) * Vector3.up;
        transform.rotation = Quaternion.LookRotation(transform.forward, up);
    }

    //FORCES CALCULATION
    private float DragForce()
    {
        return -1 * dragCoefficient * speed;
    }
    private Vector2 WindForce()
    {
        float w = Vector2.Dot(wind, forward);
        w *= windFactor;

        return forward * w;
    }

    //EXTERNAL COMANDS
    public void Stop()
    {
        speed = 0f;
    }
}