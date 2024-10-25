using UnityEngine;

#if UNITY_EDITOR
#endif

[ExecuteInEditMode]
public class TilemapToXZ : MonoBehaviour
{
    public WallTilemap objectsContainer;
    private PrefabData prefabData = new PrefabData();

    void Start()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Initialize();
        }
#endif
    }
    void Initialize()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        objectsContainer = FindObjectOfType<WallTilemap>();
        //objectsContainer = transform.parent.GetComponent<WallTilemap>();
        prefabData.position = gameObject.transform.position;
        prefabData.name = gameObject.name;

        TogglePrefabInList(prefabData);
    }

    public void TogglePrefabInList(PrefabData newPrefabData)
    {
        PrefabData existingPrefab = objectsContainer.items.Find(prefab => prefab.position == newPrefabData.position);

        if (existingPrefab != null)
        {
            if (!Application.isPlaying) // Убедимся, что мы не в режиме игры
            {
                DestroyImmediate(gameObject); // Удаляем объект в редакторе
            }
        }
        else
        {
            objectsContainer.items.Add(newPrefabData);
            Debug.Log($"Prefab at position {newPrefabData.position} added.");
        }
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        // Убедимся, что это не вызвано выходом из игрового режима
        if (!Application.isPlaying && objectsContainer != null)
        {
            PrefabData existingPrefab = objectsContainer.items.Find(prefab => prefab == prefabData);

            if (existingPrefab != null)
            {
                objectsContainer.items.Remove(existingPrefab);
            }
        }
#endif
    }
}
