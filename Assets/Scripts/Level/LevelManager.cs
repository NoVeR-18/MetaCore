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

    public ParticleSystem smallVictory;

    public static LevelManager Instance;
    [SerializeField] private int currentLevel = 1;
    private const string LevelName = "CurrentLevel";
    private const string LevelMulltiplayer = "LevelMulltiplayer";

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
        Debug.Log("Loaded level:" + currentLevel);

        currentLevel = PlayerPrefs.GetInt(LevelName, 1);
        CreateLevel();
        TakeButton.onClick.AddListener(() =>
        {
            audioSource.PlayOneShot(audioClips[0]);
            VictoryPopUp.gameObject.SetActive(false);
            GameManager.Instance.Vibrate();
            currentLevel++;
            CreateLevel();
            audioSource.PlayOneShot(audioClips[3]);

            foreach (GameObject people in tileInteraction.ColectedPeople)
            {
                Destroy(people);
            }
            tileInteraction.ColectedPeople.Clear();
        });
        GoToIslandButton.onClick.AddListener(() =>
        {
            audioSource.PlayOneShot(audioClips[1]);
            SceneManager.LoadScene(0);
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
        PlayerPrefs.SetInt(LevelName, currentLevel + 1);
        PlayerPrefs.Save();
        smallVictory.Play();
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
        Loader.tilemapData = Resources.Load<TilemapData>($"Levels/Level{currentLevel}");
        if (Loader.tilemapData == null)
        {
            currentLevel = 1;
            PlayerPrefs.SetInt(LevelName, currentLevel);
            PlayerPrefs.Save();
            Loader.tilemapData = Resources.Load<TilemapData>($"Levels/Level{currentLevel}");
        }
        Debug.Log("Loaded:" + Loader.tilemapData.name);
        CurrentLevel.text = $"LEVEL {PlayerPrefs.GetInt(LevelName, 1) + PlayerPrefs.GetInt(LevelName, 1) * PlayerPrefs.GetInt(LevelMulltiplayer, 0)}";
        Loader.LoadTilemap();
        tileInteraction.tilesToPaint = Loader.tilemapData.tilesToPaint.Length;
        var centerOfMap = Loader.FindCenterFromObjects();
        Debug.Log("MapCenter" + centerOfMap);
        var sizeField = Loader.SizeOfField();
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
        TutorialCore.Instance.CompleteLevel();
        ColectedCoins = 0;
        ColectedCrystal = 0;
        ColectedPeople = 0;
        GoToIslandButton.gameObject.SetActive(true);
        PlayerMovement.CanMoving = false;
    }

    private void ResetLevels()
    {
        currentLevel = 1;
        PlayerPrefs.SetInt(LevelName, currentLevel);
        PlayerPrefs.Save();
        Loader.tilemapData = Resources.Load<TilemapData>($"Levels/Level{currentLevel}");

        if (Loader.tilemapData == null)
        {
            currentLevel = 1;
            Loader.tilemapData = Resources.Load<TilemapData>($"Levels/Level{currentLevel}");
        }
        CurrentLevel.text = $"LEVEL {PlayerPrefs.GetInt(LevelName, 1) + PlayerPrefs.GetInt(LevelName, 1) * PlayerPrefs.GetInt(LevelMulltiplayer, 0)}";
        Loader.LoadTilemap();
        tileInteraction.tilesToPaint = Loader.tilemapData.tilesToPaint.Length;
        var centerOfMap = Loader.FindCenterFromObjects();
        var sizeField = Loader.SizeOfField();
        PositionCamera(centerOfMap, sizeField.x, sizeField.y);
        playerMovement.DisableMove();
    }
    public float padding = 1.2f;
    public void PositionCamera(Vector3 center, float width, float height)
    {
        if (!cameraTransform.orthographic)
        {
            Debug.LogWarning("Камера должна быть ортографической для этого метода.");
            return;
        }

        cameraTransform.transform.position = new Vector3(center.x, center.y + 10f, cameraTransform.transform.position.z);

        float sizeX = width / cameraTransform.aspect / 2;
        float sizeY = height / 2;

        cameraTransform.orthographicSize = Mathf.Max(sizeX, sizeY) * padding;
    }
}
