using MapMagic.Locks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldMapCameraMovement : MonoBehaviour
{
    //INPUTS
    public PlayerInputActions playerControls;
    private InputAction move;
    private InputAction scroll;
    Vector2 moveDirection = Vector2.zero;
    float scrollDirection = 0f;

    public float speed = 4f;
    public float ElevationSpeed = 40f;

    public float minHeight = 2f;
    public float maxHeight = 40f;

    Camera camera;

    float y;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        scroll = playerControls.UI.ScrollWheel;
        scroll.Enable();
    }

    private void OnDisable()
    {
        move.Disable();

        scroll.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Elevation();
    }

    private void Movement()
    {
        moveDirection = move.ReadValue<Vector2>();

        Vector2 cameraDisplacement = new Vector2(0, 0);

        cameraDisplacement = moveDirection.normalized * speed * Time.deltaTime;

        transform.position += transform.up * cameraDisplacement.y + transform.right * cameraDisplacement.x;
    }
    private void Elevation()
    {
        scrollDirection = scroll.ReadValue<float>();

        y += scrollDirection * ElevationSpeed * Time.deltaTime;
        y = Mathf.Clamp(y, minHeight, maxHeight);

        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
