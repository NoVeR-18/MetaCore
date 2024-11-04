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
        }
        else
        {
            Debug.Log("������������ ���������� ��� ��������� ������.");
        }
    }
}
