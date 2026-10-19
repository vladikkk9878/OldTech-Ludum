using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GeneratorItem : MonoBehaviour
{
    [Header("Настройки генератора")]
    public bool isActivated = false;
    public Color activatedColor = Color.green;

    [Header("Звуковые эффекты")]
    public AudioClip activationSound;
    public AudioClip workingLoopSound;
    public float fadeDuration = 2.0f;

    private AudioSource activationAudioSource;
    private AudioSource loopAudioSource;

    void Start()
    {
        activationAudioSource = GetComponent<AudioSource>();
        activationAudioSource.playOnAwake = false;
        activationAudioSource.spatialBlend = 1f;

        loopAudioSource = gameObject.AddComponent<AudioSource>();
        loopAudioSource.playOnAwake = false;
        loopAudioSource.loop = true;
        loopAudioSource.spatialBlend = 1f;
        
        loopAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        loopAudioSource.minDistance = 1f;
        loopAudioSource.maxDistance = 20f;
    }

    public void ActivateGenerator()
    {
        if (isActivated) return;

        isActivated = true;
        Debug.Log($"[LEVEL] {gameObject.name} успешно АКТИВИРОВАН!");

        if (activationAudioSource != null && activationSound != null)
        {
            activationAudioSource.clip = activationSound;
            activationAudioSource.Play();
        }

        if (loopAudioSource != null && workingLoopSound != null)
        {
            StartCoroutine(PlayLoopWithDelayAndFade());
        }

        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = activatedColor;
        }

        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.CheckGenerators();
        }
    }

    private IEnumerator PlayLoopWithDelayAndFade()
    {
        yield return new WaitForSeconds(0.5f);

        loopAudioSource.clip = workingLoopSound;
        loopAudioSource.volume = 0f;
        loopAudioSource.Play();

        float currentTime = 0f;
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            loopAudioSource.volume = Mathf.Lerp(0f, 0.5f, currentTime / fadeDuration);
            yield return null;
        }

        loopAudioSource.volume = 0.5f;
    }
}
