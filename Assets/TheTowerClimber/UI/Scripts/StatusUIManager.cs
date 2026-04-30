using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class StatusUIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject statusUI;
    [SerializeField] private TextMeshProUGUI hpStatText;
    [SerializeField] private TextMeshProUGUI speedStatText;
    [SerializeField] private TextMeshProUGUI jumpStatText;
    [SerializeField] private TextMeshProUGUI shieldStatText;
    [SerializeField] private TextMeshProUGUI inventoryText;

    private PlayerInputReader playerInputReader;

    private void Start()
    {
        playerInputReader = FindFirstObjectByType<PlayerInputReader>();
        statusUI.SetActive(false);
    }

    private void Update()
    {
        // 카운트다운 중에는 상태창 막기
        if (GameManager.Instance != null && GameManager.Instance.isCountingDown) return;

        // 일시정지 상태면 I(상태창) 키 막기
        if (GameManager.Instance != null && GameManager.Instance.IsPaused()) return;

        if (playerInputReader != null && playerInputReader.ToggleStatusPressedThisFrame)
        {
            ToggleStatus();
        }
    }

    private void ToggleStatus()
    {
        bool isActive = !statusUI.activeSelf;
        statusUI.SetActive(isActive);

        if (isActive)
        {
            UpdateStatusUI();
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private void UpdateStatusUI()
    {
        // GameManager에서 저장된 스탯 불러오기
        int currentHP = GameManager.Instance.savedMaxHP;
        float currentSpeed = GameManager.Instance.savedMoveSpeed;
        float currentJump = GameManager.Instance.savedJumpForce;
        bool hasShield = GameManager.Instance.savedHasShield;

        // 텍스트 갱신
        hpStatText.text = $"체력: {currentHP}";
        speedStatText.text = $"이동속도: {currentSpeed:F1}";
        jumpStatText.text = $"점프력: {currentJump:F1}";
        shieldStatText.text = hasShield ? "보호막: O" : "보호막: X";

        // 보유 아이템 목록 표시 (List 사용)
        UpdateInventoryText();
    }

    private void UpdateInventoryText()
    {
        if (inventoryText == null) return;

        ShopManager shop = FindFirstObjectByType<ShopManager>(FindObjectsInactive.Include);

        if (shop == null)
        {
            inventoryText.text = "강화 상태:\n 없음";
            return;
        }

        List<ItemData> inventory = shop.GetInventory();
        Debug.Log($"inventory 개수: {inventory.Count}");

        if (inventory.Count == 0)
        {
            inventoryText.text = "강화 상태:\n 없음";
            return;
        }

        // 아이템 이름별로 개수 세기
        Dictionary<string, int> itemCount = new Dictionary<string, int>();
        foreach (ItemData item in inventory)
        {
            if (itemCount.ContainsKey(item.name))
            {
                itemCount[item.name]++;
            }
            else
            {
                itemCount[item.name] = 1;
            }
        }

        // 문자열로 조합
        string result = "강화 상태:\n";
        foreach (var kvp in itemCount)
        {
            result += $" - {kvp.Key} x{kvp.Value}\n";
        }

        inventoryText.text = result;
    }

    public bool IsStatusOpen()
    {
        return statusUI != null && statusUI.activeSelf;
    }
}
