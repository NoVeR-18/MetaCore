using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Transform Player;
    public TilemapLoader Loader;
    public PlayerMovement playerMovement;
    public Transform cameraTransform;

    public PlayerTileInteraction tileInteraction;

    public int ColectedCoins;
    public int ColectedCrystal;
    public int ColectedPeople;

    public TextMeshProUGUI CurrentLevel;
    public TextMeshProUGUI CurrentCoins;
    public TextMeshProUGUI CurrentCrystal;
    public TextMeshProUGUI CurrentPeople;

    public Transform VictoryPopUp;

    public Button TakeButton;
    public Button GoToIslandButton;
    public Button ResetButton;

    public static LevelManager Instance;
    [SerializeField] private int currentLevel = 1;
    private const string LevelName = "CurrentLevel";

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
        currentLevel = PlayerPrefs.GetInt(LevelName, 1);
        Loader.tilemapData = Resources.Load<TilemapData>($"Levels/Level{currentLevel}");
        CurrentLevel.text = $"LEVEL {currentLevel}";

        Loader.LoadTilemap();
        var centerOfMap = Loader.FindPaintedTilesCenter();
        cameraTransform.position = new Vector3(centerOfMap.x, cameraTransform.position.y, cameraTransform.position.z);
        TakeButton.onClick.AddListener(() =>
        {
            VictoryPopUp.gameObject.SetActive(false);
        });
        GoToIslandButton.onClick.AddListener(() => { SceneManager.LoadScene("MetaSceneTest"); });
        ResetButton.onClick.AddListener(() => { ResetLevels(); });
        ColectedCoins = 0;
        ColectedCrystal = 0;
        ColectedPeople = 0;
    }

    public void LevelComplete()
    {
        currentLevel++;
        PlayerPrefs.SetInt(LevelName, currentLevel);
        if (currentLevel % 5 == 0)
        {
            SetVictory();
        }
        {
            CurrentLevel.text = currentLevel.ToString();
            Loader.tilemapData = Resources.Load<TilemapData>($"Levels/Level{currentLevel}");
            Loader.LoadTilemap();
            var centerOfMap = Loader.FindPaintedTilesCenter();
            cameraTransform.position = new Vector3(centerOfMap.x, cameraTransform.position.y, cameraTransform.position.z);
            playerMovement.DisableMove();
        }
    }
    private void SetVictory()
    {
        CurrentCoins.text = ColectedCoins.ToString();
        CurrentCrystal.text = ColectedCrystal.ToString();
        CurrentPeople.text = ColectedPeople.ToString();
        VictoryPopUp.gameObject.SetActive(true);

        PlayerPrefs.SetInt("TotalCoins", ColectedCoins);
        PlayerPrefs.SetInt("TotalCrystal", ColectedCrystal);
        PlayerPrefs.SetInt("TotalPeople", ColectedPeople);
        PlayerPrefs.Save();
    }

    private void ResetLevels()
    {
        currentLevel = 1;
        PlayerPrefs.SetInt(LevelName, currentLevel);
        CurrentLevel.text = currentLevel.ToString();
        Loader.tilemapData = Resources.Load<TilemapData>($"Levels/Level{currentLevel}");
        Loader.LoadTilemap();
        tileInteraction.tilesToPaint = Loader.tilemapData.tilesToPaint.Length;
        var centerOfMap = Loader.FindPaintedTilesCenter();
        cameraTransform.position = new Vector3(centerOfMap.x, cameraTransform.position.y, cameraTransform.position.z);
        playerMovement.DisableMove();
    }
}
