using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    //INPUTS
    public PlayerInputActions playerControls;
    private InputAction move;
    private InputAction look;
    private InputAction scroll;
    private InputAction middle;
    Vector2 moveDirection = Vector2.zero;
    Vector2 lookDirection = Vector2.zero;
    float scrollDirection = 0f;

    public float speed = 2f;
    public float ElevationSpeed = 10f;
    public float horizontalRotationSpeed = 2f;
    public float verticalRotationSpeed = 1f;
    private bool rotate = false;

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

        look = playerControls.Player.Look;
        look.Enable();

        scroll = playerControls.UI.ScrollWheel;
        scroll.Enable();
    }

    private void OnDisable()
    {
        move.Disable();

        look.Disable();

        scroll.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        camera = Utility.Camera;
        y = camera.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Rotation();
        Elevation();
    }

    private void Movement()
    {
        moveDirection = move.ReadValue<Vector2>();

        Vector2 cameraDisplacement = 
            moveDirection.normalized * 
            speed * ( 0.1f + Mathf.Lerp(0f, 0.9f, (y - minHeight) / (maxHeight - minHeight)) ) * 
            Time.deltaTime;

        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;

        transform.position += forward * cameraDisplacement.y + transform.right * cameraDisplacement.x;
    }
    private void Rotation()
    {
        if (Input.GetKey(KeyCode.Mouse2))
        {
            lookDirection = look.ReadValue<Vector2>();

            transform.eulerAngles = new Vector3(transform.eulerAngles.x + lookDirection.y * -verticalRotationSpeed * Time.deltaTime, transform.eulerAngles.y + lookDirection.x * horizontalRotationSpeed * Time.deltaTime, 0);
        }
    }
    private void Elevation()
    {
        scrollDirection = scroll.ReadValue<float>();

        y += scrollDirection * ElevationSpeed * Time.deltaTime;
        y = Mathf.Clamp(y, minHeight, maxHeight);

        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}