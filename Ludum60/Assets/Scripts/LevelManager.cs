using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("Настройки выхода")]
    public GameObject exitDoor;

    [Header("Настройки UI интерфейса")]
    public TextMeshProUGUI uiCounterText;

    private GeneratorItem[] allGenerators;
    private int totalGenerators;
    private int activatedCount = 0;

    void Start()
    {
        allGenerators = FindObjectsOfType<GeneratorItem>();
        totalGenerators = allGenerators.Length;

        Debug.Log($"[LEVEL] На карте обнаружено генераторов: {totalGenerators}");

        if (exitDoor != null)
        {
            exitDoor.SetActive(true);
        }

        UpdateUI();
    }

    public void CheckGenerators()
    {
        activatedCount = 0;

        foreach (var gen in allGenerators)
        {
            if (gen.isActivated) activatedCount++;
        }

        Debug.Log($"[LEVEL] Прогресс: {activatedCount} / {totalGenerators}");
        UpdateUI();

        if (activatedCount >= totalGenerators)
        {
            OpenExit();
        }
    }

    void UpdateUI()
    {
        if (uiCounterText != null)
        {
            uiCounterText.text = $"Generators activated: {activatedCount} / {totalGenerators}";
        }
    }

    void OpenExit()
    {
        Debug.Log("[LEVEL] ВСЕ ГЕНЕРАТОРЫ СЕТИ ВКЛЮЧЕНЫ! ВЫХОД ОТКРЫТ!");

        if (uiCounterText != null)
        {
            uiCounterText.text = "The door was unlocked, run!";
            uiCounterText.color = Color.whiteSmoke;
        }

        if (exitDoor != null)
        {
            exitDoor.SetActive(false);
        }
    }
}