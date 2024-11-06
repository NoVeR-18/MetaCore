using System.Collections.Generic;
using UnityEngine;

public class SavedPeoplePanel : MonoBehaviour
{
    private int currentPeopleCount; // ������� ���������� �������� ����������
    public GameObject personPrefab; // ������ ��������� ��� ����������� � ������
    public List<GameObject> slots; // ������ ������ ��� ����������� ����������
    public AudioSource audioSource;

    private const string SavedPeopleKey = "SavedPeopleCount"; // ���� ��� ����������

    private void Start()
    {
        LoadPeople(); // ��������� ���������� ���������� ��� ����� �� �����
        UpdatePanel();
    }

    public void AddPerson()
    {
        if (currentPeopleCount < slots.Count)
        {
            currentPeopleCount++;
            SavePeople();
            UpdatePanel();
        }
        else
        {
            Debug.Log("������ ���������, ���������� �������� ������ ����������.");
        }
    }

    public void RemovePerson(GameObject person)
    {
        if (currentPeopleCount > 0)
        {
            audioSource.Play();
            currentPeopleCount--;
            SavePeople();
            UpdatePanel();
        }
        else
        {
            Debug.Log("�� ������ ��� ���������� ��� ��������.");
        }
    }

    private void UpdatePanel()
    {
        // ������� ��� ����� ����� �����������
        foreach (var slot in slots)
        {
            // ������� ��� ������� � �����
            foreach (Transform child in slot.transform)
            {
                Destroy(child.gameObject); // ������� ���������� �� �����
            }
        }

        // ��������� �������� ���������� � ��������� �����
        for (int i = 0; i < currentPeopleCount; i++)
        {
            if (i < slots.Count) // ���������, ��� ������ �� ������� �� ������� ������ ������
            {
                // ������ ��������� � ��������������� �����
                Instantiate(personPrefab, slots[i].transform); // ��������� ��������� � ����
            }
        }
    }

    private void SavePeople()
    {
        PlayerPrefs.SetInt(SavedPeopleKey, currentPeopleCount);
        PlayerPrefs.Save();
    }

    private void LoadPeople()
    {
        currentPeopleCount = PlayerPrefs.GetInt(SavedPeopleKey, 0); // ���������, ���� ������ ���, ����� � 0
        currentPeopleCount = 2;
    }
}
