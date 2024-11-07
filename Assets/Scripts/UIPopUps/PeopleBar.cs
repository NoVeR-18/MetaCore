using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PeopleBar : MonoBehaviour
{
    public Image progressBar;  // Ссылка на UI-прогрессбар (Slider)
    public TextMeshProUGUI progressText;   // Текст для отображения количества заселённых человечков
    public List<House> houses;  // Список всех домиков на сцене

    private int totalResidents;     // Общее количество заселённых человечков
    private int maxCapacity;        // Общая максимальная вместимость

    private void Start()
    {
        UpdateProgress();
    }
    public void InitBar(House house)
    {
        if (!houses.Contains(house))
        {
            houses.Add(house);
        }
    }
    // Метод для обновления прогресса
    public void UpdateProgress()
    {
        totalResidents = 0;
        maxCapacity = 0;

        // Подсчёт общего количества заселённых человечков и максимальной вместимости
        foreach (House house in houses)
        {
            totalResidents += house.currentResidents;  // Получаем текущее количество жителей
            maxCapacity += house.capacity;             // Получаем максимальную вместимость
        }

        // Вычисляем процент заполненности
        float progress = (float)totalResidents / maxCapacity;

        // Обновляем прогрессбар и текст
        progressBar.fillAmount = progress;
        progressText.text = $"{totalResidents} / {maxCapacity}";
    }
}
