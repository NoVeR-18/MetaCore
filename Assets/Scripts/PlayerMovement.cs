using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    public float moveSpeed;
    private Vector2 moveDirection;
    private bool isMoving;
    private bool facingRight = true;
    private Rigidbody2D rb;
    int layerMask;
    public bool playerAlive = true;

    private Vector2 startTouchPosition, endTouchPosition;
    private bool isSwiping = false;
    public float minSwipeDistance = 50f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        layerMask = LayerMask.GetMask("WallLayer");
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isMoving && playerAlive)
        {
            // Обработка ввода с телефона (свайпы)
            DetectSwipe();

            // Обработка ввода с ПК (клавиатура)
            DetectKeyboardInput();
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            rb.velocity = moveDirection * moveSpeed * Time.fixedDeltaTime;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, 0.6f, layerMask);
            if (hit.collider != null)
            {
                rb.velocity = Vector2.zero;
                transform.position = hit.point - (Vector2)moveDirection * 0.5f;
                isMoving = false;
                RotatePlayerToWall(moveDirection);
            }
        }
    }

    void DetectSwipe()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Ended && isSwiping)
            {
                endTouchPosition = touch.position;
                isSwiping = false;
                CheckSwipeDirection();
            }
        }
    }

    void CheckSwipeDirection()
    {
        Vector2 swipeVector = endTouchPosition - startTouchPosition;
        if (swipeVector.magnitude > minSwipeDistance)
        {
            swipeVector.Normalize();

            if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
            {
                // Горизонтальный свайп
                if (swipeVector.x > 0)
                {
                    moveDirection = Vector2.right;
                    if (!facingRight)
                    {
                        facingRight = !facingRight;
                        FlipCharacter();
                    }
                }
                else
                {
                    moveDirection = Vector2.left;
                    if (facingRight)
                    {
                        facingRight = !facingRight;
                        FlipCharacter();
                    }
                }
            }
            else
            {
                // Вертикальный свайп
                if (swipeVector.y > 0)
                {
                    moveDirection = Vector2.up;
                }
                else
                {
                    moveDirection = Vector2.down;
                }
            }
            isMoving = true;
        }
    }

    void DetectKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            moveDirection = Vector2.up;
            isMoving = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveDirection = Vector2.down;
            isMoving = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveDirection = Vector2.left;
            isMoving = true;
            if (facingRight)
            {
                facingRight = !facingRight;
                FlipCharacter();
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveDirection = Vector2.right;
            isMoving = true;
            if (!facingRight)
            {
                facingRight = !facingRight;
                FlipCharacter();
            }
        }
    }

    void FlipCharacter()
    {
        Vector2 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void RotatePlayerToWall(Vector2 moveDirection)
    {
        float angle = 0f;
        if (moveDirection == Vector2.up)
        {
            angle = 180f;
        }
        else if (moveDirection == Vector2.down)
        {
            angle = 0f;
        }
        else if (moveDirection == Vector2.left)
        {
            angle = -90f;
        }
        else if (moveDirection == Vector2.right)
        {
            angle = 90f;
        }
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void PlayerDeath()
    {
        animator.SetTrigger("Explode");
        isMoving = false;
        rb.velocity *= 0f;
        playerAlive = false;
    }
}
