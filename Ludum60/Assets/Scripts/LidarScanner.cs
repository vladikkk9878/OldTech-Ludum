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

    public float scanDistance = 100f;
    public float particleSize = 0.02f;
    public LayerMask layersToIgnore;

    public int leftClickRays = 150;
    public float leftClickSpread = 10f;

    public int rightClickRays = 300;
    public float rightClickSpread = 50f;

    private ParticleSystem.Particle[] particleBuffer;
    private int currentRingIndex = 0;
    private bool bufferInitialized = false;
    private AudioSource ambientSource;
    private AudioClip lastPlayingClip;

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
        AudioClip soundToPlay = null;

        if (leftPressed)
        {
            Scan(leftClickRays, leftClickSpread);
            soundToPlay = leftClickSound;
        }
        else if (rightPressed)
        {
            Scan(rightClickRays, rightClickSpread);
            soundToPlay = rightClickSound;
        }

        if (soundToPlay != null)
        {
            if (audioSource.clip != soundToPlay || !audioSource.isPlaying)
            {
                audioSource.clip = soundToPlay;
                audioSource.loop = true; 
                audioSource.pitch = 1f;
                audioSource.Play();
            }
            lastPlayingClip = soundToPlay;
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
                audioSource.clip = null;
                lastPlayingClip = null;
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
