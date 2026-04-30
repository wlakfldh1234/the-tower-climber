using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public enum MoveDirection
    {
        Horizontal, // 좌우
        Vertical // 위아래
    }

    [Header("Settings")]
    [SerializeField] private MoveDirection moveDirection = MoveDirection.Horizontal;
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private Transform currentPlayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // Mathf.PingPong으로 0 ~ moveDistance 값을 왕복
        float offset = Mathf.PingPong(Time.time * moveSpeed, moveDistance * 2) - moveDistance;

        Vector2 newPos;
        if (moveDirection == MoveDirection.Horizontal)
        {
            newPos = new Vector2(startPosition.x + offset, startPosition.y);
        }
        else
        {
            newPos = new Vector2(startPosition.x, startPosition.y + offset);
        }
        rb.MovePosition(newPos);
    }

    // 플레이어가 올라타면 부모로 설정 (같이 움직이게)
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            currentPlayer = other.collider.transform;
            currentPlayer.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            if (currentPlayer != null)
            {
                currentPlayer.SetParent(null);
                currentPlayer = null;
            }
        }
    }
}
