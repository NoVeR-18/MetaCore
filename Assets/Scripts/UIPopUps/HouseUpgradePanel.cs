using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HouseUpgradePanel : MonoBehaviour
{
    public Player.PlayerWallet playerWallet;
    [SerializeField] private int goldCost = 150;
    public House currentHouse;                // Текущий выбранный домик
    public Button upgradeRentButton;          // Кнопка для повышения аренды
    public Button upgradeCapacityButton;      // Кнопка для повышения вместимости
    public Button closeNutton;      // Кнопка для повышения вместимости
    public TextMeshProUGUI rentPriceText;                // Текст, отображающий цену аренды
    public TextMeshProUGUI capacityText;                 // Текст, отображающий вместимость
    public TextMeshProUGUI levelText;                    // Текст, отображающий уровень домика

    private void Start()
    {
        UpdatePanel();
        closeNutton.onClick.AddListener(() => { CloseTab(); });

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
                currentHouse.IncreaseRentPrice(50); // Увеличиваем аренду на 50
                UpdatePanel();
            }
            else
            {
                Debug.Log("Недостаточно золота для разблокировки ячейки.");
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
                Debug.Log("Недостаточно золота для разблокировки ячейки.");
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
            rentPriceText.text = "" + currentHouse.rentPrice;
            capacityText.text = "" + currentHouse.capacity;
            levelText.text = "" + currentHouse.level;
        }
    }

    private void CloseTab()
    {
        IslandManager.Instance.cameraController.ResetCamera();
        currentHouse = null;
        gameObject.SetActive(false);
    }
}

