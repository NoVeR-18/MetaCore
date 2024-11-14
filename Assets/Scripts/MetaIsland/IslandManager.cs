using Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IslandManager : MonoBehaviour
{
    public HouseUpgradePanel houseUpgradePanel;

    public CameraController cameraController;

    public PlayerWallet playerWallet;
    public Button GoToLevelButton;
    public TextMeshProUGUI LevelText;

    public static IslandManager Instance;


    private const string LevelName = "CurrentLevel";
    private const string LevelMulltiplayer = "LevelMulltiplayer";
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
        LevelText.text = $"LEVEL {PlayerPrefs.GetInt(LevelName, 1) + PlayerPrefs.GetInt(LevelName, 1) * PlayerPrefs.GetInt(LevelMulltiplayer, 0)}";
    }
}
