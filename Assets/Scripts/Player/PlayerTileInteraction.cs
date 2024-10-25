using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerTileInteraction : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase newTile;
    [SerializeField] private int tilesToPaint;
    private int paintedTiles = 0;

    void Start()
    {
        tilesToPaint = CountTilesToPaint();
        Debug.Log("Количество тайлов для закраски: " + tilesToPaint);
    }

    void Update()
    {
        Vector3Int tilePosition = tilemap.WorldToCell(new Vector3(transform.position.x, 0.5f, transform.position.z));
        TileBase currentTile = tilemap.GetTile(tilePosition);

        if (currentTile != null && currentTile != newTile)
        {
            tilemap.SetTile(tilePosition, newTile);
            paintedTiles++;

            if (paintedTiles >= tilesToPaint)
            {
                OnLevelCompleted();
            }
        }
    }

    int CountTilesToPaint()
    {
        TileBase[] allTiles = new TileBase[tilemap.GetUsedTilesCount()];
        tilemap.GetUsedTilesNonAlloc(allTiles);

        int count = 0;

        foreach (TileBase tile in allTiles)
        {
            if (tile != null && tile != newTile)
            {
                count++;
            }
        }

        return count;
    }

    void OnLevelCompleted()
    {
        Debug.Log("Уровень пройден!");
    }
}
