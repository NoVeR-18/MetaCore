using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private Animator animator;
        public float moveSpeed;
        private Vector3 moveDirection;
        private bool isMoving;
        private bool facingRight = true;
        private Rigidbody rb;
        int layerMask;
        public static bool CanMoving = true;

        private Vector3 startTouchPosition;
        private bool isSwiping = false;
        public float minSwipeDistance = 50f;

        public AudioSource audioSource;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            layerMask = LayerMask.GetMask("WallLayer");
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            if (!isMoving && CanMoving)
            {
                DetectSwipe();
                DetectKeyboardInput();
            }
        }

        void FixedUpdate()
        {
            if (isMoving)
            {
                rb.velocity = new Vector3(moveDirection.x, 0, moveDirection.z) * moveSpeed * Time.fixedDeltaTime;
                Physics.Raycast(transform.position, moveDirection, out RaycastHit hit, 0.6f, layerMask);
                if (hit.collider != null)
                {
                    rb.velocity = Vector3.zero;
                    transform.position = hit.point - moveDirection * 0.5f;
                    isMoving = false;
                    RotatePlayerToWall(moveDirection);
                }
            }
        }

        void DetectSwipe()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                startTouchPosition = Input.mousePosition;
                isSwiping = true;
            }
            else if (Input.GetMouseButton(0) && isSwiping)
            {
                Vector3 currentTouchPosition = Input.mousePosition;
                Vector2 swipeVector = (Vector2)(currentTouchPosition - startTouchPosition);

                if (swipeVector.magnitude > minSwipeDistance)
                {
                    CheckSwipeDirection(swipeVector);
                    startTouchPosition = currentTouchPosition;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isSwiping = false;
            }
#else
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Moved && isSwiping)
            {
                Vector3 currentTouchPosition = touch.position;
                Vector2 swipeVector = (Vector2)(currentTouchPosition - startTouchPosition);

                if (swipeVector.magnitude > minSwipeDistance)
                {
                    CheckSwipeDirection(swipeVector);
                    startTouchPosition = currentTouchPosition;
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isSwiping = false;
            }
        }
#endif
        }

        void CheckSwipeDirection(Vector2 swipeVector)
        {
            swipeVector.Normalize();
            GameManager.Instance.Vibrate();
            audioSource.PlayOneShot(audioSource.clip);
            if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
            {
                if (swipeVector.x > 0)
                {
                    moveDirection = Vector3.right;
                    if (!facingRight)
                    {
                        facingRight = !facingRight;
                    }
                }
                else
                {
                    moveDirection = Vector3.left;
                    if (facingRight)
                    {
                        facingRight = !facingRight;
                    }
                }
            }
            else
            {
                if (swipeVector.y > 0)
                {
                    moveDirection = Vector3.forward;
                }
                else
                {
                    moveDirection = Vector3.back;
                }
            }
            isMoving = true;
        }

        void DetectKeyboardInput()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                moveDirection = Vector3.forward;
                isMoving = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                moveDirection = Vector3.back;
                isMoving = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                moveDirection = Vector3.left;
                isMoving = true;
                if (facingRight)
                {
                    facingRight = !facingRight;
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                moveDirection = Vector3.right;
                isMoving = true;
                if (!facingRight)
                {
                    facingRight = !facingRight;
                }
            }
        }

        void RotatePlayerToWall(Vector3 moveDirection)
        {
            // Оставим пустым, если нужен поворот, можно раскомментировать и настроить.
        }

        public void PlayerDeath()
        {
            animator.SetTrigger("Explode");
            isMoving = false;
            rb.velocity = Vector3.zero;
            CanMoving = false;
        }

        public void DisableMove()
        {
            isMoving = false;
            rb.velocity = Vector3.zero;
        }
    }
}