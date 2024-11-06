using System.Collections.Generic;
using UnityEngine;

public class SavedPeoplePanel : MonoBehaviour
{
    private int currentPeopleCount; // Текущее количество спасённых человечков
    public GameObject personPrefab; // Префаб человечка для отображения в слотах
    public List<GameObject> slots; // Массив слотов для отображения человечков
    public AudioSource audioSource;

    public const string SavedPeopleKey = "SavedPeopleCount"; // Ключ для сохранения

    private void Start()
    {
        LoadPeople(); // Загружаем количество человечков при входе на сцену
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
            Debug.Log("Панель заполнена, невозможно добавить больше человечков.");
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
            Debug.Log("На панели нет человечков для удаления.");
        }
    }

    private void UpdatePanel()
    {
        // Очищаем все слоты перед обновлением
        foreach (var slot in slots)
        {
            // Удаляем все объекты в слоте
            foreach (Transform child in slot.transform)
            {
                Destroy(child.gameObject); // Удаляем человечков из слота
            }
        }

        // Добавляем спасённых человечков в доступные слоты
        for (int i = 0; i < currentPeopleCount; i++)
        {
            if (i < slots.Count) // Проверяем, что индекс не выходит за пределы списка слотов
            {
                // Создаём человечка в соответствующем слоте
                Instantiate(personPrefab, slots[i].transform); // Добавляем человечка в слот
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
        currentPeopleCount = PlayerPrefs.GetInt(SavedPeopleKey, 0); // Загружаем, если данных нет, начнём с 0
        currentPeopleCount = 2;
    }
}
