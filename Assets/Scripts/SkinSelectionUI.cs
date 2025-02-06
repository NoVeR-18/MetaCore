using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelectionUI : MonoBehaviour
{
    public GameObject popupPanel;
    public Button closeButton;
    public Button openButton;
    public Button takeADSCrystalButton;
    public Animator animator;
    public Button rightSwapButton;
    public Button leftSwapButton;
    private const string openFirstPage = "first";
    private const string openSecondPage = "second";
    public List<SkinSlot> skinSlots;

    private void Awake()
    {
        openButton.onClick.AddListener(() => popupPanel.SetActive(true));
        closeButton.onClick.AddListener(() => popupPanel.SetActive(false));
    }

    private void Start()
    {
        takeADSCrystalButton.onClick.AddListener(() =>
        {
            //YsoCorp.GameUtils.YCManager.instance.adsManager.ShowRewarded((bool ok) =>
            //{
            //    if (ok)
            //    {
            //        LevelManager.Instance.wallet.AddCrystals(30);
            //        Vector2 uiPosition;
            //        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            //            LevelManager.Instance.wallet.canvas.transform as RectTransform,
            //            RectTransformUtility.WorldToScreenPoint(LevelManager.Instance.wallet.canvas.worldCamera, takeADSCrystalButton.transform.position),
            //            LevelManager.Instance.wallet.canvas.worldCamera,
            //            out uiPosition
            //        );

            //        LevelManager.Instance.wallet.CollectUICrystal(uiPosition);
            //    }
            //});
        });

        InitializeSlots();
        leftSwapButton.onClick.AddListener(() => { animator.SetTrigger(openFirstPage); });
        rightSwapButton.onClick.AddListener(() => { animator.SetTrigger(openSecondPage); });
    }

    private void InitializeSlots()
    {
        List<SkinData> skins = SkinManager.Instance.GetSkinList();
        if (skins == null) return;

        int selectedSkinIndex = SkinManager.Instance.GetSelectedSkinIndex();

        // Отключаем лишние слоты
        for (int i = skins.Count; i < skinSlots.Count; i++)
        {
            skinSlots[i].gameObject.SetActive(false);

        }
        skinSlots.RemoveAll(item => item.gameObject.activeSelf == false);
        for (int i = 0; i < skins.Count; i++)
        {
            int index = i;
            SkinSlot slot = skinSlots[i];

            slot.gameObject.SetActive(true);
            if (skins[index].skinID == "Default")
                SkinManager.Instance.PurchaseSkin(index);

            slot.buyButton.onClick.AddListener(() =>
            {
                if (IslandManager.Instance.playerWallet.CanWithdrawCrystal(skins[index].price))
                {
                    if (SkinManager.Instance.PurchaseSkin(index))
                    {
                        IslandManager.Instance.playerWallet.WithdrawCrystal(skins[index].price);
                        slot.SetPurchased();
                    }
                }
            });

            slot.coastText.text = skins[index].price.ToString();
            if (skins[index].previewIcon != null)
            {
                slot.PreviewSelectedIcon.sprite = skins[index].previewIcon;
                slot.PreviewUnselectedIcon.sprite = skins[index].previewIcon;
            }
            slot.selectButton.onClick.AddListener(() =>
            {
                SkinManager.Instance.ApplySkin(index);

                for (int j = 0; j < skinSlots.Count; j++)
                {
                    if (j < skins.Count && skins[j].isPurchased)
                    {
                        skinSlots[j].SetPurchased();
                    }
                    else
                        skinSlots[j].UpdateUI(skins[j].isPurchased);
                }
                slot.SetSelected();
            });

            slot.UpdateUI(skins[index].isPurchased);

            if (index == selectedSkinIndex)
            {
                slot.SetSelected();
            }
        }
    }

    public void OnSkinSelected(int skinIndex)
    {
        if (SkinManager.Instance != null)
        {
            SkinManager.Instance.ApplySkin(skinIndex);
        }
    }
}
