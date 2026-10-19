using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicPlayer : MonoBehaviour
{
    [Header("Настройки трека")]
    public AudioClip backgroundMusic;
    public bool loop = true;
    
    [Range(0f, 1f)]
    public float targetVolume = 0.5f;

    [Header("Плавное появление (сек)")]
    public float fadeDuration = 2.0f;

    private AudioSource audioSource;

    void Start()
    {
        if (backgroundMusic == null) return;

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D звук (играет везде одинаково)
        audioSource.loop = loop;
        
        audioSource.clip = backgroundMusic;
        audioSource.volume = 0f;
        audioSource.Play();

        StartCoroutine(FadeInMusic());
    }

    private IEnumerator FadeInMusic()
    {
        float currentTime = 0f;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, currentTime / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}