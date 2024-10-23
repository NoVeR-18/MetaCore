using UnityEngine;

public class SpawnPossition : MonoBehaviour
{
    void Start()
    {
        LevelManager.Instance.Player.position = transform.position;
    }

}
