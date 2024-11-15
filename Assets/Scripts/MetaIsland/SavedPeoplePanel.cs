using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PersonPrefab
{
    public TypeObject type;
    public GameObject prefab;
}

public class SavedPeoplePanel : MonoBehaviour
{
    public List<PersonPrefab> personPrefabs; // ������ � ��������� ���������� ������ �����
    private int currentPeopleCount;
    public List<GameObject> slots;
    public AudioSource audioSource;
    public Transform panelIsFull;
    public const string SavedPeopleKey = "SavedPeopleCount";

    private const string SavedPeopleTypesKey = "SavedPeopleTypes"; // ����� ���� ��� �����

    private List<TypeObject> savedPeopleTypes = new List<TypeObject>(); // ��������� ������ ��� �������� ����� ����������� �����

    private void Start()
    {
        LoadPeople();
        UpdatePanel();
    }

    public GameObject GetPersonPrefab(TypeObject type)
    {
        // ������� ������ �� ����
        PersonPrefab personPrefab = personPrefabs.Find(p => p.type == type);
        return personPrefab?.prefab;
    }

    public static void AddPerson(TypeObject personType)
    {
        int peopleCount = PlayerPrefs.GetInt(SavedPeopleKey, 0);

        if (peopleCount < 5) // ����� �� 5 �������
        {
            // ��������� ���������� �����
            PlayerPrefs.SetInt(SavedPeopleKey, peopleCount + 1);

            // ��������� ������� ������ �����, ��������� ����� ��� � ��������� �������
            List<TypeObject> types = LoadPeopleTypes();
            types.Add(personType);
            SavePeopleTypes(types);

            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("������ ���������, ���������� �������� ������ ����������.");
        }
    }

    private void UpdatePanel()
    {
        // ������� ��� ����� ����� �����������
        foreach (var slot in slots)
        {
            foreach (Transform child in slot.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // ��������� �������� ���������� � ��������� �����
        for (int i = 0; i < savedPeopleTypes.Count && i < slots.Count; i++)
        {
            TypeObject personType = savedPeopleTypes[i];
            GameObject prefab = GetPersonPrefab(personType);
            if (prefab != null)
            {
                var pop = Instantiate(prefab, slots[i].transform).GetComponent<DraggablePerson>();
                pop.typeObject = personType;


            }
        }

        panelIsFull.gameObject.SetActive(currentPeopleCount == slots.Count);
    }

    public void RemovePerson(GameObject person)
    {
        if (currentPeopleCount > 0)
        {
            // ���������� ��� ���������, ����� ����� ��� � ������ ���������� �����
            TypeObject personType = person.GetComponent<DraggablePerson>().typeObject;

            // ���� ������ ������� � ��������� ����� � ������� ��� �� ���������� �����
            int indexToRemove = savedPeopleTypes.IndexOf(personType);
            if (indexToRemove >= 0)
            {
                audioSource.Play();
                currentPeopleCount--;

                // ������� ��������� ��� �� ������ � ��������� ���������
                savedPeopleTypes.RemoveAt(indexToRemove);
                SavePeopleTypes(savedPeopleTypes);

                PlayerPrefs.SetInt(SavedPeopleKey, currentPeopleCount);
                PlayerPrefs.Save();

                // ��������� ������
                UpdatePanel();
            }
            else
            {
                Debug.Log("��������� �� ������ � ���������� �����.");
            }
        }
        else
        {
            Debug.Log("�� ������ ��� ���������� ��� ��������.");
        }
    }


    private void LoadPeople()
    {
        currentPeopleCount = PlayerPrefs.GetInt(SavedPeopleKey, 0);
        savedPeopleTypes = LoadPeopleTypes();
    }

    private static List<TypeObject> LoadPeopleTypes()
    {
        string typesString = PlayerPrefs.GetString(SavedPeopleTypesKey, "");
        List<TypeObject> types = new List<TypeObject>();

        if (!string.IsNullOrEmpty(typesString))
        {
            string[] typeStrings = typesString.Split(',');
            foreach (var typeString in typeStrings)
            {
                if (System.Enum.TryParse(typeString, out TypeObject type))
                {
                    types.Add(type);
                }
            }
        }

        return types;
    }

    private static void SavePeopleTypes(List<TypeObject> types)
    {
        string typesString = string.Join(",", types);
        PlayerPrefs.SetString(SavedPeopleTypesKey, typesString);
    }
}
