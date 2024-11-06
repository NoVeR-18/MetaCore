using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HouseUpgradePanel : MonoBehaviour
{
    public Player.PlayerWallet playerWallet;
    [SerializeField] private int goldCost = 15;
    public House currentHouse;                    // Текущий выбранный домик
    public Button upgradeRentButton;              // Кнопка для повышения аренды
    public Button upgradeCapacityButton;          // Кнопка для повышения вместимости
    public Button closeButton;                    // Кнопка для закрытия панели
    public TextMeshProUGUI rentPriceText;         // Текст, отображающий цену аренды
    public TextMeshProUGUI capacityText;          // Текст, отображающий вместимость
    public TextMeshProUGUI levelText;             // Текст, отображающий уровень домика
    public TextMeshProUGUI maxLevelText;          // Текст, отображающий максимальный уровень домика
    public Image levelProgressBar;                // Прогресс-бар для отображения уровня

    private void Start()
    {
        UpdatePanel();
        closeButton.onClick.AddListener(() => { CloseTab(); });

        upgradeRentButton.onClick.AddListener(() => { OnUpgradeRentButton(); });
        upgradeCapacityButton.onClick.AddListener(() => { OnUpgradeCapacityButton(); });
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
            if (playerWallet.WithdrawMoney(goldCost))
            {
                currentHouse.IncreaseRentPrice(0.1f); // Увеличиваем аренду на 0.1
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
            if (playerWallet.WithdrawMoney(goldCost))
            {
                currentHouse.IncreaseCapacity(1); // Увеличиваем вместимость на 1
                UpdatePanel();
                OnUpgradeHouseButton();
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
            rentPriceText.text = currentHouse.rentPrice.ToString("F1"); // Отображение цены аренды
            capacityText.text = currentHouse.capacity.ToString();       // Отображение вместимости
            levelText.text = $"LVL  {currentHouse.level}";          // Отображение текущего уровня
            maxLevelText.text = $"LVL {currentHouse.maxLevel}";           // Отображение максимального уровня

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
