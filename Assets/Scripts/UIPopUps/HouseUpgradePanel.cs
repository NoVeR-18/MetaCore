using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HouseUpgradePanel : MonoBehaviour
{
    public Player.PlayerWallet playerWallet;
    [SerializeField] private int goldCost = 150;
    public House currentHouse;                // ������� ��������� �����
    public Button upgradeRentButton;          // ������ ��� ��������� ������
    public Button upgradeCapacityButton;      // ������ ��� ��������� �����������
    public Button closeNutton;      // ������ ��� ��������� �����������
    public TextMeshProUGUI rentPriceText;                // �����, ������������ ���� ������
    public TextMeshProUGUI capacityText;                 // �����, ������������ �����������
    public TextMeshProUGUI levelText;                    // �����, ������������ ������� ������

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
                currentHouse.IncreaseRentPrice(50); // ����������� ������ �� 50
                UpdatePanel();
            }
            else
            {
                Debug.Log("������������ ������ ��� ������������� ������.");
            }
        }
    }

    public void OnUpgradeCapacityButton()
    {
        if (currentHouse != null)
        {
            if (playerWallet.WithdrawMoney(goldCost))
            {
                currentHouse.IncreaseCapacity(1); // ����������� ����������� �� 1
                UpdatePanel();
                OnUpgradeHouseButton();
            }
            else
            {
                Debug.Log("������������ ������ ��� ������������� ������.");
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

