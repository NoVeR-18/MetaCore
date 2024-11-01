using System.Collections.Generic;
using UnityEngine;

public class ExpansionTile : MonoBehaviour
{
    private IslandController islandController;
    public int RequiredLevel = 1; // Уровень, необходимый для разблокировки этой ячейки
    [SerializeField] public bool IsUnlocked = false;

    [SerializeField] private GameObject buyableIndicator;
    [SerializeField] private GameObject road;

    public ExpansionTile topNeighbor;
    public ExpansionTile bottomNeighbor;
    public ExpansionTile leftNeighbor;
    public ExpansionTile rightNeighbor;

    private GameObject currentRoad;


    public ExpansionTile(ExpansionTile tile)
    {
    }

    public List<RoadTile> roadTiles;
    [SerializeField] private Dictionary<RoadType, GameObject> roadPrefabs = new Dictionary<RoadType, GameObject>();
    private void Awake()
    {
        roadPrefabs = new Dictionary<RoadType, GameObject>();
        foreach (var tile in roadTiles)
        {
            roadPrefabs[tile.key] = tile.prefab;
        }
    }

    public void SetIslandController(IslandController controller)
    {
        islandController = controller;
    }

    public void UpdateVisibility(int currentIslandLevel)
    {
        if (currentIslandLevel < RequiredLevel)
        {
            buyableIndicator.SetActive(false);
            road.SetActive(false);
            gameObject.SetActive(false);
        }
        else if (currentIslandLevel == RequiredLevel && !IsUnlocked)
        {
            buyableIndicator.SetActive(true);
            road.SetActive(false);
            gameObject.SetActive(true);
        }
        else
        {
            road.SetActive(true);
            buyableIndicator.SetActive(false);
            IsUnlocked = true;
        }
    }

    public void Unlock()
    {
        IsUnlocked = true;
        buyableIndicator.SetActive(false);
        Debug.Log($"Ячейка на {transform.position} разблокирована!");

        UpdateRoadPrefab();

        if (topNeighbor != null)
            if (topNeighbor.IsUnlocked)
                topNeighbor.UpdateRoadPrefab();
        if (bottomNeighbor != null)
            if (bottomNeighbor.IsUnlocked)
                bottomNeighbor.UpdateRoadPrefab();
        if (leftNeighbor != null)
            if (leftNeighbor.IsUnlocked)
                leftNeighbor.UpdateRoadPrefab();
        if (rightNeighbor != null)
            if (rightNeighbor.IsUnlocked)
                rightNeighbor.UpdateRoadPrefab();
    }

    public void UpdateRoadPrefab()
    {
        //if (currentRoad != null)
        //{
        //    Destroy(currentRoad);
        //}

        bool top = topNeighbor != null && topNeighbor.IsUnlocked;
        bool bottom = bottomNeighbor != null && bottomNeighbor.IsUnlocked;
        bool left = leftNeighbor != null && leftNeighbor.IsUnlocked;
        bool right = rightNeighbor != null && rightNeighbor.IsUnlocked;

        RoadType selectedRoadType;

        if (top && bottom && left && right)
        {
            selectedRoadType = RoadType.Cross;
        }
        else if ((top && bottom && left) || (top && bottom && right) || (top && left && right) || (bottom && left && right))
        {
            selectedRoadType = RoadType.TJunction;
        }
        else if ((top && bottom) || (left && right))
        {
            selectedRoadType = RoadType.Straight;
        }
        else if ((top && left) || (top && right) || (bottom && left) || (bottom && right))
        {
            selectedRoadType = RoadType.Corner;
        }
        else
        {
            selectedRoadType = RoadType.End;
        }

        // Устанавливаем префаб из словаря в зависимости от выбранного типа дороги
        if (roadPrefabs.TryGetValue(selectedRoadType, out GameObject prefab))
        {
            currentRoad = Instantiate(prefab, transform.position, Quaternion.identity, transform);

            // Поворачиваем префаб в зависимости от конфигурации соседей
            if (selectedRoadType == RoadType.Straight && (left && right))
                currentRoad.transform.rotation = Quaternion.Euler(0, 90, 0);
            else if (selectedRoadType == RoadType.Corner)
            {
                if (bottom && right) currentRoad.transform.rotation = Quaternion.Euler(0, 180, 0);
                else if (top && right) currentRoad.transform.rotation = Quaternion.Euler(0, 90, 0);
                else if (bottom && left) currentRoad.transform.rotation = Quaternion.Euler(0, -90, 0);
            }
            else if (selectedRoadType == RoadType.TJunction)
            {
                if (bottom && right && top) currentRoad.transform.rotation = Quaternion.Euler(0, 180, 0);
                else if (top && right && left) currentRoad.transform.rotation = Quaternion.Euler(0, 90, 0);
                else if (bottom && left && right) currentRoad.transform.rotation = Quaternion.Euler(0, -90, 0);
                else currentRoad.transform.rotation = Quaternion.identity;

            }
            else if (selectedRoadType == RoadType.End)
            {
                if (bottom) currentRoad.transform.rotation = Quaternion.Euler(0, 180, 0);
                else if (top) currentRoad.transform.rotation = Quaternion.identity;
                else if (right) currentRoad.transform.rotation = Quaternion.Euler(0, 90, 0);
                else if (left) currentRoad.transform.rotation = Quaternion.Euler(0, -90, 0);
            }
            var tile = currentRoad.GetComponent<ExpansionTile>();
            tile.topNeighbor = topNeighbor;
            tile.leftNeighbor = leftNeighbor;
            tile.rightNeighbor = rightNeighbor;
            tile.bottomNeighbor = bottomNeighbor;
            if (tile.topNeighbor != null) tile.topNeighbor.bottomNeighbor = tile;
            if (tile.bottomNeighbor != null) tile.bottomNeighbor.topNeighbor = tile;
            if (tile.leftNeighbor != null) tile.leftNeighbor.rightNeighbor = tile;
            if (tile.rightNeighbor != null) tile.rightNeighbor.leftNeighbor = tile;


            tile.RequiredLevel = RequiredLevel - 1;

            tile.SetIslandController(islandController);
            tile.UpdateVisibility(islandController.islandLevel);
            islandController.expansionTiles.Remove(this);
            islandController.expansionTiles.Add(tile);
            currentRoad.transform.parent = gameObject.transform.parent;
            Destroy(gameObject);
            //gameObject.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        if (!IsUnlocked && buyableIndicator.activeSelf)
        {
            islandController.UnlockTile(this);
        }
        else
        {
            Debug.Log("Ячейка недоступна для покупки.");
        }
    }
}

[System.Serializable]
public class RoadTile
{
    public RoadType key;
    public GameObject prefab;
}

[System.Serializable]
public enum RoadType
{
    End,
    Straight,
    Corner,
    TJunction,
    Cross
}