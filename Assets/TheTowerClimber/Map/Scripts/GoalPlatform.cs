using UnityEngine;

public class GoalPlatform : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float targetY = 1.6f;

    private bool isActivated = false;
    private bool isArrived = false;
    private Transform player;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // 충돌 지점이 플랫폼 위쪽인지 확인
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // 충돌 노멀이 위쪽을 향하면 플레이어가 위에서 내려온 것
                if (contact.normal.y < -0.5f)
                {
                    isActivated = true;
                    player = collision.transform;
                    return;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            player = null;
        }
    }

    private void Update()
    {
        if (!isActivated || isArrived) return;

        if (transform.position.y < targetY)
        {
            float newY = transform.position.y + moveSpeed * Time.deltaTime;
            transform.position = new Vector2(transform.position.x, newY);

            if (player != null)
            {
                player.position = new Vector2(transform.position.x, player.position.y + moveSpeed * Time.deltaTime);
            }
        }
        else
        {
            transform.position = new Vector2(transform.position.x, targetY);
            isArrived = true;
            this.enabled = false;
            // Debug.Log("정상 도달!");
        }
    }
}