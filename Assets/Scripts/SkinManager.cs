using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance;

    public List<SkinData> playerSkins;
    public PlayerSkinController playerSkinController;

    private int selectedSkinIndex = 0;
    private const string SaveKey = "SelectedSkin";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        LoadSkins();
    }

    public void ApplySkin(int skinIndex)
    {
        if (playerSkins == null || skinIndex >= playerSkins.Count) return;

        if (playerSkins[skinIndex].isPurchased)
        {
            playerSkinController.SetPlayerModel(playerSkins[skinIndex].modelPrefab);
            selectedSkinIndex = skinIndex;
            SaveSkins();
        }
    }

    public bool PurchaseSkin(int skinIndex)
    {
        if (playerSkins == null || skinIndex >= playerSkins.Count || playerSkins[skinIndex].isPurchased) return false;

        if (IslandManager.Instance.playerWallet.CanWithdrawCrystal(playerSkins[skinIndex].price))
        {
            playerSkins[skinIndex].isPurchased = true;
            SaveSkins();
            return true;
        }
        return false;
    }

    public List<SkinData> GetSkinList()
    {
        return playerSkins;
    }

    private void SaveSkins()
    {
        SaveData data = new SaveData(playerSkins, selectedSkinIndex);
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    //private void LoadSkins()
    //{

    //    selectedSkinIndex = PlayerPrefs.GetInt(SaveKey, 0);
    //    if (selectedSkinIndex < playerSkins.Count)
    //    {
    //        playerSkinController.SetPlayerModel(playerSkins[selectedSkinIndex].modelPrefab);
    //    }
    //}
    private void LoadSkins()
    {
        if (!PlayerPrefs.HasKey(SaveKey)) return;

        string json = PlayerPrefs.GetString(SaveKey);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // Обновляем статус покупки в текущих списках скинов
        UpdateSkinPurchaseStatus(playerSkins, data.Skins);

        // Восстанавливаем выбранные скины
        selectedSkinIndex = data.selectedSkinIndex;
    }
    private void UpdateSkinPurchaseStatus(List<SkinData> currentSkins, List<SkinData> savedSkins)
    {
        if (savedSkins == null || currentSkins == null) return;

        foreach (var savedSkin in savedSkins)
        {
            SkinData currentSkin = currentSkins.Find(s => s.skinID == savedSkin.skinID);
            if (currentSkin != null)
            {
                currentSkin.isPurchased = savedSkin.isPurchased; // Восстанавливаем покупку
            }
        }
    }

    public int GetSelectedSkinIndex()
    {
        return selectedSkinIndex;
    }

    [System.Serializable]
    private class SaveData
    {
        public List<SkinData> Skins;
        public int selectedSkinIndex;

        public SaveData(List<SkinData> Skins, int selected)
        {
            this.Skins = Skins ?? new List<SkinData>();

            selectedSkinIndex = selected;
        }
    }
}

[System.Serializable]
public class SkinData
{
    public string skinID;
    public Mesh modelPrefab; // Теперь храним модель, а не материал
    public Sprite previewIcon;
    public int price;
    public bool isPurchased;
}
