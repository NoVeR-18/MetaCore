using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IslandController : MonoBehaviour
{
    public Player.PlayerWallet playerWallet;
    public LevelProgressManager levelProgressManager;
    public List<ExpansionTile> expansionTiles;
    public int islandLevel = 1;


    [SerializeField] private Button CheatButton;

    [Space]
    public AudioSource audioSource;
    public AudioClip unlockTileSound;
    public AudioClip buyHouseSound;

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
        levelProgressManager.UpLevel += UpdateTileVisibility;
    }

    private void InitializeExpansionTiles()
    {
        foreach (ExpansionTile tile in expansionTiles)
        {
            tile.SetIslandController(this);
        }
    }

    public void UpdateTileVisibility()
    {
        foreach (ExpansionTile tile in expansionTiles)
        {
            tile.UpdateVisibility(islandLevel);
        }
    }

    public void UnlockTile(ExpansionTile tile)
    {
        if (playerWallet.WithdrawMoney(tile.GoldCost))
        {
            tile.Unlock();
            audioSource?.PlayOneShot(unlockTileSound);
            CheckAndIncreaseIslandLevel();

            SaveIslandState();
        }
        else
        {
            Debug.Log("Недостаточно золота для разблокировки ячейки.");
        }
    }

    private void CheckAndIncreaseIslandLevel()
    {
        foreach (ExpansionTile tile in expansionTiles)
        {
            if (tile.RequiredLevel == islandLevel && !tile.IsUnlocked)
            {
                return;
            }
        }

        islandLevel++;
        Debug.Log($"Уровень острова повышен до {islandLevel}");
        UpdateTileVisibility();
    }

    public void UnlockBuilding(ExpansionTile tile)
    {
        if (playerWallet.WithdrawCrystal(tile.CrystalCost))
        {
            tile.BuildHouse();

            SaveIslandState();
            audioSource?.PlayOneShot(buyHouseSound);
        }
        else
        {
            Debug.Log("Недостаточно кристаллов для постройки домика.");
        }
    }
    private const string IslandLevelKey = "IslandLevel";
    private const string TileStateKeyPrefix = "TileState_";

    public void SaveIslandState()
    {
        PlayerPrefs.SetInt(IslandLevelKey, islandLevel);

        for (int i = 0; i < expansionTiles.Count; i++)
        {
            ExpansionTile tile = expansionTiles[i];
            string key = TileStateKeyPrefix + i;

            int state = tile.IsUnlocked ? 1 : 0;
            state |= (tile.currentHouse.gameObject.activeSelf ? 2 : 0);
            PlayerPrefs.SetInt(key, state);
        }

        PlayerPrefs.Save();
    }

    public void LoadIslandState()
    {
        islandLevel = PlayerPrefs.GetInt(IslandLevelKey, 1);
        Debug.Log("Island level:" + islandLevel);
        for (int i = 0; i < expansionTiles.Count; i++)
        {
            ExpansionTile tile = expansionTiles[i];
            string key = TileStateKeyPrefix + i;

            int state = PlayerPrefs.GetInt(key, 0);

            tile.IsUnlocked = (state & 1) != 0;
            if (tile.IsUnlocked)
            {
                tile.Unlock();
            }
            tile.UpdateVisibility(islandLevel);
            bool hasHouse = (state & 2) != 0;
            if (hasHouse && tile.canBuildHouse)
            {
                tile.BuildHouse();
            }
        }

        UpdateTileVisibility();
    }
    private void OnApplicationQuit()
    {
        SaveIslandState();
        YsoCorp.GameUtils.YCManager.instance.OnGameFinished(true);
    }
}
