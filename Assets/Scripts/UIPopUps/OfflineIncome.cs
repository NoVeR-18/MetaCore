using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfflineIncome : MonoBehaviour
{
    public TextMeshProUGUI CoinIncomeText;
    public TextMeshProUGUI LastTimeOpened;
    public Button CollectButton;

    public int totalCollected = 0;
    private void Start()
    {
        CollectButton.onClick.AddListener(() =>
        {
            GetCoins();
        });


        string lastSavedTime = PlayerPrefs.GetString($"House_{0}_LastSavedTime", DateTime.Now.ToString());
        DateTime lastTime;
        if (DateTime.TryParse(lastSavedTime, out lastTime))
        {
            TimeSpan timeAway = DateTime.Now - lastTime;
            LastTimeOpened.text = $"{timeAway.Hours:D2}:{timeAway.Minutes:D2}:{timeAway.Seconds:D2}";
        }
    }

    public void UpdateUI()
    {
        CoinIncomeText.text = totalCollected.ToString();
    }

    public void GetCoins()
    {

        IslandManager.Instance.playerWallet.AddMoney(totalCollected);
        gameObject.SetActive(false);
    }




}
