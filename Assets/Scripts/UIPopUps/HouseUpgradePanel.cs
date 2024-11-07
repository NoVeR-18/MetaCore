using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HouseUpgradePanel : MonoBehaviour
{
    public Player.PlayerWallet playerWallet;
    public LevelProgressManager levelProgressManager;
    [SerializeField] private int goldCost = 15;
    public House currentHouse;                    // ������� ��������� �����
    public Button upgradeRentButton;              // ������ ��� ��������� ������
    public Button upgradeCapacityButton;          // ������ ��� ��������� �����������
    public Button closeButton;                    // ������ ��� �������� ������
    public TextMeshProUGUI rentPriceText;         // �����, ������������ ���� ������
    public TextMeshProUGUI capacityText;          // �����, ������������ �����������
    public TextMeshProUGUI levelText;             // �����, ������������ ������� ������
    public TextMeshProUGUI maxLevelText;          // �����, ������������ ������������ ������� ������
    public TextMeshProUGUI currentCapacityText;          // �����, ������������ ������������ ������� ������
    public TextMeshProUGUI rentPriceUpdateText;          // �����, ������������ ������������ ������� ������
    public TextMeshProUGUI rentPriceAwardText;          // �����, ������������ ������������ ������� ������
    public TextMeshProUGUI capacityPriceAwardText;          // �����, ������������ ������������ ������� ������
    public TextMeshProUGUI capacityPriceUpdateText;          // �����, ������������ ������������ ������� ������
    public Image levelProgressBar;                // ��������-��� ��� ����������� ������
    public ExperienceTable experienceTable;

    private void Start()
    {
        UpdatePanel();
        closeButton.onClick.AddListener(() => { CloseTab(); });

        upgradeRentButton.onClick.AddListener(() => { OnUpgradeRentButton(); });
        upgradeCapacityButton.onClick.AddListener(() => { OnUpgradeCapacityButton(); });
        if (experienceTable == null)
            experienceTable = Resources.Load<ExperienceTable>("ExperienceTable");
    }

    public void SetCurrentHouse(House house)
    {
        currentHouse = house;
        UpdatePanel();
    }

    public void OnUpgradeRentButton()
    {
        if (currentHouse != null)
        {
            if (playerWallet.WithdrawMoney(experienceTable.incomeCost[currentHouse.rentPriceLevel].x))
            {
                currentHouse.IncreaseRentPrice(); // ����������� ������ �� 0.1
                levelProgressManager.AddExperience(experienceTable.incomeCost[currentHouse.rentPriceLevel].y);
                UpdatePanel();
            }
            else
            {
                Debug.Log("������������ ������ ��� ��������� ������.");
            }
        }
    }

    public void OnUpgradeCapacityButton()
    {
        if (currentHouse != null)
        {
            var currentLevel = experienceTable.capacityCost[currentHouse.capacityLevel];
            if (playerWallet.WithdrawMoney(currentLevel.CostUpgrade))
            {
                currentHouse.capacityLevel++;
                currentHouse.IncreaseCapacity(currentLevel.PlaceCount);
                levelProgressManager.AddExperience(currentLevel.AwardStarsCount);
                UpdatePanel();
            }
            else
            {
                Debug.Log("������������ ������ ��� ��������� �����������.");
            }
        }
    }

    public void OnUpgradeHouseButton()
    {
        if (currentHouse != null)
        {
            currentHouse.UpgradeHouse(); // �������� ������� ������
            UpdatePanel();
        }
    }

    private void UpdatePanel()
    {
        // ���������� ��������� ����� �� ������ � ����������� � ������
        if (currentHouse != null)
        {
            rentPriceText.text = (1 + 0.01f * currentHouse.rentPriceLevel).ToString("F1"); // ����������� ���� ������
            capacityText.text = currentHouse.capacity.ToString();       // ����������� �����������
            levelText.text = $"LVL  {currentHouse.level}";          // ����������� �������� ������
            maxLevelText.text = $"LVL {currentHouse.maxLevel}";           // ����������� ������������� ������
            currentCapacityText.text = $"{currentHouse.currentResidents} / {currentHouse.capacity}";
            rentPriceUpdateText.text = $"{experienceTable.incomeCost[currentHouse.rentPriceLevel].x}";
            rentPriceAwardText.text = $"{experienceTable.incomeCost[currentHouse.rentPriceLevel].y}";
            capacityPriceUpdateText.text = $"{experienceTable.capacityCost[currentHouse.capacityLevel].CostUpgrade}";
            capacityPriceAwardText.text = $"{experienceTable.capacityCost[currentHouse.capacityLevel].AwardStarsCount}";
            // ���������� ��������-���� ������
            levelProgressBar.fillAmount = (float)currentHouse.level / currentHouse.maxLevel;
        }
    }

    public void CloseTab()
    {
        IslandManager.Instance.cameraController.ResetCamera();
        currentHouse = null;
        gameObject.SetActive(false);
    }
}
