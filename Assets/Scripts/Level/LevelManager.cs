using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform Player;
    public TilemapLoader Loader;
    public PlayerMovement playerMovement;
    public Transform cameraTransform;

    public int ColectedCoins;
    public int ColectedCrystal;
    public int ColectePeople;


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

        Loader.LoadTilemap();
        var centerOfMap = Loader.FindPaintedTilesCenter();
        cameraTransform.position = new Vector3(centerOfMap.x, cameraTransform.position.y, centerOfMap.z);

        ColectedCoins = 0;
        ColectedCrystal = 0;
        ColectePeople = 0;
    }

    public void LevelComplete()
    {
        currentLevel++;
        PlayerPrefs.SetInt(LevelName, currentLevel);
        Loader.tilemapData = Resources.Load<TilemapData>($"Levels/Level{currentLevel}");
        Loader.LoadTilemap();
        var centerOfMap = Loader.FindPaintedTilesCenter();
        cameraTransform.position = new Vector3(centerOfMap.x, cameraTransform.position.y, centerOfMap.z);
        playerMovement.DisableMove();
    }

}
