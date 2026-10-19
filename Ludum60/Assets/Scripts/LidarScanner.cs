using UnityEngine;
using UnityEngine.InputSystem;

public class LidarScanner : MonoBehaviour
{
    public ParticleSystem lidarParticleSystem;
    public AudioSource audioSource;
    private Camera mainCamera;

    public AudioClip leftClickSound;
    public AudioClip rightClickSound;
    public AudioClip ambientMusic;
    public AudioClip errorSound;
    public AudioClip leftErrorSound;

    public float scanDistance = 100f;
    public float particleSize = 0.02f;
    public LayerMask layersToIgnore;
    public LayerMask greenLayer;
    public LayerMask redLayer;
    public LayerMask orangeLayer;

    public int leftClickRays = 150;
    public float leftClickSpread = 10f;

    public int rightClickRays = 300;
    public float rightClickSpread = 50f;

    private ParticleSystem.Particle[] particleBuffer;
    private int currentRingIndex = 0;
    private bool bufferInitialized = false;
    private AudioSource ambientSource;

    private bool isLeftPlaying = false;
    private float leftScanTimer = 0f;
    private bool leftNeedsReclick = false;

    private bool isRightActive = false;
    private float rightScanTimer = 0f;
    private float rightCooldownTimer = 0f;
    private bool isRightInCooldown = false;
    private bool wasRightPressedLastFrame = false;

    void Start()
    {
        mainCamera = Camera.main;

        if (audioSource != null && ambientMusic != null)
        {
            ambientSource = gameObject.AddComponent<AudioSource>();
            ambientSource.clip = ambientMusic;
            ambientSource.loop = true;
            ambientSource.volume = 0.5f;
            ambientSource.Play();
        }
    }

    void Update()
    {
        if (Mouse.current == null || mainCamera == null || lidarParticleSystem == null || audioSource == null) return;

        bool leftPressed = Mouse.current.leftButton.isPressed;
        bool rightPressed = Mouse.current.rightButton.isPressed;

        if (isRightInCooldown)
        {
            rightCooldownTimer -= Time.deltaTime;
            if (rightCooldownTimer <= 0f)
            {
                isRightInCooldown = false;
            }
        }

        if (rightPressed && !wasRightPressedLastFrame)
        {
            if (isRightInCooldown || isRightActive)
            {
                audioSource.PlayOneShot(errorSound);
            }
            else if (!leftPressed)
            {
                isRightActive = true;
                rightScanTimer = 0f;
                
                if (isLeftPlaying)
                {
                    audioSource.Stop();
                    isLeftPlaying = false;
                }

                audioSource.clip = rightClickSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        wasRightPressedLastFrame = rightPressed;

        if (isRightActive)
        {
            rightScanTimer += Time.deltaTime;
            if (rightScanTimer >= 5f)
            {
                audioSource.Stop();
                isRightActive = false;
                isRightInCooldown = true;
                rightCooldownTimer = 10f;
                rightScanTimer = 0f;
            }
            else
            {
                Scan(rightClickRays, rightClickSpread);
            }
        }

        if (!isRightActive)
        {
            if (!leftPressed)
            {
                leftNeedsReclick = false;
            }

            if (leftPressed && !leftNeedsReclick)
            {
                leftScanTimer += Time.deltaTime;
                if (leftScanTimer >= 5f)
                {
                    if (isLeftPlaying)
                    {
                        audioSource.Stop();
                        isLeftPlaying = false;
                    }
                    if (leftErrorSound != null)
                    {
                        audioSource.PlayOneShot(leftErrorSound);
                    }
                    leftNeedsReclick = true;
                    leftScanTimer = 0f;
                }
                else
                {
                    Scan(leftClickRays, leftClickSpread);

                    if (!isLeftPlaying)
                    {
                        audioSource.clip = leftClickSound;
                        audioSource.loop = true;
                        audioSource.Play();
                        isLeftPlaying = true;
                    }
                }
            }
            else
            {
                if (isLeftPlaying)
                {
                    audioSource.Stop();
                    isLeftPlaying = false;
                    leftScanTimer = 0f;
                }
            }
        }
    }

    void Scan(int raysCount, float currentSpread)
    {
        int maxParticles = lidarParticleSystem.main.maxParticles;

        if (!bufferInitialized || particleBuffer == null || particleBuffer.Length != maxParticles)
        {
            particleBuffer = new ParticleSystem.Particle[maxParticles];
            bufferInitialized = true;
        }

        int activeCount = lidarParticleSystem.GetParticles(particleBuffer);

        for (int i = 0; i < raysCount; i++)
        {
            Vector3 rayDirection = Quaternion.Euler(
                Random.Range(-currentSpread, currentSpread),
                Random.Range(-currentSpread, currentSpread),
                0
            ) * transform.forward;

            Ray ray = new Ray(transform.position, rayDirection);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, scanDistance, ~layersToIgnore))
            {
                int hitLayerMask = 1 << hit.collider.gameObject.layer;
                Color targetColor = Color.white;

                if ((hitLayerMask & greenLayer) != 0)
                {
                    targetColor = Color.green;
                }
                else if ((hitLayerMask & redLayer) != 0)
                {
                    targetColor = Color.red;
                }
                else if ((hitLayerMask & orangeLayer) != 0)
                {
                    targetColor = new Color(1f, 0.5f, 0f);
                }

                if (activeCount < maxParticles)
                {
                    ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
                    emitParams.position = hit.point;
                    emitParams.velocity = Vector3.zero;
                    emitParams.startColor = targetColor;
                    emitParams.startSize = particleSize;

                    lidarParticleSystem.Emit(emitParams, 1);
                    activeCount++;
                }
                else
                {
                    particleBuffer[currentRingIndex].position = hit.point;
                    particleBuffer[currentRingIndex].velocity = Vector3.zero;
                    particleBuffer[currentRingIndex].startColor = targetColor;
                    particleBuffer[currentRingIndex].startSize = particleSize;
                    particleBuffer[currentRingIndex].remainingLifetime = lidarParticleSystem.main.startLifetime.constant;

                    currentRingIndex = (currentRingIndex + 1) % maxParticles;
                }
            }
        }

        if (activeCount >= maxParticles)
        {
            lidarParticleSystem.SetParticles(particleBuffer, activeCount);
        }
    }
}
