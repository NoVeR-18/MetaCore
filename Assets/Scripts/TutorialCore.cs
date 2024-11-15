using UnityEngine;

public class TutorialCore : MonoBehaviour
{
    public Transform tutorialBlur;
    public Transform tutorialSwipe;

    public static TutorialCore Instance
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
        tutorialBlur.gameObject.SetActive(false);
        if (!GameManager.Instance.tutorialCompleted)
        {
            tutorialSwipe.gameObject.SetActive(true);
        }
        else
            tutorialSwipe.gameObject.SetActive(false);
    }
    public void CompleteLevel()
    {
        if (!GameManager.Instance.tutorialCompleted)
            tutorialBlur.gameObject.SetActive(true);
    }
    public void SwipeCompleted()
    {
        tutorialSwipe.gameObject.SetActive(false);

    }

}
