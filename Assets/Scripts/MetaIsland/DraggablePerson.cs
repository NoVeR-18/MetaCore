using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePerson : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private RectTransform rectTransform;
    private Canvas parentCanvas;  // Canvas, содержащий этот объект

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>(); // Получаем ссылку на Canvas
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IslandManager.Instance.cameraController._canScroling = false;
        originalParent = transform.parent;
        canvasGroup.blocksRaycasts = false;

        // Перемещаем объект на верхний уровень в Canvas
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

        // Проверяем Raycast для определения объекта House, как в предыдущем примере
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

        // Если домик не найден, возвращаем в исходное место
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;  // Возвращаем на исходную позицию в родительском объекте
    }
}
