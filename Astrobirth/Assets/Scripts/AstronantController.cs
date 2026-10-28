using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AstronantController : MonoBehaviour
{
    [Header("움직임 세팅")]
    public float moveSpeed = 5f;
    public float jumpPower = 5f;
    public float gravity = -20f;

    [Header("시선&카메라 시점")]
    public float mouseSensitivity = 0.1f;
    public Transform cameraPivot;
    public Transform cameraTransform;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalVelocity;
    private float pitch;
    private bool isRunning;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    #region Input Callbacks (New Input System)
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && controller.isGrounded)
        {
            verticalVelocity = jumpPower;
        }
    }

    public void OnSprint(InputValue value)
    {
        isRunning = value.isPressed;
    }
    #endregion

    void Update()
    {
        transform.Rotate(0f, lookInput.x * mouseSensitivity, 0f);
        pitch = pitch - lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -20f, 60f);

        if (cameraPivot != null)
        {
            cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;
        float speed = moveSpeed;
        float targetZ = -6f;

        if (isRunning)
        {
            speed = moveSpeed * 2f;
            targetZ = -8f;
        }
        else
        {
            targetZ = -6f;
        }

            move = move * speed;
        move.y = verticalVelocity;

        if (cameraTransform != null)
        {
            Vector3 camPos = cameraTransform.localPosition;
            camPos.z = Mathf.Lerp(camPos.z, targetZ, 5f * Time.deltaTime);
            cameraTransform.localPosition = camPos;
        }

        controller.Move(move * Time.deltaTime);
    }
}