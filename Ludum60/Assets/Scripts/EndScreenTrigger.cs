using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenTrigger : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip transitionSound;
    public AudioClip gameplayAmbient;
    public Image fadeOverlay;
    
    private bool isEnding = false;

    void Start()
    {
        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
            fadeOverlay.gameObject.SetActive(false);
        }

        if (audioSource != null && gameplayAmbient != null)
        {
            audioSource.clip = gameplayAmbient;
            audioSource.loop = true;
            audioSource.volume = 0.5f;
            audioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isEnding) return;

        isEnding = true;
        StartCoroutine(LoadEndMenuScene());
    }

    private IEnumerator LoadEndMenuScene()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            
            if (clickSound != null)
            {
                audioSource.PlayOneShot(clickSound);
            }
            if (transitionSound != null)
            {
                audioSource.PlayOneShot(transitionSound);
            }
        }

        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            float elapsedTime = 0f;
            
            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime;
                Color c = fadeOverlay.color;
                c.a = Mathf.Clamp01(elapsedTime / 1f);
                fadeOverlay.color = c;
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        SceneManager.LoadScene("EndMenu");
    }
}