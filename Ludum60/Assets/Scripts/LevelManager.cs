using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Настройки выхода")]
    public GameObject exitDoor;

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
    }

    public void CheckGenerators()
    {
        activatedCount = 0;

        foreach (var gen in allGenerators)
        {
            if (gen.isActivated) activatedCount++;
        }

        Debug.Log($"[LEVEL] Прогресс: {activatedCount} / {totalGenerators}");

        if (activatedCount >= totalGenerators)
        {
            OpenExit();
        }
    }

    void OpenExit()
    {
        Debug.Log("[LEVEL] ВСЕ ГЕНЕРАТОРЫ СЕТИ ВКЛЮЧЕНЫ! ВЫХОД ОТКРЫТ!");
        if (exitDoor != null)
        {
            exitDoor.SetActive(false);
        }
    }
}
