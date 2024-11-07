using System.Collections.Generic;
using UnityEngine;

public class SavedPeoplePanel : MonoBehaviour
{
    private int currentPeopleCount; // ������� ���������� �������� ����������
    public GameObject personPrefab; // ������ ��������� ��� ����������� � ������
    public List<GameObject> slots; // ������ ������ ��� ����������� ����������
    public AudioSource audioSource;

    public const string SavedPeopleKey = "SavedPeopleCount"; // ���� ��� ����������

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
            SavePeople(currentPeopleCount);
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
            SavePeople(currentPeopleCount);
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

    public static void SavePeople(int PeopleCount)
    {
        if (PlayerPrefs.GetInt(SavedPeopleKey, 0) + PeopleCount >= 5)

            PlayerPrefs.SetInt(SavedPeopleKey, 5);
        else
            PlayerPrefs.SetInt(SavedPeopleKey, PeopleCount);
        PlayerPrefs.Save();
    }

    private void LoadPeople()
    {
        currentPeopleCount = PlayerPrefs.GetInt(SavedPeopleKey, 0); // ���������, ���� ������ ���, ����� � 0
        currentPeopleCount = 2;
    }
}
