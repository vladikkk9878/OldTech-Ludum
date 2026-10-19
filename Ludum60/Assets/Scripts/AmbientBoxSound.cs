using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientBoxSound : MonoBehaviour
{
    [Header("Список страшных звуков для этой коробки")]
    public List<AudioClip> sounds = new List<AudioClip>();
    
    [Range(0f, 1f)]
    public float volume = 0.5f;

    [Header("Интервалы между звуками (сек)")]
    public float minDelay = 15f;
    public float maxDelay = 40f;

    [Header("Дистанция слышимости 3D звука")]
    public float minDistance = 3f;
    public float maxDistance = 25f;

    [Header("Разброс тональности")]
    public float minPitch = 0.8f;
    public float maxPitch = 1.1f;

    private AudioSource audioSource;
    private AudioListener playerListener;

    void Start()
    {
        if (sounds == null || sounds.Count == 0) return;

        playerListener = FindFirstObjectByType<AudioListener>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;

        StartCoroutine(PlaySoundsRandomly());
    }

    private IEnumerator PlaySoundsRandomly()
    {
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            if (sounds.Count > 0 && audioSource != null && !audioSource.isPlaying)
            {
                if (playerListener != null)
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, playerListener.transform.position);
                    if (distanceToPlayer > maxDistance) continue;
                }

                int randomIndex = Random.Range(0, sounds.Count);
                audioSource.clip = sounds[randomIndex];
                audioSource.volume = volume;
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.Play();
            }
        }
    }
}
