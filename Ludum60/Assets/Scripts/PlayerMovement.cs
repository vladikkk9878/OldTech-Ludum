using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 0.1f;
    public Transform cameraTransform;

    [Header("Покачивание камеры (Head Bobbing)")]
    public float bobbingSpeed = 14f;
    public float bobbingAmount = 0.05f;

    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float cameraPitch = 0f;
    
    private float defaultY = 0f;
    private float timer = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        if (cameraTransform != null)
        {
            defaultY = cameraTransform.localPosition.y;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            float moveX = 0f;
            float moveZ = 0f;

            if (Keyboard.current.wKey.isPressed) moveZ += 1f;
            if (Keyboard.current.sKey.isPressed) moveZ -= 1f;
            if (Keyboard.current.aKey.isPressed) moveX -= 1f;
            if (Keyboard.current.dKey.isPressed) moveX += 1f;

            moveInput = new Vector2(moveX, moveZ).normalized;
        }

        if (Mouse.current != null)
        {
            lookInput = Mouse.current.delta.ReadValue();
        }

        RotatePlayer();
        MovePlayer();
        HandleHeadBob();
    }

    void RotatePlayer()
    {
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);

        cameraPitch -= lookInput.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    void MovePlayer()
    {
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        
        Vector3 finalVelocity = moveDirection * moveSpeed;
        finalVelocity.y = Physics.gravity.y;

        characterController.Move(finalVelocity * Time.deltaTime);
    }

    void HandleHeadBob()
    {
        if (cameraTransform == null || characterController == null) return;

        bool isMoving = moveInput.magnitude > 0.1f && characterController.isGrounded;

        if (isMoving)
        {
            timer += Time.deltaTime * bobbingSpeed;
            
            float newY = defaultY + Mathf.Sin(timer) * bobbingAmount;
            float newX = Mathf.Cos(timer / 2) * bobbingAmount * 0.5f;

            cameraTransform.localPosition = new Vector3(newX, newY, cameraTransform.localPosition.z);
        }
        else
        {
            timer = 0f;
            Vector3 targetPosition = new Vector3(0f, defaultY, cameraTransform.localPosition.z);
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPosition, Time.deltaTime * 8f);
        }
    }
}
