using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HouseUpgradePanel : MonoBehaviour
{
    public LevelProgressManager levelProgressManager;
    public House currentHouse;                    // Текущий выбранный домик
    public Button upgradeRentButton;              // Кнопка для повышения аренды
    public Button upgradeCapacityButton;          // Кнопка для повышения вместимости
    public Button closeButton;                    // Кнопка для закрытия панели
    public TextMeshProUGUI rentPriceText;         // Текст, отображающий цену аренды
    public TextMeshProUGUI capacityText;          // Текст, отображающий вместимость
    public TextMeshProUGUI levelText;             // Текст, отображающий уровень домика
    public TextMeshProUGUI maxLevelText;          // Текст, отображающий максимальный уровень домика
    public TextMeshProUGUI rentPriceUpdateText;          // Текст, отображающий максимальный уровень домика
    public TextMeshProUGUI rentPriceAwardText;          // Текст, отображающий максимальный уровень домика
    public Transform rentUpgradeBlock;
    public TextMeshProUGUI rentBlockPriceText;          // Текст, отображающий максимальный уровень домика
    public Transform rentUpgradeMax;
    public Image levelProgressBar;                // Прогресс-бар для отображения уровня
    public ExperienceTable experienceTable;
    public TextMeshProUGUI currentCapacityText;          // Текст, отображающий максимальный уровень домика
    public TextMeshProUGUI capacityPriceAwardText;          // Текст, отображающий максимальный уровень домика
    public TextMeshProUGUI capacityPriceUpdateText;          // Текст, отображающий максимальный уровень домика
    public Transform CapacityUpgradeBlock;
    public TextMeshProUGUI CapacityBlockPriceText;          // Текст, отображающий максимальный уровень домика
    public TextMeshProUGUI CapacityUpgradeText;
    public Transform CapacityUpgradeMax;
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
                currentHouse.IncreaseRentPrice(); // Увеличиваем аренду на 0.1
                levelProgressManager.AddExperience(experienceTable.incomeCost[currentHouse.rentPriceLevel].y);
                OnUpgradeHouseButton();
                UpdatePanel();
            }
            else
            {
                Debug.Log("Недостаточно золота для повышения аренды.");
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
                Debug.Log("Недостаточно золота для повышения вместимости.");
            }
        }
    }

    public void OnUpgradeHouseButton()
    {
        if (currentHouse != null)
        {
            currentHouse.UpgradeHouse(); // Повышаем уровень домика
            UpdatePanel();
        }
    }

    private void UpdatePanel()
    {
        // Обновление текстовых полей на панели с информацией о домике
        if (currentHouse != null)
        {

            capacityText.text = currentHouse.capacity.ToString();       // Отображение вместимости
            rentPriceText.text = (1 + 0.01f * currentHouse.rentPriceLevel).ToString("F1"); // Отображение цены аренды
            levelText.text = $"LVL  {currentHouse.level}";          // Отображение текущего уровня
            maxLevelText.text = $"LVL {experienceTable.incomeCost.Count}";           // Отображение максимального уровня
            rentPriceUpdateText.text = $"{experienceTable.incomeCost[currentHouse.rentPriceLevel].x}";
            rentPriceAwardText.text = $"{experienceTable.incomeCost[currentHouse.rentPriceLevel].y}";
            // Обновление прогресс-бара уровня
            levelProgressBar.fillAmount = (float)currentHouse.level / experienceTable.incomeCost.Count;

            if (currentHouse.capacityLevel == experienceTable.capacityCost.Count)
            {
                CapacityUpgradeMax.gameObject.SetActive(true);
            }
            else
            {
                currentCapacityText.text = $"{currentHouse.currentResidents} / {currentHouse.capacity}";
                capacityPriceUpdateText.text = $"{experienceTable.capacityCost[currentHouse.capacityLevel].CostUpgrade}";
                capacityPriceAwardText.text = $"{experienceTable.capacityCost[currentHouse.capacityLevel].AwardStarsCount}";
                CapacityUpgradeMax.gameObject.SetActive(false);
                var currentCost = experienceTable.capacityCost[currentHouse.capacityLevel];
                if (currentHouse.level + 1 <= currentCost.minLevelToUpgrade)
                {
                    CapacityUpgradeText.text = $"Level {experienceTable.capacityCost[currentHouse.capacityLevel].minLevelToUpgrade} needed";
                    CapacityUpgradeBlock.gameObject.SetActive(true);
                    CapacityBlockPriceText.text = currentCost.CostUpgrade.ToString();
                    CapacityUpgradeText.color = Color.red;
                }
                else if (!IslandManager.Instance.playerWallet.CanWithdrawMoney(currentCost.CostUpgrade))
                {
                    CapacityUpgradeBlock.gameObject.SetActive(true);
                    CapacityBlockPriceText.text = currentCost.CostUpgrade.ToString();
                }
                else
                {
                    CapacityUpgradeBlock.gameObject.SetActive(false);
                    CapacityUpgradeText.text = "Increases the amount of income";
                    CapacityUpgradeText.color = new Color(118, 90, 42, 90);

                }
            }
            if (currentHouse.level >= experienceTable.incomeCost.Count)
            {
                rentUpgradeMax.gameObject.SetActive(true);

            }
            else
                rentUpgradeMax.gameObject.SetActive(false);
            if (!IslandManager.Instance.playerWallet.CanWithdrawMoney(experienceTable.incomeCost[currentHouse.rentPriceLevel].x))
            {
                rentUpgradeBlock.gameObject.SetActive(true);
                rentBlockPriceText.text = experienceTable.incomeCost[currentHouse.rentPriceLevel].x.ToString();
            }
            else
            {
                rentUpgradeBlock.gameObject.SetActive(false);
            }

        }
    }

    private void OnEnable()
    {
        House.GetCoins += UpdatePanel;
    }
    private void OnDisable()
    {
        House.GetCoins -= UpdatePanel;
    }

    public void CloseTab()
    {
        IslandManager.Instance.cameraController.ResetCamera();
        currentHouse = null;
        gameObject.SetActive(false);
    }
}
