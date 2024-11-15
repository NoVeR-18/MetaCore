using System;
using System.Collections.Generic;
using TMPro;
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

    public Animator animator;
    public TextMeshPro countCoinAddedText;

    public static Action GetCoins;

    public float incomePerInterval = 1; // Прибыль за 10 секунд
    private float incomeTimer = 15; // Таймер для накопления прибыли
    private void Start()
    {
        if (peopleBar == null)
            peopleBar = FindObjectOfType<PeopleBar>();
        peopleBar.InitBar(this);
        LoadProgress();
        incomePerInterval = currentResidents * (1 + 0.01f * level);
        CalculateOfflineIncome();
        UpdateHouseModel();
        TutorialMeta.Instance.BuildHouse();
    }
    private void FixedUpdate()
    {
        // Таймер для онлайн-прибыли
        incomeTimer -= Time.deltaTime;
        if (incomeTimer <= 0f)
        {
            if (incomePerInterval > 0)
                GenerateIncome();
            incomeTimer = 15; // Сбросить таймер на 10 секунд
        }
    }
    private void GenerateIncome()
    {
        IslandManager.Instance.playerWallet.AddMoney(incomePerInterval);
        countCoinAddedText.text = ((int)incomePerInterval).ToString();
        animator.SetTrigger("Show");
        Debug.Log($"Дом {houseID} принёс прибыль: {incomePerInterval}");
        GetCoins?.Invoke();
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
            incomePerInterval = currentResidents * (1 + 0.01f * level);
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
            modelIndex = models.Count - 1;

        for (int i = 0; i < models.Count; i++)
            models[i].SetActive(i == modelIndex);
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt($"House_{houseID}_Level", level);
        PlayerPrefs.SetInt($"House_{houseID}_rentPriceLevel", rentPriceLevel);
        PlayerPrefs.SetInt($"House_{houseID}_Capacity", capacity);
        PlayerPrefs.SetInt($"House_{houseID}_CapacityLevel", capacityLevel);
        PlayerPrefs.SetInt($"House_{houseID}_CurrentResidents", currentResidents);
        PlayerPrefs.SetString($"House_{houseID}_LastSavedTime", DateTime.Now.ToString());
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
        if (EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.tutorialCompleted)
        {
            return;
        }
        IslandManager.Instance.houseUpgradePanel.SetCurrentHouse(this);
        IslandManager.Instance.houseUpgradePanel.gameObject.SetActive(true);
        IslandManager.Instance.cameraController.FocusOnObject(transform);
        TutorialMeta.Instance.UpgradeHouse();
    }
    private void CalculateOfflineIncome()
    {
        string lastSavedTime = PlayerPrefs.GetString($"House_{houseID}_LastSavedTime", DateTime.Now.ToString());
        DateTime lastTime;
        if (DateTime.TryParse(lastSavedTime, out lastTime))
        {
            TimeSpan timeAway = DateTime.Now - lastTime;
            double maxOfflineSeconds = 2 * 60 * 60; // Максимум 2 часа
            double secondsAway = Math.Min(timeAway.TotalSeconds, maxOfflineSeconds);
            float offlineIncome = (float)(secondsAway / 15) * incomePerInterval;

            if (offlineIncome > 0)
            {

                if (GameManager.Instance.hasShownIncomePanel && secondsAway >= 20f)
                {
                    IslandManager.Instance.incomePanel.gameObject.SetActive(true);
                    IslandManager.Instance.incomePanel.totalCollected += (int)offlineIncome;
                    IslandManager.Instance.incomePanel.UpdateUI();
                    GameManager.Instance.hasShownIncomePanel = false;
                }
                else
                    IslandManager.Instance.playerWallet.AddMoney(offlineIncome);


                Debug.Log($"Дом {houseID} принёс прибыль оффлайн: {offlineIncome}");
            }
        }
    }
    private void OnApplicationQuit()
    {
        SaveProgress();
    }
}
