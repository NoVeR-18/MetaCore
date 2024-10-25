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

    void Start()
    {
        LoadTilemap();
    }

    //void LoadTilemap()
    //{
    //    if (tilemapData == null || tilemap == null)
    //    {
    //        Debug.LogError("TilemapData или Tilemap не указаны!");
    //        return;
    //    }

    //    tilemap.ClearAllTiles();

    //    foreach (TileData tileData in tilemapData.tiles)
    //    {
    //        TileBase tileToPlace = GetTileFromPalette(tileData.tileName);

    //        if (tileToPlace != null)
    //        {
    //            tilemap.SetTile(tileData.position, tileToPlace);
    //        }
    //        else
    //        {
    //            Debug.LogError($"Tile '{tileData.tileName}' не найден в палитре.");
    //        }
    //    }
    //    foreach (TileData tileData in tilemapData.tilesToPaint)
    //    {
    //        TileBase tileToPlace = GetTileFromPalette(tileData.tileName);

    //        if (tileToPlace != null)
    //        {
    //            tilemapToPaint.SetTile(tileData.position, tileToPlace);
    //        }
    //        else
    //        {
    //            Debug.LogError($"Tile '{tileData.tileName}' не найден в палитре для окрашивания.");
    //        }
    //    }
    //    foreach (Transform child in itemsPrefabContainer.transform)
    //        Destroy(child.gameObject);
    //    foreach (PrefabData prefabData in tilemapData.prefabs)
    //    {
    //        GameObject prefabToInstantiate = System.Array.Find(allPrefabs, prefab => prefab.GetComponent<InteractableObject>().typeObject == prefabData.prefabType);
    //        if (prefabToInstantiate != null)
    //        {
    //            Instantiate(prefabToInstantiate, prefabData.position, Quaternion.identity, itemsPrefabContainer.transform);
    //        }
    //        else
    //        {
    //            Debug.LogError($"Префаб типа '{prefabData.prefabType}' не найден!");
    //        }
    //    }
    //}

    public void LoadTilemap()
    {
        if (tilemapData == null || objectsContainer == null)
        {
            Debug.LogError("TilemapData или Tilemap не указаны!");
            return;
        }

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
                var item = Instantiate(prefabToInstantiate, prefabData.position, Quaternion.identity, objectsContainer.transform);
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
                if (prefabData.prefabType != TypeObject.People)
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

}
