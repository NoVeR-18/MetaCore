#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSaver : MonoBehaviour
{
    public WallTilemap objectsContainer;
    public Tilemap targetTilemapToPaint;
    public Transform prefabToSaveConteiner;
    public List<GameObject> allPrefabs = new List<GameObject>();
    public List<GameObject> itemsPrefabs = new List<GameObject>();

    [MenuItem("Tools/Save Tilemap")]
    public static void SaveTilemap()
    {
        TilemapSaver saver = FindObjectOfType<TilemapSaver>();
        if (saver == null || saver.targetTilemapToPaint == null || saver.prefabToSaveConteiner == null)
        {
            Debug.LogError("TilemapSaver не найден или Tilemap/ObjectParent не указаны!");
            return;
        }
        saver.objectsContainer = saver.targetTilemapToPaint.GetComponent<WallTilemap>();
        var walls = saver.objectsContainer.items;
        Tilemap tilemapToPaint = saver.targetTilemapToPaint;

        TilemapData tilemapData = ScriptableObject.CreateInstance<TilemapData>();

        List<TileData> tileDataList = new List<TileData>();

        tilemapData.Wallprefabs = new List<PrefabData>(walls);

        List<TileData> tileDataListToPaint = new List<TileData>();

        BoundsInt boundsToPaint = tilemapToPaint.cellBounds;
        TileBase[] allTilesToPaint = tilemapToPaint.GetTilesBlock(boundsToPaint);

        for (int x = boundsToPaint.xMin; x < boundsToPaint.xMax; x++)
        {
            for (int y = boundsToPaint.yMin; y < boundsToPaint.yMax; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                TileBase tile = tilemapToPaint.GetTile(position);

                if (tile != null)
                {
                    TileData tileData = new TileData
                    {
                        position = position,
                        tileName = tile.name
                    };
                    tileDataListToPaint.Add(tileData);
                }
            }
        }

        List<TileBase> paletteTiles = new List<TileBase>();

        paletteTiles.AddRange(tilemapToPaint.GetTilesBlock(tilemapToPaint.cellBounds));

        TileBase[] paintTiles = tilemapToPaint.GetTilesBlock(tilemapToPaint.cellBounds);
        foreach (TileBase tile in paintTiles)
        {
            if (tile != null && !paletteTiles.Contains(tile))
            {
                paletteTiles.Add(tile);
            }
        }

        tilemapData.tilesPaliter = paletteTiles.ToArray();

        tilemapData.tilesToPaint = tileDataListToPaint.ToArray();



        List<PrefabData> prefabDataList = new List<PrefabData>();
        foreach (Transform child in saver.prefabToSaveConteiner.transform)
        {
            GameObject prefab = child.gameObject;
            if (prefab != null)
            {
                PrefabData prefabData = new PrefabData
                {
                    prefabType = prefab.GetComponent<InteractableObject>().typeObject,
                    position = prefab.transform.position
                };
                prefabDataList.Add(prefabData);
            }
        }


        tilemapData.prefabs = prefabDataList;

        string path = EditorUtility.SaveFilePanelInProject("Save Tilemap and Objects Data", "TilemapWithObjectsData", "asset", "Введите имя файла для сохранения данных");
        if (path.Length > 0)
        {
            AssetDatabase.CreateAsset(tilemapData, path);
            AssetDatabase.SaveAssets();
            Debug.Log("Данные Tilemap и объектов успешно сохранены!");
        }
    }

    [MenuItem("Tools/SaveObjects")]
    public static void SaveObjects()
    {
        TilemapSaver saver = FindObjectOfType<TilemapSaver>();
        if (saver == null || saver.targetTilemapToPaint == null || saver.prefabToSaveConteiner == null)
        {
            Debug.LogError("TilemapSaver не найден или Tilemap/ObjectParent не указаны!");
            return;
        }
        List<PrefabData> prefabDataList = new List<PrefabData>();
        foreach (Transform child in saver.prefabToSaveConteiner.transform)
        {
            GameObject prefab = child.gameObject;
            if (prefab != null)
            {
                PrefabData prefabData = new PrefabData
                {
                    prefabType = prefab.GetComponent<InteractableObject>().typeObject,
                    position = prefab.transform.position
                };
                prefabDataList.Add(prefabData);
            }
        }
        saver.tilemapData.prefabs = prefabDataList;

    }
    [MenuItem("Tools/Clear Tilemap")]
    public static void ClearTileMap()
    {
        TilemapSaver saver = FindObjectOfType<TilemapSaver>();
        if (saver == null || saver.targetTilemapToPaint == null)
        {
            Debug.LogError("TilemapSaver не найден или Tilemap/ObjectParent не указаны!");
            return;
        }

        saver.objectsContainer = saver.targetTilemapToPaint.GetComponent<WallTilemap>();
        var walls = saver.objectsContainer.items;
        walls.Clear();
        for (int i = saver.objectsContainer.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = saver.objectsContainer.transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }
        Tilemap tilemapToPaint = saver.targetTilemapToPaint;
        tilemapToPaint.ClearAllTiles();


        foreach (Transform child in saver.prefabToSaveConteiner)
            DestroyImmediate(child.gameObject);

    }
    public TilemapData tilemapData;
    [MenuItem("Tools/Load Tilemap")]
    public static void LoadTilemap()
    {
        TilemapSaver saver = FindObjectOfType<TilemapSaver>();
        if (saver == null || saver.targetTilemapToPaint == null)
        {
            Debug.LogError("TilemapSaver не найден или Tilemap/ObjectParent не указаны!");
            return;
        }

        if (saver.tilemapData == null)
        {
            Debug.LogError("TilemapData или Tilemap не указаны!");
            return;
        }

        saver.objectsContainer = saver.targetTilemapToPaint.GetComponent<WallTilemap>();
        var walls = saver.objectsContainer.items;
        walls.Clear();
        for (int i = saver.objectsContainer.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = saver.objectsContainer.transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }

        foreach (PrefabData prefabData in saver.tilemapData.Wallprefabs)
        {
            GameObject prefabToInstantiate = saver.allPrefabs.Find(prefab => prefab.name == prefabData.name);
            if (prefabToInstantiate != null)
            {
                var item = Instantiate(prefabToInstantiate, prefabData.position, Quaternion.identity, saver.objectsContainer.transform);
                item.name = prefabData.name;
            }
            else
            {
                Debug.LogError($"Префаб типа '{prefabData.prefabType}' не найден!");
            }
        }


        foreach (TileData tileData in saver.tilemapData.tilesToPaint)
        {
            TileBase tileToPlace = GetTileFromPalette(tileData.tileName);

            if (tileToPlace != null)
            {
                saver.targetTilemapToPaint.SetTile(tileData.position, tileToPlace);
            }
            else
            {
                Debug.LogError($"Tile '{tileData.tileName}' не найден в палитре для окрашивания.");
            }
        }

        foreach (Transform child in saver.prefabToSaveConteiner)
            DestroyImmediate(child.gameObject);
        foreach (PrefabData prefabData in saver.tilemapData.prefabs)
        {
            GameObject prefabToInstantiate = saver.itemsPrefabs.Find
                (prefab => prefab.GetComponent<InteractableObject>().typeObject
                == prefabData.prefabType);
            if (prefabToInstantiate != null)
            {
                var item = Instantiate(prefabToInstantiate, prefabData.position, Quaternion.identity, saver.prefabToSaveConteiner);
                if (prefabData.prefabType != TypeObject.People)
                    item.transform.rotation = Quaternion.Euler(90, 0, 0);
            }
            else
            {
                Debug.LogError($"Префаб типа '{prefabData.prefabType}' не найден!");
            }
        }

        TileBase GetTileFromPalette(string tileName)
        {
            foreach (TileBase tile in saver.tilemapData.tilesPaliter)
            {
                if (tile != null && tile.name == tileName)
                {
                    return tile;
                }
            }
            return null;
        }

    }
}
#endif