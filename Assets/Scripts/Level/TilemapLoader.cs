using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapLoader : MonoBehaviour
{
    public TilemapData tilemapData;
    public WallTilemap objectsContainer;
    public Tilemap tilemapToPaint;
    public Transform itemsPrefabContainer;
    public List<GameObject> allPrefabs;
    public List<GameObject> itemsPrefabs = new List<GameObject>();


    public void LoadTilemap()
    {
        if (tilemapData == null || objectsContainer == null)
        {
            Debug.LogError("TilemapData или Tilemap не указаны!");
            return;
        }
        tilemapToPaint.ClearAllTiles();
        objectsContainer = tilemapToPaint.GetComponent<WallTilemap>();
        var walls = objectsContainer.items;
        walls.Clear();
        for (int i = objectsContainer.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = objectsContainer.transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }

        foreach (PrefabData prefabData in tilemapData.Wallprefabs)
        {
            GameObject prefabToInstantiate = allPrefabs.Find(prefab => prefab.name == prefabData.name);
            if (prefabToInstantiate != null)
            {
                var item = Instantiate(prefabToInstantiate, prefabData.position, prefabData.rotations, objectsContainer.transform);
                item.name = prefabData.name;
            }
            else
            {
                Debug.LogError($"Префаб типа '{prefabData.prefabType}' не найден!");
            }
        }


        foreach (TileData tileData in tilemapData.tilesToPaint)
        {
            TileBase tileToPlace = GetTileFromPalette(tileData.tileName);

            if (tileToPlace != null)
            {
                tilemapToPaint.SetTile(tileData.position, tileToPlace);
            }
            else
            {
                Debug.LogError($"Tile '{tileData.tileName}' не найден в палитре для окрашивания.");
            }
        }

        foreach (Transform child in itemsPrefabContainer)
            Destroy(child.gameObject);
        foreach (PrefabData prefabData in tilemapData.prefabs)
        {
            GameObject prefabToInstantiate = itemsPrefabs.Find(prefab => prefab.GetComponent<InteractableObject>().typeObject == prefabData.prefabType);
            if (prefabToInstantiate != null)
            {
                var item = Instantiate(prefabToInstantiate, prefabData.position, Quaternion.identity, itemsPrefabContainer);
                if (prefabData.prefabType == TypeObject.Crystal)
                    item.transform.rotation = Quaternion.Euler(90, 0, 0);
                if (prefabData.prefabType == TypeObject.Coin)
                    item.transform.rotation = Quaternion.Euler(90, 0, 0);

                if (prefabData.prefabType == TypeObject.SpawnPossiton)
                    item.transform.rotation = Quaternion.Euler(90, 0, 0);

            }
            else
            {
                Debug.LogError($"Префаб типа '{prefabData.prefabType}' не найден!");
            }
        }
    }
    TileBase GetTileFromPalette(string tileName)
    {
        foreach (TileBase tile in tilemapData.tilesPaliter)
        {
            if (tile != null && tile.name == tileName)
            {
                return tile;
            }
        }
        return null;
    }
    public Vector3 FindCenterFromObjects()
    {
        // Проверяем, есть ли объекты в контейнере
        if (objectsContainer.transform.childCount == 0)
        {
            Debug.LogError("В контейнере объектов нет дочерних элементов.");
            return Vector3.zero;
        }

        // Инициализируем минимальные и максимальные координаты
        Vector3 minPos = Vector3.positiveInfinity;
        Vector3 maxPos = Vector3.negativeInfinity;

        // Проходим по всем объектам в контейнере и ищем крайние координаты
        foreach (Transform child in objectsContainer.transform)
        {
            // Получаем мировую позицию каждого объекта
            Vector3 objPos = child.position;

            // Обновляем минимальные и максимальные координаты
            minPos = Vector3.Min(minPos, objPos);
            maxPos = Vector3.Max(maxPos, objPos);
        }

        // Вычисляем центр между минимальной и максимальной позицией
        Vector3 center = (minPos + maxPos) / 2f;

        // Возвращаем только X и Z, так как карта в плоскости XZ
        return new Vector3(center.x, 0f, center.z);
    }





    public Vector2 SizeOfField()
    {
        BoundsInt bounds = tilemapToPaint.cellBounds;

        Vector3Int min = new Vector3Int(int.MaxValue, int.MaxValue, 0);
        Vector3Int max = new Vector3Int(int.MinValue, int.MinValue, 0);

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemapToPaint.HasTile(pos))
            {
                min = Vector3Int.Min(min, pos);
                max = Vector3Int.Max(max, pos);
            }
        }

        if (min == new Vector3Int(int.MaxValue, int.MaxValue, 0) || max == new Vector3Int(int.MinValue, int.MinValue, 0))
        {
            return Vector2.zero;
        }

        float width = (max.x - min.x + 1) * tilemapToPaint.cellSize.x;
        float height = (max.y - min.y + 1) * tilemapToPaint.cellSize.y;

        return new Vector2(width, height);
    }
}
