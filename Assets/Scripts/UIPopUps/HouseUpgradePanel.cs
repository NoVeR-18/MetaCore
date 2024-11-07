using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HouseUpgradePanel : MonoBehaviour
{
    public Player.PlayerWallet playerWallet;
    public LevelProgressManager levelProgressManager;
    [SerializeField] private int goldCost = 15;
    public House currentHouse;                    // Текущий выбранный домик
    public Button upgradeRentButton;              // Кнопка для повышения аренды
    public Button upgradeCapacityButton;          // Кнопка для повышения вместимости
    public Button closeButton;                    // Кнопка для закрытия панели
    public TextMeshProUGUI rentPriceText;         // Текст, отображающий цену аренды
    public TextMeshProUGUI capacityText;          // Текст, отображающий вместимость
    public TextMeshProUGUI levelText;             // Текст, отображающий уровень домика
    public TextMeshProUGUI maxLevelText;          // Текст, отображающий максимальный уровень домика
    public TextMeshProUGUI currentCapacityText;          // Текст, отображающий максимальный уровень домика
    public TextMeshProUGUI rentPriceUpdateText;          // Текст, отображающий максимальный уровень домика
    public TextMeshProUGUI rentPriceAwardText;          // Текст, отображающий максимальный уровень домика
    public TextMeshProUGUI capacityPriceAwardText;          // Текст, отображающий максимальный уровень домика
    public TextMeshProUGUI capacityPriceUpdateText;          // Текст, отображающий максимальный уровень домика
    public Image levelProgressBar;                // Прогресс-бар для отображения уровня
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
                currentHouse.IncreaseRentPrice(); // Увеличиваем аренду на 0.1
                levelProgressManager.AddExperience(experienceTable.incomeCost[currentHouse.rentPriceLevel].y);
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
            if (playerWallet.WithdrawMoney(currentLevel.CostUpgrade))
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
            rentPriceText.text = (1 + 0.01f * currentHouse.rentPriceLevel).ToString("F1"); // Отображение цены аренды
            capacityText.text = currentHouse.capacity.ToString();       // Отображение вместимости
            levelText.text = $"LVL  {currentHouse.level}";          // Отображение текущего уровня
            maxLevelText.text = $"LVL {currentHouse.maxLevel}";           // Отображение максимального уровня
            currentCapacityText.text = $"{currentHouse.currentResidents} / {currentHouse.capacity}";
            rentPriceUpdateText.text = $"{experienceTable.incomeCost[currentHouse.rentPriceLevel].x}";
            rentPriceAwardText.text = $"{experienceTable.incomeCost[currentHouse.rentPriceLevel].y}";
            capacityPriceUpdateText.text = $"{experienceTable.capacityCost[currentHouse.capacityLevel].CostUpgrade}";
            capacityPriceAwardText.text = $"{experienceTable.capacityCost[currentHouse.capacityLevel].AwardStarsCount}";
            // Обновление прогресс-бара уровня
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
