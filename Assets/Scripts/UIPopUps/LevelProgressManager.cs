using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressManager : MonoBehaviour
{
    public Image experienceProgressBar;      // ����������� UI
    public TextMeshProUGUI levelText;                    // UI ��� ����������� ������
    public TextMeshProUGUI experienceText;                    // UI ��� ����������� ������
    public ExperienceTable experienceTable;   // ������ �� ������� �����
    public Animator animator;

    public int currentLevel;
    private int currentExperience;
    private int experienceForNextLevel;

    public Action UpLevel;

    private void Start()
    {
        LoadProgress();  // ��������� ����������� ������
        UpdateLevelInfo();
    }

    // ����� ��� ���������� �����
    public void AddExperience(int amount)
    {
        currentExperience += amount;

        animator?.SetTrigger("levelUp");
        // �������� �� ��������� ������
        while (currentLevel < experienceTable.experiencePerLevel.Count && currentExperience >= experienceForNextLevel)
        {
            currentExperience -= experienceForNextLevel;
            currentLevel++;
            UpdateLevelInfo();
        }

        SaveProgress();   // ��������� ����� ���������� �����
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
            experienceProgressBar.fillAmount = 1; // ��������� ��������� �����������, ���� ��������� ������������ �������
            experienceText.text = $"{experienceForNextLevel} / {experienceForNextLevel}";

        }
    }

    // ����� ��� ���������� ���������
    private void SaveProgress()
    {
        PlayerPrefs.SetInt("PlayerLevel", currentLevel);
        PlayerPrefs.SetInt("PlayerExperience", currentExperience);
        PlayerPrefs.Save();
    }

    // ����� ��� �������� ���������
    private void LoadProgress()
    {
        currentLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        currentExperience = PlayerPrefs.GetInt("PlayerExperience", 0);
    }
}
