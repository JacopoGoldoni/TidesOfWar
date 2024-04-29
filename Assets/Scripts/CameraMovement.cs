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
    public float horizontalRotationSpeed = 2f;
    public float verticalRotationSpeed = 1f;
    private bool rotate = false;

    Camera camera;

    float y = 4.16f;

    OfficerManager selectedOfficial;

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

        Vector2 cameraDisplacement = new Vector2(0, 0);

        cameraDisplacement = moveDirection.normalized * speed * Time.deltaTime;

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

        y += scrollDirection * 2f * Time.deltaTime;
        y = Mathf.Clamp(y, 2, 40);

        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}