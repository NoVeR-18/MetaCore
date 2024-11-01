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
        Vector3 eulerRotation = transform.rotation.eulerAngles;

        transform.rotation = Quaternion.Euler(0, eulerRotation.y, 0);
        //transform.rotation = Quaternion.Euler(0, transform.rotation.y, transform.rotation.z);
        objectsContainer = FindObjectOfType<WallTilemap>();
        //objectsContainer = transform.parent.GetComponent<WallTilemap>();
        prefabData.position = gameObject.transform.position;
        prefabData.rotations = gameObject.transform.rotation;
        prefabData.name = gameObject.name;
        objectsContainer.itemsTransform.Add(gameObject.transform);
        TogglePrefabInList(prefabData);
    }

    public void TogglePrefabInList(PrefabData newPrefabData)
    {
        PrefabData existingPrefab = objectsContainer.items.Find(prefab => prefab.position == newPrefabData.position);

        if (existingPrefab != null)
        {
            if (!Application.isPlaying) // Убедимся, что мы не в режиме игры
            {

                objectsContainer.items.Remove(existingPrefab);
                objectsContainer.items.Add(newPrefabData);
                var item = objectsContainer.itemsTransform.Find(prefab => prefab.position == existingPrefab.position);
                DestroyImmediate(item.gameObject); // Удаляем объект в редакторе
                Debug.Log($"Replased {existingPrefab.name} on {newPrefabData.name}");
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
