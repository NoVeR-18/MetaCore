using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class House : MonoBehaviour, IDropHandler
{
    public int houseID;                  // ���������� ID ��� ������� ������
    public int level = 1;                // ��������� ������� ������
    public int maxLevel = 25;            // ������������ ������� ������
    public float rentPrice = 3;          // ��������� ���� ������
    public int capacity = 2;             // ��������� ����������� (�������� ����� � ����)
    public List<GameObject> models;      // ������ ������ ��� ������ �������
    public AudioSource audioSource;
    public int currentResidents = 0;    // ������� ���������� ����� � ������

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

    // ����� ���������� �������� � �����
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
            Debug.Log("��� �����, ���������� �������� ������ ��������.");
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
