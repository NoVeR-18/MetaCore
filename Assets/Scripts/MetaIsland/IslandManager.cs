using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IslandManager : MonoBehaviour
{
    public HouseUpgradePanel houseUpgradePanel;

    public CameraController cameraController;

    public PlayerWallet playerWallet;
    public Button GoToLevelButton;

    public static IslandManager Instance;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        GoToLevelButton.onClick.AddListener(() =>
        {
            GameManager.Instance.Vibrate();
            SceneManager.LoadScene("CoreScene");
        });
        YsoCorp.GameUtils.YCManager.instance.OnGameStarted(0);
    }
}
