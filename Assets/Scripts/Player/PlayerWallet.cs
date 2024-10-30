using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerWallet : MonoBehaviour
    {
        private const string CoinKey = "PlayerCoin";
        private const string CrystalKey = "PlayerCrystal";

        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI crystalText;

        public float CoinBalance { get; private set; } = 0;
        public float CrystalBalance { get; private set; } = 0;

        private void Start()
        {
            LoadBalance();
            UpdateBalanceUI();
        }

        public void AddMoney(float amount)
        {
            CoinBalance += amount;
            SaveBalance();
            UpdateBalanceUI();
            Debug.Log($"Added coins: {amount}. Balance: {CoinBalance}");
        }
        public void AddCrystal(float amount)
        {
            CrystalBalance += amount;
            SaveBalance();
            UpdateBalanceUI();
            Debug.Log($"Added coins: {amount}. Balance: {CoinBalance}");
        }

        public bool WithdrawMoney(float amount)
        {
            if (CoinBalance >= amount)
            {
                CoinBalance -= amount;
                SaveBalance();
                UpdateBalanceUI();
                Debug.Log($"Minus: {amount}. Current balance: {CoinBalance}");
                return true;
            }
            else
            {
                Debug.Log("not enough coins");
                return false;
            }
        }
        public bool WithdrawCrystal(float amount)
        {
            if (CrystalBalance >= amount)
            {
                CrystalBalance -= amount;
                SaveBalance();
                UpdateBalanceUI();
                Debug.Log($"Minus: {amount}. Current balance: {CrystalBalance}");
                return true;
            }
            else
            {
                Debug.Log("not enough crystalss");
                return false;
            }
        }

        private void SaveBalance()
        {
            PlayerPrefs.SetFloat(CoinKey, CoinBalance);
            PlayerPrefs.SetFloat(CrystalKey, CrystalBalance);
            PlayerPrefs.Save();
        }

        private void LoadBalance()
        {
            CoinBalance = PlayerPrefs.GetFloat(CoinKey, 0);
            CrystalBalance = PlayerPrefs.GetFloat(CrystalKey, 0);
        }

        private void UpdateBalanceUI()
        {
            if (coinText != null)
            {
                coinText.text = $"{CoinBalance}";
            }
            if (crystalText != null)
            {
                crystalText.text = $"{CrystalBalance}";
            }
        }
    }
}