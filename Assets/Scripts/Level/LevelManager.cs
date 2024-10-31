using Player;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Transform Player;
    public TilemapLoader Loader;
    public PlayerMovement playerMovement;
    public Camera cameraTransform;

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

    public AudioSource audioSource;
    public List<AudioClip> audioClips = new List<AudioClip>();

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

        if (Loader.tilemapData == null)
        {
            currentLevel = 1;
            Loader.tilemapData = Resources.Load<TilemapData>($"Levels/Level{currentLevel}");
        }
        CurrentLevel.text = $"LEVEL {currentLevel}";

        Loader.LoadTilemap();
        var centerOfMap = Loader.FindPaintedTilesCenter();
        var sizeField = Loader.SizeOfField();
        //cameraTransform.position = new Vector3(centerOfMap.x, cameraTransform.position.y, cameraTransform.position.z);
        PositionCamera(centerOfMap, sizeField.x, sizeField.y);
        TakeButton.onClick.AddListener(() =>
        {
            audioSource.PlayOneShot(audioClips[0]);
            VictoryPopUp.gameObject.SetActive(false);
            PlayerMovement.CanMoving = true;
            GameManager.Instance.Vibrate();
            currentLevel++;
            CreateLevel();
            audioSource.PlayOneShot(audioClips[3]);
        });
        GoToIslandButton.onClick.AddListener(() =>
        {
            audioSource.PlayOneShot(audioClips[1]);
            SceneManager.LoadScene("MetaSceneTest");
            PlayerMovement.CanMoving = true;
            GameManager.Instance.Vibrate();
        });
        ResetButton.onClick.AddListener(() =>
        {
            audioSource.PlayOneShot(audioClips[1]);
            ResetLevels();
            PlayerMovement.CanMoving = true;
            GameManager.Instance.Vibrate();
        });
        ColectedCoins = 0;
        ColectedCrystal = 0;
        ColectedPeople = 0;
    }

    public void LevelComplete()
    {
        PlayerPrefs.SetInt(LevelName, currentLevel);
        if (currentLevel % 5 == 0)
        {
            SetVictory();
        }
        else
        {
            currentLevel++;
            CreateLevel();
        }
    }
    private void CreateLevel()
    {

        CurrentLevel.text = currentLevel.ToString();
        Loader.tilemapData = Resources.Load<TilemapData>($"Levels/Level{currentLevel}");

        if (Loader.tilemapData == null)
        {
            currentLevel = 1;
            Loader.tilemapData = Resources.Load<TilemapData>($"Levels/Level{currentLevel}");
        }
        CurrentLevel.text = $"LEVEL {currentLevel}";
        Loader.LoadTilemap();
        tileInteraction.tilesToPaint = Loader.tilemapData.tilesToPaint.Length;
        var centerOfMap = Loader.FindPaintedTilesCenter();
        var sizeField = Loader.SizeOfField();
        //cameraTransform.position = new Vector3(centerOfMap.x, cameraTransform.position.y, cameraTransform.position.z);
        PositionCamera(centerOfMap, sizeField.x, sizeField.y);
        playerMovement.DisableMove();
    }

    private void SetVictory()
    {
        audioSource.PlayOneShot(audioClips[2]);
        CurrentCoins.text = ColectedCoins.ToString();
        CurrentCrystal.text = ColectedCrystal.ToString();
        CurrentPeople.text = ColectedPeople.ToString();
        VictoryPopUp.gameObject.SetActive(true);

        PlayerMovement.CanMoving = false;

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

        if (Loader.tilemapData == null)
        {
            currentLevel = 1;
            Loader.tilemapData = Resources.Load<TilemapData>($"Levels/Level{currentLevel}");
        }
        CurrentLevel.text = $"LEVEL {currentLevel}";
        Loader.LoadTilemap();
        tileInteraction.tilesToPaint = Loader.tilemapData.tilesToPaint.Length;
        var centerOfMap = Loader.FindPaintedTilesCenter();
        var sizeField = Loader.SizeOfField();
        //cameraTransform.position = new Vector3(centerOfMap.x, cameraTransform.position.y, cameraTransform.position.z);
        PositionCamera(centerOfMap, sizeField.x, sizeField.y);
        playerMovement.DisableMove();
    }
    public float padding = 1.2f;
    public void PositionCamera(Vector3 center, float width, float height)
    {
        // Проверка на то, что у камеры включена ортографическая проекция
        if (!cameraTransform.orthographic)
        {
            Debug.LogWarning("Камера должна быть ортографической для этого метода.");
            return;
        }

        // Устанавливаем положение камеры на центр поля
        cameraTransform.transform.position = new Vector3(center.x, center.y + 10f, cameraTransform.transform.position.z);

        // Вычисляем необходимый ортографический размер
        float sizeX = width / cameraTransform.aspect / 2;
        float sizeY = height / 2;

        // Увеличиваем размер, чтобы добавить отступ
        cameraTransform.orthographicSize = Mathf.Max(sizeX, sizeY) * padding;
    }
}
