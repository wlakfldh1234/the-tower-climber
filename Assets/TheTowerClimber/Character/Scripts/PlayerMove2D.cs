using UnityEngine;

public class PlayerMove2D : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D rb;
    private PlayerInputReader playerInputReader;
    private Animator animator;
    private PlayerHp playerHp;

    [Header("Move")]
    public float moveSpeed = 3.0f;

    [Header("Jump")]
    public float jumpForce = 5.0f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    [Header("Animation")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float inputX;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerHp = GetComponent<PlayerHp>();
        if (playerInputReader == null)
            playerInputReader = GetComponent<PlayerInputReader>();
    }

    private void Update()
    {
        // 죽으면 입력 무시
        if (playerHp != null && playerHp.IsDead)
        {
            inputX = 0;
            UpdateAnimation();
            return;
        }
        inputX = playerInputReader != null ? playerInputReader.MoveVector.x : 0f;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && playerInputReader != null && playerInputReader.JumpPressedThisFrame)
        {
            Jump();
        }

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        animator.SetTrigger("IsJumping");
        SoundManager.Instance.PlayJump();
    }

    private void UpdateAnimation()
    {
        animator.SetFloat("Speed", Mathf.Abs(inputX));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsFalling", !isGrounded && rb.linearVelocity.y < 0);

        if (inputX > 0.1f) spriteRenderer.flipX = false;
        else if (inputX < -0.1f) spriteRenderer.flipX = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}