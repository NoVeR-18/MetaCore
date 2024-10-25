using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IslandManager : MonoBehaviour
{
    public Button GoToLevelButton;
    private void Start()
    {
        GoToLevelButton.onClick.AddListener(() => { SceneManager.LoadScene("CoreScene"); });
    }
}
