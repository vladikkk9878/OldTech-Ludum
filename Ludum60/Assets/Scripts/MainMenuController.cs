using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip transitionSound;
    public AudioClip menuAmbient;
    public Image fadeOverlay;
    
    private bool isStarting = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
            fadeOverlay.gameObject.SetActive(false);
        }

        if (audioSource != null && menuAmbient != null)
        {
            audioSource.clip = menuAmbient;
            audioSource.loop = true;
            audioSource.volume = 0.5f;
            audioSource.Play();
        }
    }

    public void StartGameWithDelay()
    {
        if (isStarting) return;
        
        isStarting = true;
        StartCoroutine(LoadGameplayScene());
    }

    private IEnumerator LoadGameplayScene()
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

        SceneManager.LoadScene("MainGame");
    }
}
