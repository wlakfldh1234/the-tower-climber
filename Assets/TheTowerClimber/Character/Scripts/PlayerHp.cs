using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHp : MonoBehaviour
{
    
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [SerializeField] Slider hpUI;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerStats playerStats;

    //get 함수 - 캡슐화
    public int GetCurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>();
        currentHealth = maxHealth;
        UpdateHPUI();
    }

    //set 함수 역할
    public void ChangeHealth(int amount)
    {
        if (IsDead) return; // 이미 죽었으면 무시

        // 데미지면 보호막 체크
        if (amount < 0 && playerStats != null && playerStats.HasShield())
        {
            playerStats.UseShield(); // 보호막 1회 소모
            Debug.Log("보호막이 데미지를 막았습니다!");
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateHPUI();

        if (IsDead)
        {
            animator.SetBool("IsDead", true);
            // 죽을 때 밑으로 떨어지는거 방지를 위해 RigidBody도 정지
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            StartCoroutine(ShowGameOverAfterDelay());
            // Debug.Log("게임 오버");
        }
        else
        {
            animator.SetTrigger("IsHit");
            SoundManager.Instance.PlayHit();
        }
    }

    private IEnumerator ShowGameOverAfterDelay()
    {
        // 죽는 애니메이션 끝날 때까지 대기
        yield return new WaitForSeconds(2f);
        GameManager.Instance.GameOver();
        SoundManager.Instance.PlayGameOver();
    }

    public void SetMaxHealth(int newMax)
    {
        maxHealth = newMax;
        currentHealth = newMax;
        UpdateHPUI();
    }
    
    // 스파이크 충돌 처리
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Spike"))
        {
            // 플레이어가 Spike 위쪽에 있을 때만 데미지
            if (transform.position.y > other.transform.position.y)
            {
                ChangeHealth(-10);
            }
        }
    }

    [ContextMenu("UpdateHPUI")]
    private void UpdateHPUI()
    {
        if(hpUI != null)
        {
            hpUI.value = (float)currentHealth / maxHealth;     
        }
    }
}
