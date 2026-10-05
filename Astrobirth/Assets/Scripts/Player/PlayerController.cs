using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    private Vector2 moveInput;
    private CharacterController controller;

    public float jumpPower = 5.0f;
    public float gravity = -20.0f;

    private Vector2 lookInput;
    private float mouseSensitivity = 0.2f;

    private float verticalVelocity;

    public Transform cameraPivot;
    public Transform cameraTransform;


    private float pitch = 20f;

    private bool isRunning;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        moveInput *= moveSpeed;
    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed && controller.isGrounded)
        {
            verticalVelocity = jumpPower;
        }
    }
    private void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    private void OnSprint(InputValue value)
    {
        isRunning = value.isPressed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, lookInput.x * mouseSensitivity, 0f);



        if (controller.isGrounded && verticalVelocity < 0.0f)
        {
            verticalVelocity = -2.0f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;

        //Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        float speed = moveSpeed;
        float targetZ = -3f;
        if (isRunning)
        {
            speed = moveSpeed * 2f;
            targetZ = -5f;
        }
        move = move * speed;

        Vector3 camPos = cameraTransform.localPosition;
        camPos.z = Mathf.Lerp(camPos.z, targetZ, 5f * Time.deltaTime);
        cameraTransform.localPosition = camPos;


        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        pitch = pitch - lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -20f, 60f);
        cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }
}