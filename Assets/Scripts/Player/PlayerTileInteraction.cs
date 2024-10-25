using System.Collections;
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
        tilesToPaint = LevelManager.Instance.Loader.tilemapData.tilesToPaint.Length;
        //CountTilesToPaint();
        Debug.Log("���������� ������ ��� ��������: " + tilesToPaint);
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


    void OnLevelCompleted()
    {
        paintedTiles = 0;
        Debug.Log("������� �������!");

        StartCoroutine(timeToNextLevel());
    }
    private IEnumerator timeToNextLevel()
    {
        yield return new WaitForSeconds(0.5f);
        LevelManager.Instance.LevelComplete();
        tilesToPaint = LevelManager.Instance.Loader.tilemapData.tilesToPaint.Length;
    }
}
