using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IslandController : MonoBehaviour
{
    public Player.PlayerWallet playerWallet;
    public List<ExpansionTile> expansionTiles;
    public int islandLevel = 1; // Текущий уровень острова

    [SerializeField] private int crystalCost = 50;
    [SerializeField] private int goldCost = 100;
    [SerializeField] private Button CheatButton;
    private void Start()
    {
        InitializeExpansionTiles();
        UpdateTileVisibility();
        LoadIslandState();
        CheatButton?.onClick.AddListener(() =>
        {
            playerWallet.AddMoney(2000);
            playerWallet.AddCrystal(500);
        });
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

            SaveIslandState();
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

    public void UnlockBuilding(ExpansionTile tile)
    {
        if (playerWallet.WithdrawCrystal(crystalCost)) // метод для снятия кристаллов
        {
            tile.BuildHouse(); // вызываем метод для постройки дома

            SaveIslandState();
        }
        else
        {
            Debug.Log("Недостаточно кристаллов для постройки домика.");
        }
    }
    private const string IslandLevelKey = "IslandLevel";
    private const string TileStateKeyPrefix = "TileState_"; // Префикс для состояния ячеек

    public void SaveIslandState()
    {
        PlayerPrefs.SetInt(IslandLevelKey, islandLevel);

        for (int i = 0; i < expansionTiles.Count; i++)
        {
            ExpansionTile tile = expansionTiles[i];
            string key = TileStateKeyPrefix + i;

            // Сохраняем состояние ячейки в формате: IsUnlocked,HasHouse
            int state = tile.IsUnlocked ? 1 : 0;
            state |= (tile.currentHouse != null ? 2 : 0); // Если дом построен, добавляем 2 к состоянию
            PlayerPrefs.SetInt(key, state);
        }

        PlayerPrefs.Save();
    }

    public void LoadIslandState()
    {
        islandLevel = PlayerPrefs.GetInt(IslandLevelKey, 1); // Загружаем уровень острова или 1, если не сохранён

        for (int i = 0; i < expansionTiles.Count; i++)
        {
            ExpansionTile tile = expansionTiles[i];
            string key = TileStateKeyPrefix + i;

            int state = PlayerPrefs.GetInt(key, 0);

            tile.IsUnlocked = (state & 1) != 0; // Разблокирована ли ячейка
            if (tile.IsUnlocked)
            {
                tile.Unlock();
            }

            bool hasHouse = (state & 2) != 0; // Построен ли дом
            if (hasHouse && tile.canBuildHouse)
            {
                tile.BuildHouse(); // Восстанавливаем дом
            }
        }

        UpdateTileVisibility();
    }
    private void OnApplicationQuit()
    {
        SaveIslandState();
    }
}
