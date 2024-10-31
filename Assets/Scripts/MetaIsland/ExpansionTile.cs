using UnityEngine;

public class ExpansionTile : MonoBehaviour
{
    private IslandController islandController;
    public int RequiredLevel = 1; // �������, ����������� ��� ������������� ���� ������
    public bool IsUnlocked { get; private set; } = false;

    [SerializeField] private GameObject buyableIndicator; // ������, ������������, ��� ������ ����� ������

    public void SetIslandController(IslandController controller)
    {
        islandController = controller;
    }

    public void UpdateVisibility(int currentIslandLevel)
    {
        if (currentIslandLevel < RequiredLevel)
        {
            // ������ ������, ���� ������� ������� ������������
            buyableIndicator.SetActive(false);
            gameObject.SetActive(false);
        }
        else if (currentIslandLevel == RequiredLevel && !IsUnlocked)
        {
            // ���������� ��������� �������, ���� ������� ��������� ������ ������
            buyableIndicator.SetActive(true);
            gameObject.SetActive(true);
        }
        else
        {
            // ������ ��� �������������� � ������
            buyableIndicator.SetActive(false);
        }
    }

    // ������������� ������
    public void Unlock()
    {
        IsUnlocked = true;
        buyableIndicator.SetActive(false);
        Debug.Log($"������ �� {transform.position} ��������������!");
    }

    private void OnMouseDown()
    {
        if (!IsUnlocked && buyableIndicator.activeSelf)
        {
            // ���� ������ �������� ��� �������, ������������ �
            islandController.UnlockTile(this);
        }
        else
        {
            Debug.Log("������ ���������� ��� �������.");
        }
    }
}
