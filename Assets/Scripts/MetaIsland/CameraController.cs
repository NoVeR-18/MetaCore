using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [Header("Pan Settings")]
    public float panSpeed = 5f;
    public Vector2 panLimitX = new Vector2(-15, 0);
    public Vector2 panLimitZ = new Vector2(0, 25);

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minZoom = 7f;
    public float maxZoom = 25f;

    private Camera cam;
    private Vector2 lastTouchPosition;



    public float focusSpeed = 2.0f;
    public float targetDistance = 5.0f;
    public float targetFOV = 5f;
    private bool isFocusing = false;
    private Transform targetObject;
    private float initialFOV;
    private bool _canScroling = true;

    [SerializeField]
    private Vector3 distanceMultiply = new Vector3(0, 1.1f, -0.5f);
    void Start()
    {

#if UNITY_EDITOR
        panSpeed = 3f;
        zoomSpeed = 3f;
#endif
        cam = Camera.main;
    }

    void Update()
    {
        if (isFocusing && targetObject != null)
        {
            Vector3 targetPosition = targetObject.position + distanceMultiply * targetDistance;
            cam.transform.position = Vector3.Lerp(cam.transform.position, targetPosition, Time.deltaTime * focusSpeed);

            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetFOV, Time.deltaTime * focusSpeed);

            if (Vector3.Distance(cam.transform.position, targetPosition) < 0.1f && Mathf.Abs(cam.orthographicSize - targetFOV) < 0.1f)
            {
                isFocusing = false;
            }
        }
        else if (isFocusing)
        {

            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetFOV, Time.deltaTime * focusSpeed);

            if (Mathf.Abs(cam.orthographicSize - targetFOV) < 0.1f)
            {
                isFocusing = false;
            }
        }
        else
        {
            if (_canScroling)
            {
                HandlePan();
                HandleZoom();
            }
        }
    }

    void HandlePan()
    {
        Vector3 pos = transform.position;

#if UNITY_EDITOR || UNITY_STANDALONE

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            pos += Vector3.forward * panSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            pos += Vector3.back * panSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            pos += Vector3.left * panSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            pos += Vector3.right * panSpeed * Time.deltaTime;
#endif

        if (Input.touchCount == 1)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
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

        pos.x = Mathf.Clamp(pos.x, panLimitX.x, panLimitX.y);
        pos.z = Mathf.Clamp(pos.z, panLimitZ.x, panLimitZ.y);

        transform.position = pos;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }


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

    public void FocusOnObject(Transform obj)
    {
        initialFOV = cam.orthographicSize;
        targetObject = obj;
        targetFOV = 5f;
        isFocusing = true;
        _canScroling = false;
    }

    public void ResetCamera()
    {
        //cam.transform.position = initialPosition;
        targetFOV = initialFOV;
        targetObject = null;
        _canScroling = true;
        isFocusing = true;
        //isFocusing = false;
    }
}
