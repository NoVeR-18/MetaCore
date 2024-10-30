using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Pan Settings")]
    public float panSpeed = 5f; // Скорость перемещения камеры
    public Vector2 panLimitX = new Vector2(-15, 0); // Ограничение по X
    public Vector2 panLimitZ = new Vector2(0, 25); // Ограничение по Z

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f; // Скорость зума
    public float minZoom = 7f; // Минимальная дистанция до камеры
    public float maxZoom = 25f; // Максимальная дистанция до камеры

    private Camera cam;
    private Vector2 lastTouchPosition; // Позиция последнего касания для отслеживания движения

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandlePan();
        HandleZoom();
    }

    void HandlePan()
    {
        Vector3 pos = transform.position;

#if UNITY_EDITOR || UNITY_STANDALONE
        // Перемещение на ПК с использованием WASD или стрелок
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            pos += Vector3.forward * panSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            pos += Vector3.back * panSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            pos += Vector3.left * panSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            pos += Vector3.right * panSpeed * Time.deltaTime;
#endif

        // Панорамирование на смартфонах с помощью жеста drag
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.position - lastTouchPosition;
                pos -= new Vector3(delta.x * panSpeed * Time.deltaTime, 0, delta.y * panSpeed * Time.deltaTime);
                lastTouchPosition = touch.position;
            }
        }

        // Ограничиваем по X и Z
        pos.x = Mathf.Clamp(pos.x, panLimitX.x, panLimitX.y);
        pos.z = Mathf.Clamp(pos.z, panLimitZ.x, panLimitZ.y);

        transform.position = pos;
    }

    void HandleZoom()
    {
        // Зум на ПК с колесиком мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }

        // Зум на мобильных устройствах с помощью pinch-zoom
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            cam.orthographicSize -= difference * zoomSpeed * 0.1f;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }
}
