using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform Player;


    public static LevelManager Instance;

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
}
