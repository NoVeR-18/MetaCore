using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapLoader : MonoBehaviour
{
    public TilemapData tilemapData;
    public Tilemap tilemap;
    public Tilemap tilemapToPaint;
    public GameObject prefabContainer;
    public GameObject[] allPrefabs;

    void Start()
    {
        LoadTilemap();
    }

    void LoadTilemap()
    {
        if (tilemapData == null || tilemap == null)
        {
            Debug.LogError("TilemapData или Tilemap не указаны!");
            return;
        }

        tilemap.ClearAllTiles();

        foreach (TileData tileData in tilemapData.tiles)
        {
            TileBase tileToPlace = GetTileFromPalette(tileData.tileName);

            if (tileToPlace != null)
            {
                tilemap.SetTile(tileData.position, tileToPlace);
            }
            else
            {
                Debug.LogError($"Tile '{tileData.tileName}' не найден в палитре.");
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
                Debug.LogError($"Tile '{tileData.tileName}' не найден в палитре дл€ окрашивани€.");
            }
        }
        foreach (Transform child in prefabContainer.transform)
            Destroy(child.gameObject);
        foreach (PrefabData prefabData in tilemapData.prefabs)
        {
            GameObject prefabToInstantiate = System.Array.Find(allPrefabs, prefab => prefab.GetComponent<InteractableObject>().typeObject == prefabData.prefabType);
            if (prefabToInstantiate != null)
            {
                Instantiate(prefabToInstantiate, prefabData.position, Quaternion.identity, prefabContainer.transform);
            }
            else
            {
                Debug.LogError($"ѕрефаб типа '{prefabData.prefabType}' не найден!");
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
