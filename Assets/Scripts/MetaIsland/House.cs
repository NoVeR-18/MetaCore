using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class House : MonoBehaviour, IDropHandler
{
    public int houseID;                  // Уникальный ID для каждого домика
    public int level = 1;                // Начальный уровень домика
    public int maxLevel = 25;            // Максимальный уровень домика
    public float rentPrice = 3;          // Начальная цена аренды
    public int capacity = 2;             // Начальная вместимость (максимум людей в доме)
    public List<GameObject> models;      // Модели домика для разных уровней
    public AudioSource audioSource;
    public int currentResidents = 0;    // Текущее количество людей в домике

    public ParticleSystem UpgradeBuild;
    public ParticleSystem UpgradeRent;

    private void Start()
    {
        LoadProgress();
        UpdateHouseModel();
    }

    public void IncreaseRentPrice(float amount)
    {
        audioSource.Play();
        UpgradeRent.Play();
        rentPrice += amount;
        SaveProgress();
    }

    public void IncreaseCapacity(int amount)
    {
        audioSource.Play();
        capacity += amount;
        UpgradeBuild.Play();
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

    // Метод добавления человека в домик
    public bool AddResident()
    {
        if (currentResidents < capacity)
        {
            currentResidents++;
            SaveProgress();
            return true;
        }
        else
        {
            Debug.Log("Дом полон, невозможно добавить нового человека.");
            return false;
        }
    }

    private void UpdateHouseModel()
    {
        int modelIndex = (level - 1) / 5;

        if (modelIndex >= models.Count)
        {
            modelIndex = models.Count - 1;
        }

        for (int i = 0; i < models.Count; i++)
        {
            models[i].SetActive(i == modelIndex);
        }
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt($"House_{houseID}_Level", level);
        PlayerPrefs.SetFloat($"House_{houseID}_RentPrice", rentPrice);
        PlayerPrefs.SetInt($"House_{houseID}_Capacity", capacity);
        PlayerPrefs.SetInt($"House_{houseID}_CurrentResidents", currentResidents);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        level = PlayerPrefs.GetInt($"House_{houseID}_Level", level);
        rentPrice = PlayerPrefs.GetFloat($"House_{houseID}_RentPrice", rentPrice);
        capacity = PlayerPrefs.GetInt($"House_{houseID}_Capacity", capacity);
        currentResidents = PlayerPrefs.GetInt($"House_{houseID}_CurrentResidents", 0);
    }

    // Метод обработки события перетаскивания объекта на домик
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop triggered.");  // Проверяем, сработало ли событие OnDrop

        DraggablePerson person = eventData.pointerDrag?.GetComponent<DraggablePerson>();
        if (person == null)
        {
            Debug.LogWarning("Dragged object is not a DraggablePerson.");
            return;
        }

        if (AddResident())
        {
            // Если есть свободное место, добавляем человечка
            Debug.Log("Resident added to the house.");

            SavedPeoplePanel savedPeoplePanel = FindObjectOfType<SavedPeoplePanel>();
            savedPeoplePanel.RemovePerson(person.gameObject);  // Убираем с панели
            Destroy(person.gameObject);  // Уничтожаем объект
        }
        else
        {
            Debug.Log("House is full, resident cannot be added.");
        }
    }


    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        IslandManager.Instance.houseUpgradePanel.SetCurrentHouse(this);
        IslandManager.Instance.houseUpgradePanel.gameObject.SetActive(true);
        IslandManager.Instance.cameraController.FocusOnObject(transform);
    }
}
