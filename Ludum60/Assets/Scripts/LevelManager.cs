using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("Exit Settings")]
    public GameObject exitDoor;

    [Header("UI Settings")]
    public TextMeshProUGUI uiCounterText;

    [Header("Monster Spawn Settings")]
    public GameObject monsterObject;
    public int generatorsRequiredForMonster = 3;

    private GeneratorItem[] allGenerators;
    private int totalGenerators;
    private int activatedCount = 0;
    private bool isMonsterSpawned = false;

    void Start()
    {
        allGenerators = FindObjectsOfType<GeneratorItem>();
        totalGenerators = allGenerators.Length;

        if (exitDoor != null)
        {
            exitDoor.SetActive(true);
        }

        if (monsterObject != null)
        {
            monsterObject.SetActive(false);
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

        UpdateUI();

        if (activatedCount >= generatorsRequiredForMonster && !isMonsterSpawned)
        {
            SpawnMonster();
        }

        if (activatedCount >= totalGenerators)
        {
            OpenExit();
        }
    }

    void SpawnMonster()
    {
        isMonsterSpawned = true;
        if (monsterObject != null)
        {
            monsterObject.SetActive(true);
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