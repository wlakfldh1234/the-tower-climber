using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private TextMeshProUGUI shopGoldText;
    [SerializeField] private Button refundButton;
    [SerializeField] private TextMeshProUGUI refundButtonText;
    [SerializeField] private TextMeshProUGUI notificationText; // 알림 텍스트

    [Header("Item Button (순서대로 HP/Speed/Jump/Shield")]
    [SerializeField] private Button[] buyButtons;
    [SerializeField] private TextMeshProUGUI[] buyButtonTexts;

    [Header("Settings")]
    [SerializeField] private float fadeTime = 1.5f;

    // Array - 상점 카탈로그
    private ItemData[] shopItems;
    // Queue - 구매 요청 대기열
    private Queue<int> buyRequestQueue = new Queue<int>();
    // Stack - 구매 이력 (환불용)
    private Stack<ItemData> purchaseHistory = new Stack<ItemData>();
    // List - 플레이어 인벤토리 (동적)
    private List<ItemData> inventory = new List<ItemData>();

    private Coroutine currentFadeCoroutine;

    private void Awake()
    {
        // Array 초기화 - 판매 아이템 4종 등록
        shopItems = new ItemData[]
        {
            new ItemData { name = "HP 강화", price = 100, description = "최대 HP 증가", type = "HP", value = 20, canBuy = true },
            new ItemData { name = "속도 강화", price = 120, description = "이동 속도 증가", type = "Speed", value = 0.3f, canBuy = true },
            new ItemData { name = "점프 강화", price = 130, description = "점프 높이 증가", type = "Jump", value = 0.3f, canBuy = true },
            new ItemData { name = "보호막", price = 150, description = "첫 데미지 무효", type = "Shield", value = 1, canBuy = true }
        };

        // PlayerStats 자동으로 찾기
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
        }

        // 알림 텍스트 초기 숨김
        if (notificationText != null)
        {
            Color c = notificationText.color;
            c.a = 0f;
            notificationText.color = c;
        }
    }

    private void OnEnable()
    {
        // 상점 열릴 때마다 UI 갱신
        RefreshUI();
    }

    private void Update()
    {
        // 대기열에 요청이 있으면 순서대로 처리
        ProcessBuy();
    }

    // 구매 버튼 클릭 -> Enqueue
    public void RequestBuy(int itemIndex)
    {
        // 이미 구매 불가면 요청 무시
        if (!shopItems[itemIndex].canBuy) return;

        buyRequestQueue.Enqueue(itemIndex);
    }

    // 구매 처리 -> Dequeue
    private void ProcessBuy()
    {
        if (buyRequestQueue.Count > 0)
        {
            int index = buyRequestQueue.Dequeue();
            BuyItem(index);
        }
    }

    private void BuyItem(int index)
    {
        ItemData item = shopItems[index];

        // 골드 부족 체크
        if (GameManager.Instance.GetGold() < item.price)
        {
            ShowNotification("골드가 부족합니다..", Color.red);
            return;
        }

        // 골드 차감
        GameManager.Instance.SpendGold(item.price);

        // 스탯 적용
        playerStats.ApplyStat(item);

        // List - 인벤토리에 추가
        GameManager.Instance.savedInventory.Add(item);

        // Stack - 구매 이력에 Push
        purchaseHistory.Push(item);

        // 구매 불가 상태로 변경
        shopItems[index].canBuy = false;

        // 효과음
        SoundManager.Instance.PlayPurchase();

        ShowNotification("구매 성공!", Color.green);

        // UI 갱신
        RefreshUI();
    }

    // 환불 버튼
    public void RefundLastItem()
    {
        if (purchaseHistory.Count == 0) return;

        // Stack - 가장 최근 구매 Pop
        ItemData item = purchaseHistory.Pop();

        // 골드 반환
        GameManager.Instance.AddGold(item.price);

        // 스탯 원상복구
        playerStats.RevertStat(item);

        // List - 인벤토리에서 제거
        GameManager.Instance.savedInventory.Remove(item);

        // 환불한 아이템을 다시 구매 가능하게
        for (int i = 0; i < shopItems.Length; i++)
        {
            if (shopItems[i].name == item.name)
            {
                shopItems[i].canBuy = true;
                break;
            }
        }
        // 효과음
        SoundManager.Instance.PlayRefund();

        ShowNotification("환불 완료!", Color.yellow);
        // UI 갱신
        RefreshUI();
    }

    // 범용 알림 표시
    private void ShowNotification(string message, Color color)
    {
        if (notificationText == null) return;

        notificationText.text = message;

        Color c = color;
        c.a = 1f;
        notificationText.color = c;

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }
        currentFadeCoroutine = StartCoroutine(FadeOutNotification());
    }

    private IEnumerator FadeOutNotification()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        Color c = notificationText.color;
        float timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer / fadeTime);
            notificationText.color = c;
            yield return null;
        }
        c.a = 0f;
        notificationText.color= c;
    }

    private void RefreshUI()
    {
        // 골드 표시
        if (shopGoldText != null)
        {
            shopGoldText.text = $"보유 골드 : {GameManager.Instance.GetGold()}";
        }

        // 각 아이템 구매 버튼 상태 업데이트
        for (int i = 0; i < buyButtons.Length; i++)
        {
            if (buyButtons[i] == null) continue;

            bool canBuy = shopItems[i].canBuy;

            // 보호막 아이템이고, 이미 보호막 보유 중이면 구매 불가
            if (shopItems[i].type == "Shield" && GameManager.Instance.savedHasShield)
            {
                canBuy = false;
            }

            buyButtons[i].interactable = canBuy;

            if (buyButtonTexts[i] != null)
            {
                buyButtonTexts[i].text = canBuy ? "구매하기" : "구매불가";
            }
        }

        // 환불 버튼 - Stack.Peek로 마지막 구매 아이템 표시
        if (refundButton != null)
        {
            bool canRefund = purchaseHistory.Count > 0;
            refundButton.interactable = canRefund;

            if (refundButtonText != null)
            {
                refundButtonText.text = canRefund
                    ? $"환불: {purchaseHistory.Peek().name}"
                    : "환불 불가";
            }
        }
    }
    public List<ItemData> GetInventory() => GameManager.Instance.savedInventory;
}
