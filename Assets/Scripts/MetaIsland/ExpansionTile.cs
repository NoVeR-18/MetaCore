using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExpansionTile : MonoBehaviour
{
    private IslandController islandController;
    public int RequiredLevel = 1;
    [SerializeField] public bool IsUnlocked = false;

    [SerializeField] private GameObject buyableIndicator;
    [SerializeField] private GameObject currentRoad;

    public ExpansionTile topNeighbor;
    public ExpansionTile bottomNeighbor;
    public ExpansionTile leftNeighbor;
    public ExpansionTile rightNeighbor;

    public bool canBuildHouse = false;
    [SerializeField] private GameObject buildableIndicator;
    public GameObject currentHouse;

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
            buildableIndicator.SetActive(false);
            currentRoad.SetActive(false);
        }
        else if (currentIslandLevel == RequiredLevel && !IsUnlocked)
        {
            buyableIndicator.SetActive(true);
            buildableIndicator.SetActive(false);
            currentRoad.SetActive(false);
            gameObject.SetActive(true);
        }
        else
        {
            currentRoad.SetActive(true);
            buyableIndicator.SetActive(false);
            buildableIndicator.SetActive(canBuildHouse && !currentHouse.activeSelf);
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
        if (currentRoad != null)
        {
            Destroy(currentRoad);
        }

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
        else if (top || bottom || left || right)
        {
            selectedRoadType = RoadType.End;
        }
        else
            selectedRoadType = RoadType.Free;

        if (roadPrefabs.TryGetValue(selectedRoadType, out GameObject prefab))
        {
            currentRoad = Instantiate(prefab, transform.position, Quaternion.identity, transform);

            if (selectedRoadType == RoadType.Straight && (left && right))
                currentRoad.transform.rotation = Quaternion.Euler(0, 0, 0);
            else if (selectedRoadType == RoadType.Straight && (top && bottom)) currentRoad.transform.rotation = Quaternion.Euler(0, 90, 0);
            else if (selectedRoadType == RoadType.Corner)
            {
                if (bottom && right) currentRoad.transform.rotation = Quaternion.Euler(0, -90, 0);
                else if (top && right) currentRoad.transform.rotation = Quaternion.Euler(0, 180, 0);
                else if (bottom && left) currentRoad.transform.rotation = Quaternion.Euler(0, 0, 0);
                else if (top && left) currentRoad.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
            else if (selectedRoadType == RoadType.TJunction)
            {
                if (bottom && right && top) currentRoad.transform.rotation = Quaternion.Euler(0, 180, 0);
                else if (top && right && left) currentRoad.transform.rotation = Quaternion.Euler(0, 90, 0);
                else if (bottom && left && right) currentRoad.transform.rotation = Quaternion.Euler(0, -90, 0);
                else currentRoad.transform.rotation = Quaternion.Euler(0, 0, 0);

            }
            else if (selectedRoadType == RoadType.End)
            {
                if (bottom) currentRoad.transform.rotation = Quaternion.Euler(0, 90, 0);
                else if (top) currentRoad.transform.rotation = Quaternion.Euler(0, -90, 0);
                else if (right) currentRoad.transform.rotation = Quaternion.Euler(0, 0, 0);
                else if (left) currentRoad.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    public void BuildHouse()
    {
        if (!canBuildHouse)
        {
            Debug.Log("На этой ячейке нельзя строить домик.");
            return;
        }

        if (currentHouse != null)
        {
            currentHouse.SetActive(true);
            Debug.Log($"Домик построен на {transform.position}.");
            buildableIndicator.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (!IsUnlocked && buyableIndicator.activeSelf)
        {
            islandController.UnlockTile(this);
        }
        else if (IsUnlocked && canBuildHouse && buildableIndicator.activeSelf)
        {
            islandController.UnlockBuilding(this);
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
    Free,
    End,
    Straight,
    Corner,
    TJunction,
    Cross
}