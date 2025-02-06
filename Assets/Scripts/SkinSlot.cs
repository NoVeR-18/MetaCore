using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinSlot : MonoBehaviour
{
    public GameObject lockedPanel;
    public GameObject unselectedPanel;
    public GameObject selectedPanel;
    public Image PreviewUnselectedIcon;
    public Image PreviewSelectedIcon;
    public TextMeshProUGUI coastText;
    public Button buyButton;
    public Button selectButton;

    public void UpdateUI(bool isPurchased)
    {
        lockedPanel.SetActive(!isPurchased);
        unselectedPanel.SetActive(isPurchased);
        //selectButton.gameObject.SetActive(false);
    }

    public void SetPurchased()
    {
        selectedPanel.gameObject.SetActive(false);
        lockedPanel.SetActive(false);
        unselectedPanel.SetActive(true);
    }

    public void SetSelected()
    {
        lockedPanel.SetActive(false);
        unselectedPanel.SetActive(true);
        selectedPanel.SetActive(true);
    }
}
