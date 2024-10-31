using UnityEngine;

public class ExpansionTile : MonoBehaviour
{
    private IslandController islandController;
    public int RequiredLevel = 1; // ”ровень, необходимый дл€ разблокировки этой €чейки
    public bool IsUnlocked { get; private set; } = false;

    [SerializeField] private GameObject buyableIndicator; // »конка, показывающа€, что €чейку можно купить

    public void SetIslandController(IslandController controller)
    {
        islandController = controller;
    }

    public void UpdateVisibility(int currentIslandLevel)
    {
        if (currentIslandLevel < RequiredLevel)
        {
            // ячейка скрыта, если уровень острова недостаточен
            buyableIndicator.SetActive(false);
            gameObject.SetActive(false);
        }
        else if (currentIslandLevel == RequiredLevel && !IsUnlocked)
        {
            // ѕоказываем индикатор покупки, если уровень позвол€ет купить €чейку
            buyableIndicator.SetActive(true);
            gameObject.SetActive(true);
        }
        else
        {
            // ячейка уже разблокирована и видима
            buyableIndicator.SetActive(false);
        }
    }

    // –азблокировка €чейки
    public void Unlock()
    {
        IsUnlocked = true;
        buyableIndicator.SetActive(false);
        Debug.Log($"ячейка на {transform.position} разблокирована!");
    }

    private void OnMouseDown()
    {
        if (!IsUnlocked && buyableIndicator.activeSelf)
        {
            // ≈сли €чейка доступна дл€ покупки, разблокируем еЄ
            islandController.UnlockTile(this);
        }
        else
        {
            Debug.Log("ячейка недоступна дл€ покупки.");
        }
    }
}
