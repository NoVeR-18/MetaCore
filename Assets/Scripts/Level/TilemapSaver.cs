using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSaver : MonoBehaviour
{
    public Tilemap targetTilemap;
    public Tilemap targetTilemapToPaint;
    public GameObject prefabToSaveConteiner;

    [MenuItem("Tools/Save Tilemap")]
    public static void SaveTilemap()
    {
        TilemapSaver saver = FindObjectOfType<TilemapSaver>();
        if (saver == null || saver.targetTilemap == null || saver.targetTilemapToPaint == null || saver.prefabToSaveConteiner == null)
        {
            Debug.LogError("TilemapSaver не найден или Tilemap/ObjectParent не указаны!");
            return;
        }
        Tilemap tilemap = saver.targetTilemap;
        Tilemap tilemapToPaint = saver.targetTilemapToPaint;

        TilemapData tilemapData = ScriptableObject.CreateInstance<TilemapData>();

        List<TileData> tileDataList = new List<TileData>();

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(position);

                if (tile != null)
                {
                    TileData tileData = new TileData
                    {
                        position = position,
                        tileName = tile.name
                    };
                    tileDataList.Add(tileData);
                }
            }
        }

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

        paletteTiles.AddRange(tilemap.GetTilesBlock(tilemap.cellBounds));

        TileBase[] paintTiles = tilemapToPaint.GetTilesBlock(tilemapToPaint.cellBounds);
        foreach (TileBase tile in paintTiles)
        {
            if (tile != null && !paletteTiles.Contains(tile))
            {
                paletteTiles.Add(tile);
            }
        }

        tilemapData.tilesPaliter = paletteTiles.ToArray();

        tilemapData.tiles = tileDataList.ToArray();
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

        string path = EditorUtility.SaveFilePanelInProject("Save Tilemap and Objects Data", "TilemapWithObjectsData", "asset", "¬ведите им€ файла дл€ сохранени€ данных");
        if (path.Length > 0)
        {
            AssetDatabase.CreateAsset(tilemapData, path);
            AssetDatabase.SaveAssets();
            Debug.Log("ƒанные Tilemap и объектов успешно сохранены!");
        }
    }
    [MenuItem("Tools/Clear Tilemap")]
    public static void ClearTileMap()
    {
        TilemapSaver saver = FindObjectOfType<TilemapSaver>();
        if (saver == null || saver.targetTilemap == null || saver.targetTilemapToPaint == null)
        {
            Debug.LogError("TilemapSaver не найден или Tilemap/ObjectParent не указаны!");
            return;
        }
        Tilemap tilemap = saver.targetTilemap;
        Tilemap tilemapToPaint = saver.targetTilemapToPaint;
        tilemap.ClearAllTiles();
        tilemapToPaint.ClearAllTiles();

    }
    public TilemapData tilemapData;
    [MenuItem("Tools/Load Tilemap")]
    public static void LoadTilemap()
    {
        TilemapSaver saver = FindObjectOfType<TilemapSaver>();
        if (saver == null || saver.targetTilemap == null || saver.targetTilemapToPaint == null)
        {
            Debug.LogError("TilemapSaver не найден или Tilemap/ObjectParent не указаны!");
            return;
        }

        if (saver.tilemapData == null || saver.targetTilemap == null)
        {
            Debug.LogError("TilemapData или Tilemap не указаны!");
            return;
        }

        saver.targetTilemap.ClearAllTiles();

        foreach (TileData tileData in saver.tilemapData.tiles)
        {
            TileBase tileToPlace = GetTileFromPalette(tileData.tileName);

            if (tileToPlace != null)
            {
                saver.targetTilemap.SetTile(tileData.position, tileToPlace);
            }
            else
            {
                Debug.LogError($"Tile '{tileData.tileName}' не найден в палитре.");
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
                Debug.LogError($"Tile '{tileData.tileName}' не найден в палитре дл€ окрашивани€.");
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
