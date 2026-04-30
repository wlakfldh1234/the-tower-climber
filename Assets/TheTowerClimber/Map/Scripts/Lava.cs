using UnityEngine;

public class Lava : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float maxY = 32.0f; // 용암 최대 높이

    private PlayerHp playerHp;

    private void Start()
    {
        playerHp = FindFirstObjectByType<PlayerHp>();
    }
    private void Update()
    {
        if (transform.position.y < maxY)
        {
            // 플레이어 죽으면 용암 멈춤
            if (playerHp != null && playerHp.IsDead) return;

            if (transform.position.y < maxY)
            {
               transform.position = new Vector2(transform.position.x, transform.position.y + moveSpeed * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 용암에 닿으면 즉시 게임 오버
            PlayerHp playerHp = other.GetComponent<PlayerHp>();
            if (playerHp != null)
            {
                playerHp.ChangeHealth(-9999);
            }
        }
    }
}
