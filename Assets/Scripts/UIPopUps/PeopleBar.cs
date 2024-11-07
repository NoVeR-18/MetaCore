using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PeopleBar : MonoBehaviour
{
    public Image progressBar;  // ������ �� UI-����������� (Slider)
    public TextMeshProUGUI progressText;   // ����� ��� ����������� ���������� ��������� ����������
    public List<House> houses;  // ������ ���� ������� �� �����

    private int totalResidents;     // ����� ���������� ��������� ����������
    private int maxCapacity;        // ����� ������������ �����������

    private void Start()
    {
        UpdateProgress();
    }
    public void InitBar(House house)
    {
        if (!houses.Contains(house))
        {
            houses.Add(house);
        }
    }
    // ����� ��� ���������� ���������
    public void UpdateProgress()
    {
        totalResidents = 0;
        maxCapacity = 0;

        // ������� ������ ���������� ��������� ���������� � ������������ �����������
        foreach (House house in houses)
        {
            totalResidents += house.currentResidents;  // �������� ������� ���������� �������
            maxCapacity += house.capacity;             // �������� ������������ �����������
        }

        // ��������� ������� �������������
        float progress = (float)totalResidents / maxCapacity;

        // ��������� ����������� � �����
        progressBar.fillAmount = progress;
        progressText.text = $"{totalResidents} / {maxCapacity}";
    }
}
