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
        objectsContainer.itemsTransform.Add(gameObject.transform);
        TogglePrefabInList(prefabData);
    }

    public void TogglePrefabInList(PrefabData newPrefabData)
    {
        PrefabData existingPrefab = objectsContainer.items.Find(prefab => prefab.position == newPrefabData.position);

        if (existingPrefab != null)
        {
            if (!Application.isPlaying) // ��������, ��� �� �� � ������ ����
            {

                objectsContainer.items.Remove(existingPrefab);
                objectsContainer.items.Add(newPrefabData);
                var item = objectsContainer.itemsTransform.Find(prefab => prefab.position == existingPrefab.position);
                DestroyImmediate(item.gameObject); // ������� ������ � ���������
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
        // ��������, ��� ��� �� ������� ������� �� �������� ������
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
