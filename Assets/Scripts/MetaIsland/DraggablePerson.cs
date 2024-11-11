using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePerson : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private RectTransform rectTransform;
    private Canvas parentCanvas;  // Canvas, ���������� ���� ������

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>(); // �������� ������ �� Canvas
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IslandManager.Instance.cameraController._canScroling = false;
        originalParent = transform.parent;
        canvasGroup.blocksRaycasts = false;

        // ���������� ������ �� ������� ������� � Canvas
        transform.SetParent(parentCanvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            parentCanvas.worldCamera,
            out localPoint))
        {
            rectTransform.localPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        IslandManager.Instance.cameraController._canScroling = true;

        // ��������� Raycast ��� ����������� ������� House, ��� � ���������� �������
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            House house = hit.collider.GetComponent<House>();
            if (house != null && house.AddResident())
            {
                SavedPeoplePanel savedPeoplePanel = FindObjectOfType<SavedPeoplePanel>();
                savedPeoplePanel.RemovePerson(gameObject);
                Destroy(gameObject);
                return;
            }
        }

        // ���� ����� �� ������, ���������� � �������� �����
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;  // ���������� �� �������� ������� � ������������ �������
    }
}
