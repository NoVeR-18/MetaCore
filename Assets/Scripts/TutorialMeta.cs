using UnityEngine;

public class TutorialMeta : MonoBehaviour
{
    public Transform BlurToPlay;
    public Transform BlurToDragPeople;
    public Transform BuildTutorial;
    public Transform ClickToUpgrade;
    public Transform ClickToUpgradeRent;
    public Transform ClickToDrag;
    public Transform BlurToUpgradeRent;
    public bool playComplete = false;
    private const string playCompletePrefKey = "TutorialPlay";

    public static TutorialMeta Instance
    {
        get; set;
    }
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
        BlurToPlay.gameObject.SetActive(false);
        BlurToDragPeople.gameObject.SetActive(false);
        BuildTutorial.gameObject.SetActive(false);
        ClickToUpgrade.gameObject.SetActive(false);
        ClickToUpgradeRent.gameObject.SetActive(false);
        ClickToDrag.gameObject.SetActive(false);
        BlurToUpgradeRent.gameObject.SetActive(false);
        if (!GameManager.Instance.tutorialCompleted)
        {
            IslandManager.Instance.cameraController._canScroling = false;
            LoadTutorial();
            if (!playComplete)
            {
                BuildTutorial.gameObject.SetActive(false);
                BlurToPlay.gameObject.SetActive(true);
            }
            else
            {
                BuildTutorial.gameObject.SetActive(true);
                BlurToPlay.gameObject.SetActive(true);
            }
        }
        else
        {
            IslandManager.Instance.cameraController._canScroling = true;
        }
    }
    private void LoadTutorial()
    {
        if (PlayerPrefs.HasKey(playCompletePrefKey))
        {
            playComplete = PlayerPrefs.GetInt(playCompletePrefKey) == 0;
        }
        else
        {
            playComplete = false;
        }
        SetTutorial(playComplete);
    }

    public void SetTutorial(bool enabled)
    {
        playComplete = enabled;

        PlayerPrefs.SetInt(playCompletePrefKey, playComplete ? 0 : 1);
        PlayerPrefs.Save();
    }
    public void PlayLevel()
    {
        if (!GameManager.Instance.tutorialCompleted)
            SetTutorial(true);
    }
    public void BuildHouse()
    {
        if (!GameManager.Instance.tutorialCompleted)
        {
            BuildTutorial.gameObject.SetActive(false);
            ClickToUpgrade.gameObject.SetActive(true);
        }
    }
    public void UpgradeHouse()
    {
        if (!GameManager.Instance.tutorialCompleted)
        {
            ClickToUpgrade.gameObject.SetActive(false);
            BlurToUpgradeRent.gameObject.SetActive(true);
            ClickToUpgradeRent.gameObject.SetActive(true);
        }
    }
    public void UpgradeRent()
    {
        if (!GameManager.Instance.tutorialCompleted)
        {
            BlurToPlay.gameObject.SetActive(false);
            BlurToUpgradeRent.gameObject.SetActive(false);
            IslandManager.Instance.houseUpgradePanel.gameObject.SetActive(false);
            ClickToDrag.gameObject.SetActive(true);
            BlurToDragPeople.gameObject.SetActive(true);
        }
    }

    public void StartDrag()
    {
        if (!GameManager.Instance.tutorialCompleted)
        {
            ClickToDrag.gameObject.SetActive(false);
            ClickToUpgrade.gameObject.SetActive(true);

        }
    }
    public void EndDrag()
    {
        if (!GameManager.Instance.tutorialCompleted)
        {

            ClickToUpgrade.gameObject.SetActive(false);

            BlurToDragPeople.gameObject.SetActive(false);

            GameManager.Instance.SetTutorial(true);

        }
    }
}
