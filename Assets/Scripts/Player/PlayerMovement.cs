using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    public float moveSpeed;
    private Vector3 moveDirection;
    private bool isMoving;
    private bool facingRight = true;
    private Rigidbody rb;
    int layerMask;
    public bool playerAlive = true;

    private Vector3 startTouchPosition, endTouchPosition;
    private bool isSwiping = false;
    public float minSwipeDistance = 50f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        layerMask = LayerMask.GetMask("WallLayer");
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isMoving && playerAlive)
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
                //FlipCharacter();
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveDirection = Vector3.right;
            isMoving = true;
            if (!facingRight)
            {
                facingRight = !facingRight;
                //FlipCharacter();
            }
        }
    }

    void FlipCharacter()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void RotatePlayerToWall(Vector3 moveDirection)
    {
        //float angle = 0f;
        //if (moveDirection == Vector3.forward)
        //{
        //    angle = 180f;
        //}
        //else if (moveDirection == Vector3.back)
        //{
        //    angle = 0f;
        //}
        //else if (moveDirection == Vector3.left)
        //{
        //    angle = -90f;
        //}
        //else if (moveDirection == Vector3.right)
        //{
        //    angle = 90f;
        //}
        //transform.rotation = Quaternion.Euler(90, 0, angle);
    }

    public void PlayerDeath()
    {
        animator.SetTrigger("Explode");
        isMoving = false;
        rb.velocity *= 0f;
        playerAlive = false;
    }

    public void DisableMove()
    {
        isMoving = false;
        rb.velocity = Vector3.zero;
    }
}
