using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IslandController : MonoBehaviour
{
    public Player.PlayerWallet playerWallet;
    public List<ExpansionTile> expansionTiles;
    public int islandLevel = 1; // ������� ������� �������

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

    // ��������� ��������� ����� � ����������� �� ������ �������
    public void UpdateTileVisibility()
    {
        foreach (ExpansionTile tile in expansionTiles)
        {
            tile.UpdateVisibility(islandLevel);
        }
    }

    // ����� ��� ������������� ������
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
            Debug.Log("������������ ������ ��� ������������� ������.");
        }
    }

    // ���������, �������������� �� ��� ������ �������� ������
    private void CheckAndIncreaseIslandLevel()
    {
        foreach (ExpansionTile tile in expansionTiles)
        {
            if (tile.RequiredLevel == islandLevel && !tile.IsUnlocked)
            {
                return; // ���� ���� ������ �������� ������, ������� �� ��������������, �� �������
            }
        }

        // ���� ��� ������ �������� ������ ��������������, ����������� ������� �������
        islandLevel++;
        Debug.Log($"������� ������� ������� �� {islandLevel}");
        UpdateTileVisibility();
    }

    public void UnlockBuilding(ExpansionTile tile)
    {
        if (playerWallet.WithdrawCrystal(crystalCost)) // ����� ��� ������ ����������
        {
            tile.BuildHouse(); // �������� ����� ��� ��������� ����

            SaveIslandState();
        }
        else
        {
            Debug.Log("������������ ���������� ��� ��������� ������.");
        }
    }
    private const string IslandLevelKey = "IslandLevel";
    private const string TileStateKeyPrefix = "TileState_"; // ������� ��� ��������� �����

    public void SaveIslandState()
    {
        PlayerPrefs.SetInt(IslandLevelKey, islandLevel);

        for (int i = 0; i < expansionTiles.Count; i++)
        {
            ExpansionTile tile = expansionTiles[i];
            string key = TileStateKeyPrefix + i;

            // ��������� ��������� ������ � �������: IsUnlocked,HasHouse
            int state = tile.IsUnlocked ? 1 : 0;
            state |= (tile.currentHouse != null ? 2 : 0); // ���� ��� ��������, ��������� 2 � ���������
            PlayerPrefs.SetInt(key, state);
        }

        PlayerPrefs.Save();
    }

    public void LoadIslandState()
    {
        islandLevel = PlayerPrefs.GetInt(IslandLevelKey, 1); // ��������� ������� ������� ��� 1, ���� �� �������

        for (int i = 0; i < expansionTiles.Count; i++)
        {
            ExpansionTile tile = expansionTiles[i];
            string key = TileStateKeyPrefix + i;

            int state = PlayerPrefs.GetInt(key, 0);

            tile.IsUnlocked = (state & 1) != 0; // �������������� �� ������
            if (tile.IsUnlocked)
            {
                tile.Unlock();
            }

            bool hasHouse = (state & 2) != 0; // �������� �� ���
            if (hasHouse && tile.canBuildHouse)
            {
                tile.BuildHouse(); // ��������������� ���
            }
        }

        UpdateTileVisibility();
    }
    private void OnApplicationQuit()
    {
        SaveIslandState();
    }
}
