using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class House : MonoBehaviour, IDropHandler
{
    public int houseID;                  // Уникальный ID для каждого домика
    public int level = 1;                // Начальный уровень домика
    public int rentPriceLevel = 0;          // Начальная цена аренды
    public int capacity = 2;             // Начальная вместимость (максимум людей в доме)
    public int capacityLevel = 0;             // Начальная вместимость (максимум людей в доме)
    public List<GameObject> models;      // Модели домика для разных уровней
    public AudioSource audioSource;
    public int currentResidents = 0;    // Текущее количество людей в домике

    public ParticleSystem UpgradeBuild;
    public ParticleSystem UpgradeRent;
    public PeopleBar peopleBar;

    private void Start()
    {
        if (peopleBar == null)
            peopleBar = FindObjectOfType<PeopleBar>();
        peopleBar.InitBar(this);
        LoadProgress();
        UpdateHouseModel();
    }

    public void IncreaseRentPrice()
    {
        audioSource.Play();
        UpgradeRent.Play();

        rentPriceLevel += 1;
        SaveProgress();
    }

    public void IncreaseCapacity(int amount)
    {
        audioSource.Play();
        capacity = amount;
        peopleBar.UpdateProgress();
        UpdateHouseModel();
        UpgradeBuild.Play();
        SaveProgress();
    }

    public void UpgradeHouse()
    {
        level++;
        SaveProgress();
    }

    // Метод добавления человека в домик
    public bool AddResident()
    {
        if (currentResidents < capacity)
        {
            currentResidents++;
            peopleBar.UpdateProgress();
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
        int modelIndex = (capacityLevel - 1) / 2;

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
        PlayerPrefs.SetInt($"House_{houseID}_rentPriceLevel", rentPriceLevel);
        PlayerPrefs.SetInt($"House_{houseID}_Capacity", capacity);
        PlayerPrefs.SetInt($"House_{houseID}_CapacityLevel", capacityLevel);
        PlayerPrefs.SetInt($"House_{houseID}_CurrentResidents", currentResidents);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        level = PlayerPrefs.GetInt($"House_{houseID}_Level", level);
        rentPriceLevel = PlayerPrefs.GetInt($"House_{houseID}_rentPriceLevel", rentPriceLevel);
        capacity = PlayerPrefs.GetInt($"House_{houseID}_Capacity", capacity);
        capacityLevel = PlayerPrefs.GetInt($"House_{houseID}_CapacityLevel", capacityLevel);
        currentResidents = PlayerPrefs.GetInt($"House_{houseID}_CurrentResidents", 0);
        peopleBar.UpdateProgress();
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
