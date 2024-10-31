using System.Collections.Generic;
using UnityEngine;

public class IslandController : MonoBehaviour
{
    public Player.PlayerWallet playerWallet;
    public List<ExpansionTile> expansionTiles;
    public int islandLevel = 1; // Текущий уровень острова

    [SerializeField] private int crystalCost = 50;
    [SerializeField] private int goldCost = 100;

    private void Start()
    {
        InitializeExpansionTiles();
        UpdateTileVisibility();
        playerWallet.AddMoney(100);
    }

    private void InitializeExpansionTiles()
    {
        foreach (ExpansionTile tile in expansionTiles)
        {
            tile.SetIslandController(this);
        }
    }

    // Обновляем видимость ячеек в зависимости от уровня острова
    public void UpdateTileVisibility()
    {
        foreach (ExpansionTile tile in expansionTiles)
        {
            tile.UpdateVisibility(islandLevel);
        }
    }

    // Метод для разблокировки ячейки
    public void UnlockTile(ExpansionTile tile)
    {
        if (playerWallet.WithdrawMoney(goldCost))
        {
            tile.Unlock();
            CheckAndIncreaseIslandLevel();
        }
        else
        {
            Debug.Log("Недостаточно золота для разблокировки ячейки.");
        }
    }

    // Проверяем, разблокированы ли все ячейки текущего уровня
    private void CheckAndIncreaseIslandLevel()
    {
        foreach (ExpansionTile tile in expansionTiles)
        {
            if (tile.RequiredLevel == islandLevel && !tile.IsUnlocked)
            {
                return; // Если есть ячейка текущего уровня, которая не разблокирована, то выходим
            }
        }

        // Если все ячейки текущего уровня разблокированы, увеличиваем уровень острова
        islandLevel++;
        Debug.Log($"Уровень острова повышен до {islandLevel}");
        UpdateTileVisibility();
    }
}
