using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class House : MonoBehaviour, IDropHandler
{
    public int houseID;                  // ���������� ID ��� ������� ������
    public int level = 1;                // ��������� ������� ������
    public int rentPriceLevel = 0;          // ��������� ���� ������
    public int capacity = 2;             // ��������� ����������� (�������� ����� � ����)
    public int capacityLevel = 0;             // ��������� ����������� (�������� ����� � ����)
    public List<GameObject> models;      // ������ ������ ��� ������ �������
    public AudioSource audioSource;
    public int currentResidents = 0;    // ������� ���������� ����� � ������

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

    // ����� ���������� �������� � �����
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
            Debug.Log("��� �����, ���������� �������� ������ ��������.");
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

    // ����� ��������� ������� �������������� ������� �� �����
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop triggered.");  // ���������, ��������� �� ������� OnDrop

        DraggablePerson person = eventData.pointerDrag?.GetComponent<DraggablePerson>();
        if (person == null)
        {
            Debug.LogWarning("Dragged object is not a DraggablePerson.");
            return;
        }

        if (AddResident())
        {
            // ���� ���� ��������� �����, ��������� ���������
            Debug.Log("Resident added to the house.");

            SavedPeoplePanel savedPeoplePanel = FindObjectOfType<SavedPeoplePanel>();
            savedPeoplePanel.RemovePerson(person.gameObject);  // ������� � ������
            Destroy(person.gameObject);  // ���������� ������
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
