using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HouseUpgradePanel : MonoBehaviour
{
    public Player.PlayerWallet playerWallet;
    [SerializeField] private int goldCost = 15;
    public House currentHouse;                    // ������� ��������� �����
    public Button upgradeRentButton;              // ������ ��� ��������� ������
    public Button upgradeCapacityButton;          // ������ ��� ��������� �����������
    public Button closeButton;                    // ������ ��� �������� ������
    public TextMeshProUGUI rentPriceText;         // �����, ������������ ���� ������
    public TextMeshProUGUI capacityText;          // �����, ������������ �����������
    public TextMeshProUGUI levelText;             // �����, ������������ ������� ������
    public TextMeshProUGUI maxLevelText;          // �����, ������������ ������������ ������� ������
    public Image levelProgressBar;                // ��������-��� ��� ����������� ������

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
                currentHouse.IncreaseRentPrice(0.1f); // ����������� ������ �� 0.1
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
            if (playerWallet.WithdrawMoney(goldCost))
            {
                currentHouse.IncreaseCapacity(1); // ����������� ����������� �� 1
                UpdatePanel();
                OnUpgradeHouseButton();
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
            rentPriceText.text = currentHouse.rentPrice.ToString("F1"); // ����������� ���� ������
            capacityText.text = currentHouse.capacity.ToString();       // ����������� �����������
            levelText.text = $"LVL  {currentHouse.level}";          // ����������� �������� ������
            maxLevelText.text = $"LVL {currentHouse.maxLevel}";           // ����������� ������������� ������

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
