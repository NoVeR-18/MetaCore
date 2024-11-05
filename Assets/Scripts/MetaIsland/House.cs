using UnityEngine;

public class House : MonoBehaviour
{
    public int houseID;                  // Уникальный ID для каждого домика
    public int level = 1;                // Начальный уровень домика
    public int maxLevel = 3;             // Максимальный уровень домика
    public float rentPrice = 100f;       // Начальная цена аренды
    public int capacity = 2;             // Начальная вместимость
    public GameObject[] models;          // Массив моделей домика для разных уровней

    private void Start()
    {
        LoadProgress(); // Загрузка прогресса для данного домика
        UpdateHouseModel(); // Обновление модели домика в зависимости от уровня
    }

    public void IncreaseRentPrice(float amount)
    {
        rentPrice += amount;
        SaveProgress();
    }

    public void IncreaseCapacity(int amount)
    {
        capacity += amount;
        SaveProgress();
    }

    public void UpgradeHouse()
    {
        if (level < maxLevel)
        {
            level++;
            UpdateHouseModel();
            SaveProgress();
        }
    }

    private void UpdateHouseModel()
    {
        // Отключаем все модели и включаем только нужную в зависимости от уровня
        for (int i = 0; i < models.Length; i++)
        {
            models[i].SetActive(i == level - 1);
        }
    }

    private void SaveProgress()
    {
        // Сохраняем уровень, цену аренды и вместимость домика
        PlayerPrefs.SetInt($"House_{houseID}_Level", level);
        PlayerPrefs.SetFloat($"House_{houseID}_RentPrice", rentPrice);
        PlayerPrefs.SetInt($"House_{houseID}_Capacity", capacity);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        // Загружаем сохраненные данные для домика, если они есть
        level = PlayerPrefs.GetInt($"House_{houseID}_Level", level);
        rentPrice = PlayerPrefs.GetFloat($"House_{houseID}_RentPrice", rentPrice);
        capacity = PlayerPrefs.GetInt($"House_{houseID}_Capacity", capacity);
    }
    private void OnMouseDown()
    {
        IslandManager.Instance.houseUpgradePanel.SetCurrentHouse(this);
        IslandManager.Instance.houseUpgradePanel.gameObject.SetActive(true);
        IslandManager.Instance.cameraController.FocusOnObject(transform);
    }
}
