using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExperienceTable", menuName = "Game Data/Experience Table")]
public class ExperienceTable : ScriptableObject
{
    public List<int> experiencePerLevel; // Опыт для каждого уровня

    public List<Vector2Int> incomeCost;

    public List<CapacityCost> capacityCost;

    public List<int> TileCost;

    public List<int> HouseBuildCost;
}
[System.Serializable]
public class CapacityCost
{
    public int PlaceCount;
    public int CostUpgrade;
    public int minLevelToUpgrade;
    public int AwardStarsCount;
}

