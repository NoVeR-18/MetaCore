using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressManager : MonoBehaviour
{
    public Image experienceProgressBar;      // Прогрессбар UI
    public TextMeshProUGUI levelText;                    // UI для отображения уровня
    public TextMeshProUGUI experienceText;                    // UI для отображения уровня
    public ExperienceTable experienceTable;   // Ссылка на таблицу опыта
    public Animator animator;

    public int currentLevel;
    private int currentExperience;
    private int experienceForNextLevel;

    public Action UpLevel;

    private void Start()
    {
        LoadProgress();  // Загрузить сохраненные данные
        UpdateLevelInfo();
    }

    // Метод для добавления опыта
    public void AddExperience(int amount)
    {
        currentExperience += amount;

        animator?.SetTrigger("levelUp");
        // Проверка на повышение уровня
        while (currentLevel < experienceTable.experiencePerLevel.Count && currentExperience >= experienceForNextLevel)
        {
            currentExperience -= experienceForNextLevel;
            currentLevel++;
            UpdateLevelInfo();
        }

        SaveProgress();   // Сохранить после добавления опыта
        UpdateProgressBar();
    }

    private void UpdateLevelInfo()
    {
        if (currentLevel - 1 < experienceTable.experiencePerLevel.Count)
        {
            experienceForNextLevel = experienceTable.experiencePerLevel[currentLevel - 1];
            levelText.text = $"{currentLevel}";
            UpdateProgressBar();
        }
        else
        {
            experienceForNextLevel = 0;
        }
        UpLevel?.Invoke();
    }

    private void UpdateProgressBar()
    {
        if (experienceForNextLevel > 0)
        {
            experienceProgressBar.fillAmount = (float)currentExperience / experienceForNextLevel;
            experienceText.text = $"{currentExperience} / {experienceForNextLevel}";
        }
        else
        {
            experienceProgressBar.fillAmount = 1; // Полностью заполнить прогрессбар, если достигнут максимальный уровень
            experienceText.text = $"{experienceForNextLevel} / {experienceForNextLevel}";

        }
    }

    // Метод для сохранения прогресса
    private void SaveProgress()
    {
        PlayerPrefs.SetInt("PlayerLevel", currentLevel);
        PlayerPrefs.SetInt("PlayerExperience", currentExperience);
        PlayerPrefs.Save();
    }

    // Метод для загрузки прогресса
    private void LoadProgress()
    {
        currentLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        currentExperience = PlayerPrefs.GetInt("PlayerExperience", 0);
    }
}
