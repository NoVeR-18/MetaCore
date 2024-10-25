using UnityEngine;

public class SpawnPossition : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    void Start()
    {
        LevelManager.Instance.Player.position = transform.position;
        spriteRenderer.enabled = false;
    }

}
