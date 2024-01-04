using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float climbSpeed = 5f;

    private float defaultGravity;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Animator animator;
    private BoxCollider2D playerBoxCollider;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        playerBoxCollider = GetComponent<BoxCollider2D>();
        defaultGravity = rb.gravityScale;
    }

    private void Start() {
    }


    private void Update() {
        FlipSprite();
        Move();
        ClimbLadder();
    }


    private void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value) {
        LayerMask groundLayer = LayerMask.GetMask("Ground");
        bool isPlayerTouchingGround = playerBoxCollider.IsTouchingLayers(groundLayer);

        if (value.isPressed && isPlayerTouchingGround) {
            animator.Play("Jumping");
            rb.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    private void FlipSprite() {
        if (moveInput.x != 0) {
            if (moveInput.x > 0) {
                sp.flipX = false;
            } else {
                sp.flipX = true;
            }
        }

    }

    private void Move() {
        bool isMoving = moveInput.x != 0;
        animator.SetBool("isRunning", isMoving);

        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;
    }

    private void ClimbLadder() {
        LayerMask climbingLayer = LayerMask.GetMask("Climbing");
        bool isPlayerTouchingLadder = playerBoxCollider.IsTouchingLayers(climbingLayer);

        if (isPlayerTouchingLadder) {
            bool isMovingVertical = moveInput.y != 0;

            if (isMovingVertical) {
                animator.speed = 1f;
                animator.SetBool("isClimbing", true);
            } else {
                if (animator.GetBool("isClimbing")) {
                    animator.speed = 0f;
                }
            }

            rb.gravityScale = 0f;
            Vector2 climbVelocity = new Vector2(rb.velocity.x, moveInput.y * climbSpeed);
            rb.velocity = climbVelocity;
        } else {
            rb.gravityScale = defaultGravity;
            animator.SetBool("isClimbing", false);
            animator.speed = 1f;
        }

    }

}
