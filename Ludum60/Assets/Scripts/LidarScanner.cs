using UnityEngine;
using UnityEngine.InputSystem;

public class LidarScanner : MonoBehaviour
{
    public ParticleSystem lidarParticleSystem;
    public AudioSource audioSource;
    private Camera mainCamera;

    public GameObject pointScanWindow;
    public GameObject wideScanWindow;

    public float scanDistance = 75f;
    public float particleSize = 0.01f;
    public LayerMask layersToIgnore;

    public int leftClickRays = 150;
    public float leftClickSpread = 10f;
    public AudioClip leftClickSound;

    public int rightClickRays = 300;
    public float rightClickSpread = 50f;
    public AudioClip rightClickSound;

    public float minPitch = 0.85f;
    public float maxPitch = 1.25f;

    private ParticleSystem.ColorOverLifetimeModule colorModule;
    private ParticleSystem.Particle[] particleBuffer;
    private int currentRingIndex = 0;
    private bool bufferInitialized = false;

    void Start()
    {
        mainCamera = Camera.main;

        if (lidarParticleSystem != null)
        {
            colorModule = lidarParticleSystem.colorOverLifetime;
            colorModule.enabled = true;

            Gradient fadeGradient = new Gradient();
            
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
            alphaKeys[1] = new GradientAlphaKey(0.0f, 1.0f);

            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0] = new GradientColorKey(Color.white, 0.0f);
            colorKeys[1] = new GradientColorKey(Color.white, 1.0f);

            fadeGradient.SetKeys(colorKeys, alphaKeys);
            colorModule.color = fadeGradient;
        }

        if (pointScanWindow != null) pointScanWindow.SetActive(false);
        if (wideScanWindow != null) wideScanWindow.SetActive(false);
    }

    void Update()
    {
        if (Mouse.current == null || mainCamera == null || lidarParticleSystem == null) return;

        bool leftPressed = Mouse.current.leftButton.isPressed;
        bool rightPressed = Mouse.current.rightButton.isPressed;
        AudioClip soundToPlay = null;

        if (leftPressed)
        {
            Scan(leftClickRays, leftClickSpread);
            soundToPlay = leftClickSound;

            if (pointScanWindow != null) pointScanWindow.SetActive(true);
            if (wideScanWindow != null) wideScanWindow.SetActive(false);
        }
        else if (rightPressed)
        {
            Scan(rightClickRays, rightClickSpread);
            soundToPlay = rightClickSound;

            if (pointScanWindow != null) pointScanWindow.SetActive(false);
            if (wideScanWindow != null) wideScanWindow.SetActive(true);
        }
        else
        {
            if (pointScanWindow != null) pointScanWindow.SetActive(false);
            if (wideScanWindow != null) wideScanWindow.SetActive(false);
        }

        if (soundToPlay != null && audioSource != null)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.PlayOneShot(soundToPlay);
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
                if (activeCount < maxParticles)
                {
                    ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
                    emitParams.position = hit.point;
                    emitParams.velocity = Vector3.zero;
                    emitParams.startColor = Color.white;
                    emitParams.startSize = particleSize;

                    lidarParticleSystem.Emit(emitParams, 1);
                    activeCount++;
                }
                else
                {
                    particleBuffer[currentRingIndex].position = hit.point;
                    particleBuffer[currentRingIndex].velocity = Vector3.zero;
                    particleBuffer[currentRingIndex].startColor = Color.white;
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
