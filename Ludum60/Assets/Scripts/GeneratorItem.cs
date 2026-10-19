using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GeneratorItem : MonoBehaviour
{
    [Header("Настройки генератора")]
    public bool isActivated = false;
    public Color activatedColor = Color.green;

    [Header("Звуковые эффекты")]
    public AudioClip activationSound;

    private AudioSource audioSource;

    void Start()
    {

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    public void ActivateGenerator()
    {
        if (isActivated) return;

        isActivated = true;
        Debug.Log($"[LEVEL] {gameObject.name} успешно АКТИВИРОВАН!");

        if (audioSource != null && activationSound != null)
        {
            audioSource.clip = activationSound;
            audioSource.Play();
        }

        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = activatedColor;
        }

        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.CheckGenerators();
        }
    }
}