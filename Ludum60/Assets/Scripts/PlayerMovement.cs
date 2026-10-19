using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6.7f;
    public float mouseSensitivity = 0.1f;
    public Transform cameraTransform;

    public float bobbingSpeed = 14f;
    public float bobbingAmount = 0.05f;

    public float idleBobSpeed = 2f;
    public float idleBobAmount = 0.015f;

    public AudioSource movementAudioSource;
    public AudioSource breathAudioSource;
    public AudioClip footstepSound;
    public AudioClip sighSound;

    [Header("Monster Proximity Settings")]
    public Transform monsterTransform;
    public float maxDistanceThreshold = 50f;
    public float maxMoveSpeed = 13.4f;

    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float cameraPitch = 0f;
    
    private float defaultY = 0f;
    private float walkTimer = 0f;
    private float idleTimer = 0f;
    private float breathTimer = 0f;

    private float baseMoveSpeed;
    private float baseBobbingSpeed;

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

        if (movementAudioSource != null && footstepSound != null)
        {
            movementAudioSource.clip = footstepSound;
            movementAudioSource.loop = true;
        }

        breathTimer = 0f;
        baseMoveSpeed = moveSpeed;
        baseBobbingSpeed = bobbingSpeed;

        if (monsterTransform == null)
        {
            MonsterAI monsterScript = FindFirstObjectByType<MonsterAI>();
            if (monsterScript != null)
            {
                monsterTransform = monsterScript.transform;
            }
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

        HandleProximityScaling();
        RotatePlayer();
        MovePlayer();
        HandleCameraEffects();
        HandleAudio();
    }

    void HandleProximityScaling()
    {
        if (monsterTransform == null || !monsterTransform.gameObject.activeInHierarchy)
        {
            moveSpeed = baseMoveSpeed;
            bobbingSpeed = baseBobbingSpeed;
            return;
        }

        float distance = Vector3.Distance(transform.position, monsterTransform.position);

        if (distance < maxDistanceThreshold)
        {
            float t = 1f - Mathf.Clamp01(distance / maxDistanceThreshold);
            
            moveSpeed = Mathf.Lerp(baseMoveSpeed, maxMoveSpeed, t);
            bobbingSpeed = Mathf.Lerp(baseBobbingSpeed, baseBobbingSpeed * (maxMoveSpeed / baseMoveSpeed), t);
        }
        else
        {
            moveSpeed = baseMoveSpeed;
            bobbingSpeed = baseBobbingSpeed;
        }
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

    void HandleCameraEffects()
    {
        if (cameraTransform == null || characterController == null) return;

        bool isMoving = moveInput.magnitude > 0.1f && characterController.isGrounded;

        if (isMoving)
        {
            idleTimer = 0f;
            walkTimer += Time.deltaTime * bobbingSpeed;
            
            float newY = defaultY + Mathf.Sin(walkTimer) * bobbingAmount;
            float newX = Mathf.Cos(walkTimer / 2) * bobbingAmount * 0.5f;

            cameraTransform.localPosition = new Vector3(newX, newY, cameraTransform.localPosition.z);
        }
        else
        {
            walkTimer = 0f;
            idleTimer += Time.deltaTime * idleBobSpeed;

            float newY = defaultY + Mathf.Sin(idleTimer) * idleBobAmount;
            float newX = Mathf.Cos(idleTimer * 0.5f) * (idleBobAmount * 0.3f);

            cameraTransform.localPosition = new Vector3(newX, newY, cameraTransform.localPosition.z);
        }
    }

    void HandleAudio()
    {
        bool isMoving = moveInput.magnitude > 0.1f && characterController != null && characterController.isGrounded;

        if (isMoving && movementAudioSource != null && footstepSound != null)
        {
            if (!movementAudioSource.isPlaying)
            {
                movementAudioSource.Play();
            }

            movementAudioSource.pitch = moveSpeed / baseMoveSpeed;
        }
        else
        {
            if (movementAudioSource != null && movementAudioSource.isPlaying)
            {
                movementAudioSource.Stop();
            }
        }

        breathTimer += Time.deltaTime;
        if (breathTimer >= 60f)
        {
            if (breathAudioSource != null && sighSound != null)
            {
                breathAudioSource.PlayOneShot(sighSound);
            }
            breathTimer = 0f;
        }
    }
}
