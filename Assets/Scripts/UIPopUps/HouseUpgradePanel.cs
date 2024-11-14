using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HouseUpgradePanel : MonoBehaviour
{
    public LevelProgressManager levelProgressManager;
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
    public Transform CapacityUpgradeBlock;
    public Transform rentUpgradeBlock;
    public Transform rentUpgradeMax;
    public TextMeshProUGUI CapacityUpgradeText;
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
            if (IslandManager.Instance.playerWallet.WithdrawMoney(experienceTable.incomeCost[currentHouse.rentPriceLevel].x))
            {
                currentHouse.IncreaseRentPrice(); // ����������� ������ �� 0.1
                levelProgressManager.AddExperience(experienceTable.incomeCost[currentHouse.rentPriceLevel].y);
                OnUpgradeHouseButton();
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
            if (IslandManager.Instance.playerWallet.WithdrawMoney(currentLevel.CostUpgrade))
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
            maxLevelText.text = $"LVL {experienceTable.incomeCost.Count}";           // ����������� ������������� ������
            currentCapacityText.text = $"{currentHouse.currentResidents} / {currentHouse.capacity}";
            rentPriceUpdateText.text = $"{experienceTable.incomeCost[currentHouse.rentPriceLevel].x}";
            rentPriceAwardText.text = $"{experienceTable.incomeCost[currentHouse.rentPriceLevel].y}";
            capacityPriceUpdateText.text = $"{experienceTable.capacityCost[currentHouse.capacityLevel].CostUpgrade}";
            capacityPriceAwardText.text = $"{experienceTable.capacityCost[currentHouse.capacityLevel].AwardStarsCount}";
            // ���������� ��������-���� ������
            levelProgressBar.fillAmount = (float)currentHouse.level / experienceTable.incomeCost.Count;

            if (currentHouse.level >= experienceTable.incomeCost.Count)
            {
                rentUpgradeMax.gameObject.SetActive(true);

            }
            else
                rentUpgradeMax.gameObject.SetActive(false);
            if (!IslandManager.Instance.playerWallet.CanWithdrawMoney(experienceTable.incomeCost[currentHouse.rentPriceLevel].x))
            {
                rentUpgradeBlock.gameObject.SetActive(true);
            }
            else
            {
                rentUpgradeBlock.gameObject.SetActive(false);
            }
            var currentCost = experienceTable.capacityCost[currentHouse.capacityLevel];
            if (currentHouse.level + 1 <= currentCost.minLevelToUpgrade)
            {
                CapacityUpgradeText.text = $"Level {experienceTable.capacityCost[currentHouse.capacityLevel].minLevelToUpgrade} needed";
                CapacityUpgradeBlock.gameObject.SetActive(true);
                CapacityUpgradeText.color = Color.red;
            }
            else if (!IslandManager.Instance.playerWallet.CanWithdrawMoney(currentCost.CostUpgrade))
            {
                CapacityUpgradeBlock.gameObject.SetActive(true);
            }
            else
            {
                CapacityUpgradeBlock.gameObject.SetActive(false);
                CapacityUpgradeText.text = "Increases the amount of income";
                CapacityUpgradeText.color = new Color(118, 90, 42, 90);

            }


        }
    }

    public void CloseTab()
    {
        IslandManager.Instance.cameraController.ResetCamera();
        currentHouse = null;
        gameObject.SetActive(false);
    }
}
