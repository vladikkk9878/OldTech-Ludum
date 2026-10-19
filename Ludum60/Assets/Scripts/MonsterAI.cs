using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MonsterAI : MonoBehaviour
{
    public Transform playerTransform;
    public float attackDistance = 2f;
    
    [Header("Audio Settings")]
    public AudioSource monsterAudioSource;
    
    public AudioClip[] ambientSounds;
    [Range(0f, 1f)] public float ambientVolume = 0.5f;

    public AudioClip jumpscareSound;
    [Range(0f, 1f)] public float jumpscareVolume = 1.0f;

    [Header("UI Jumpscare Settings")]
    public GameObject jumpscareUIObject;

    [Header("Camera Shake Settings")]
    public Transform cameraTransform;
    public float shakeDuration = 2.0f;
    public float shakeMagnitude = 0.15f;
    public float dampingSpeed = 1.0f;

    [Header("Fade Out Settings")]
    public Image fadeImage;
    public float fadeDuration = 1.5f;

    private NavMeshAgent agent;
    private bool isJumpscareActive = false;
    private Vector3 initialCameraPos;
    private float currentShakeDuration;
    private bool isFading = false;
    private float fadeTimeElapsed = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (jumpscareUIObject != null)
        {
            jumpscareUIObject.SetActive(false);
        }

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }

        PlayRandomAmbient();
    }

    void Update()
    {
        if (isJumpscareActive)
        {
            ApplyCameraShake();
            ApplyFadeOut();
            return;
        }

        if (playerTransform == null || agent == null) return;

        agent.SetDestination(playerTransform.position);

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= attackDistance)
        {
            TriggerJumpscare();
        }
    }

    void PlayRandomAmbient()
    {
        if (monsterAudioSource != null && ambientSounds != null && ambientSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, ambientSounds.Length);
            AudioClip chosenAmbient = ambientSounds[randomIndex];

            if (chosenAmbient != null)
            {
                monsterAudioSource.clip = chosenAmbient;
                monsterAudioSource.loop = true;
                monsterAudioSource.volume = ambientVolume;
                monsterAudioSource.Play();
            }
        }
    }

    void TriggerJumpscare()
    {
        isJumpscareActive = true;
        agent.isStopped = true;

        if (cameraTransform != null)
        {
            initialCameraPos = cameraTransform.localPosition;
            currentShakeDuration = shakeDuration;
        }

        if (jumpscareUIObject != null)
        {
            jumpscareUIObject.SetActive(true);
        }

        if (monsterAudioSource != null)
        {
            monsterAudioSource.Stop();
            
            if (jumpscareSound != null)
            {
                monsterAudioSource.PlayOneShot(jumpscareSound, jumpscareVolume);
            }
        }

        PlayerMovement playerMove = playerTransform.GetComponent<PlayerMovement>();
        if (playerMove != null) playerMove.enabled = false;

        LidarScanner scanner = playerTransform.GetComponentInChildren<LidarScanner>();
        if (scanner != null) scanner.enabled = false;

        Vector3 lookAtTarget = new Vector3(transform.position.x, playerTransform.position.y, transform.position.z);
        playerTransform.LookAt(lookAtTarget);

        Invoke("StartFade", 4f - fadeDuration);
    }

    void ApplyCameraShake()
    {
        if (cameraTransform == null) return;

        if (currentShakeDuration > 0)
        {
            cameraTransform.localPosition = initialCameraPos + Random.insideUnitSphere * shakeMagnitude;
            currentShakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            currentShakeDuration = 0f;
            cameraTransform.localPosition = initialCameraPos;
        }
    }

    void StartFade()
    {
        isFading = true;
        Invoke("LoadGameOverScene", fadeDuration);
    }

    void ApplyFadeOut()
    {
        if (!isFading || fadeImage == null) return;

        fadeTimeElapsed += Time.deltaTime;
        Color c = fadeImage.color;
        c.a = Mathf.Clamp01(fadeTimeElapsed / fadeDuration);
        fadeImage.color = c;
    }

    void LoadGameOverScene()
    {
        SceneManager.LoadScene("GameOver");
    }
}
