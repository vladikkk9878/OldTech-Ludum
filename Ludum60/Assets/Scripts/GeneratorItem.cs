using UnityEngine;

public class GeneratorItem : MonoBehaviour
{
    [Header("Настройки генератора")]
    public bool isActivated = false;
    public Color activatedColor = Color.green;

    public void ActivateGenerator()
    {
        if (isActivated) return;

        isActivated = true;
        Debug.Log($"[LEVEL] {gameObject.name} успешно АКТИВИРОВАН!");

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