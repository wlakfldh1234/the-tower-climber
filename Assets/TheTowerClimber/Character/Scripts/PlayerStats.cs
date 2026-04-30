using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerHp playerHp;
    [SerializeField] private PlayerMove2D playerMove;
    [SerializeField] private GameObject shieldEffect;

    private bool hasShield = false;

    public void ApplySavedStats(int maxHP, float moveSpeed, float jumpForce, bool hasShield)
    {
        playerHp.SetMaxHealth(maxHP);
        playerMove.moveSpeed = moveSpeed;
        playerMove.jumpForce = jumpForce;
        this.hasShield = hasShield;

        // 보호막 이벤트 ON/OFF
        if (shieldEffect != null) shieldEffect.SetActive(hasShield);
    }

    public void ApplyStat(ItemData item)
    {
        if (item.type == "HP")
        {
            GameManager.Instance.savedMaxHP += (int)item.value;
            playerHp.SetMaxHealth(GameManager.Instance.savedMaxHP);
        }
        else if (item.type == "Speed")
        {
            GameManager.Instance.savedMoveSpeed += item.value;
            playerMove.moveSpeed += item.value;
        }
        else if (item.type == "Jump")
        {
            GameManager.Instance.savedJumpForce += item.value;
            playerMove.jumpForce += item.value;
        }
        else if (item.type == "Shield")
        {
            GameManager.Instance.savedHasShield = true;
            hasShield = true;
            if (shieldEffect != null) shieldEffect.SetActive(true);
        }
    }

    public void RevertStat(ItemData item)
    {
        if (item.type == "HP")
        {
            GameManager.Instance.savedMaxHP -= (int)item.value;
            playerHp.SetMaxHealth(GameManager.Instance.savedMaxHP);
        }
        else if (item.type == "Speed")
        {
            GameManager.Instance.savedMoveSpeed -= item.value;
            playerMove.moveSpeed -= item.value;
        }
        else if (item.type == "Jump")
        {
            GameManager.Instance.savedJumpForce -= item.value;
            playerMove.jumpForce -= item.value;
        }
        else if (item.type == "Shield")
        {
            GameManager.Instance.savedHasShield = false;
            hasShield = false;
            if (shieldEffect != null) shieldEffect.SetActive(false);
        }
    }
    public bool HasShield() => hasShield;

    public void UseShield()
    {
        hasShield = false;
        GameManager.Instance.savedHasShield = false;
        if (shieldEffect != null) shieldEffect.SetActive(false);
    }
}
