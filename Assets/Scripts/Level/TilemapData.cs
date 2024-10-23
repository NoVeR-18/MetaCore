using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TilemapData", menuName = "Levels/New Tilemap")]
public class TilemapData : ScriptableObject
{
    public TileData[] tiles;
    public TileData[] tilesToPaint;
    public TileBase[] tilesPaliter;
    public List<PrefabData> prefabs;
}
[System.Serializable]
public class PrefabData
{
    public TypeObject prefabType;
    public Vector3 position;
}

[System.Serializable]
public class TileData
{
    public Vector3Int position;
    public string tileName;
}
