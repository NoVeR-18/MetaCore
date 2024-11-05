using UnityEngine;

public class House : MonoBehaviour
{
    public int houseID;                  // ���������� ID ��� ������� ������
    public int level = 1;                // ��������� ������� ������
    public int maxLevel = 3;             // ������������ ������� ������
    public float rentPrice = 100f;       // ��������� ���� ������
    public int capacity = 2;             // ��������� �����������
    public GameObject[] models;          // ������ ������� ������ ��� ������ �������

    private void Start()
    {
        LoadProgress(); // �������� ��������� ��� ������� ������
        UpdateHouseModel(); // ���������� ������ ������ � ����������� �� ������
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
        // ��������� ��� ������ � �������� ������ ������ � ����������� �� ������
        for (int i = 0; i < models.Length; i++)
        {
            models[i].SetActive(i == level - 1);
        }
    }

    private void SaveProgress()
    {
        // ��������� �������, ���� ������ � ����������� ������
        PlayerPrefs.SetInt($"House_{houseID}_Level", level);
        PlayerPrefs.SetFloat($"House_{houseID}_RentPrice", rentPrice);
        PlayerPrefs.SetInt($"House_{houseID}_Capacity", capacity);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        // ��������� ����������� ������ ��� ������, ���� ��� ����
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
