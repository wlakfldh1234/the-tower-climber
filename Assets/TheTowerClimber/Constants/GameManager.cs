using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Stage")]
    [SerializeField] private int currentStage = 1;

    [Header("Gold")]
    [SerializeField] private int playerGold = 0;
    [SerializeField] private int[] goldRewardPerStage = { 100, 150, 200 }; // 스테이지별 골드

    [Header("UI")]
    private GameObject stageClearUI;
    private GameObject shopUI;
    private TextMeshProUGUI goldRewardText;
    private GameObject pauseUI;
    private GameObject gameOverUI;

    private bool isPaused = false;
    private PlayerInputReader playerInputReader;

    [Header("Saved Stats")]
    public int savedMaxHP = 100;
    public float savedMoveSpeed = 3.0f;
    public float savedJumpForce = 5.0f;
    public bool savedHasShield = false;


    public List<ItemData> savedInventory = new List<ItemData>();

    // 카운트 다운 중 일시정지 막기 위한 변수
    public bool isCountingDown = false;

    private void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 씬 전환될 때마다 호출
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 카운트다운 중에는 Pause 막기
        if (isCountingDown) return;

        StatusUIManager statusManager = FindFirstObjectByType<StatusUIManager>();
        if (statusManager != null && statusManager.IsStatusOpen()) return;

        if (playerInputReader != null &&
            playerInputReader.pauseAction != null &&
            playerInputReader.pauseAction.WasPressedThisFrame())
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }

        // 시연용 치트키
        if (Keyboard.current != null)
        {
            // F1 : 즉시 스테이지 클리어 (다음 스테이지로)
            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                StageClear();
            }

            // F2 : 골드 +500
            if (Keyboard.current.f2Key.wasPressedThisFrame)
            {
                AddGold(500);
            }

            // F3 : HP 회복
            if (Keyboard.current.f3Key.wasPressedThisFrame)
            {
                PlayerHp hp = FindFirstObjectByType<PlayerHp>();
                if (hp != null)
                {
                    hp.ChangeHealth(9999);
                }
            }

            // F4 : 즉시 게임 클리어 (GameClear 씬으로)
            if (Keyboard.current.f4Key.wasPressedThisFrame)
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("GameClear");
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 전환될 때마다 PlayerInputReader 다시 찾기
        playerInputReader = FindFirstObjectByType<PlayerInputReader>();
        isPaused = false;
        Time.timeScale = 1f;
        // Stage 씬일 때만 UI 찾기
        if (scene.name.StartsWith("Stage"))
        {
            // Canvas 안에서 비활성화된 오브젝트도 찾기
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                pauseUI = canvas.transform.Find("PauseUI")?.gameObject;
                stageClearUI = canvas.transform.Find("StageClearUI")?.gameObject;
                shopUI = canvas.transform.Find("ShopUI")?.gameObject;
                goldRewardText = canvas.transform.Find("StageClearUI/Gold/GoldRewardText")?.GetComponent<TMPro.TextMeshProUGUI>();
                gameOverUI = canvas.transform.Find("GameOverUI")?.gameObject;

                // 버튼 코드로 연결 (Missing 방지)
                foreach (Button btn in canvas.GetComponentsInChildren<Button>(true))
                {
                    if (btn.gameObject.name == "GoToTitleButton")
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(GoToTitle);
                        btn.onClick.AddListener(() => SoundManager.Instance.PlayButtonClick());
                    }
                    else if (btn.gameObject.name == "OpenShopButton")
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(OpenShop);
                        btn.onClick.AddListener(() => SoundManager.Instance.PlayButtonClick());
                    }
                    else if (btn.gameObject.name == "CloseShopButton")
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(CloseShop);
                        btn.onClick.AddListener(() => SoundManager.Instance.PlayButtonClick());
                    }
                    else if (btn.gameObject.name == "NextButton")
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(NextStage);
                        btn.onClick.AddListener(() => SoundManager.Instance.PlayButtonClick());
                    }
                    else if (btn.gameObject.name == "RetryButton")
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(Retry);
                        btn.onClick.AddListener(() => SoundManager.Instance.PlayButtonClick());
                    }
                }

                pauseUI?.SetActive(false);
                stageClearUI?.SetActive(false);
                shopUI?.SetActive(false);
                gameOverUI?.SetActive(false);

                PlayerStats stats = FindFirstObjectByType<PlayerStats>();
                if (stats != null)
                {
                    stats.ApplySavedStats(savedMaxHP, savedMoveSpeed, savedJumpForce, savedHasShield);
                }
            }
        }
    }

    public void StageClear()
    {
        // 골드 지급
        int reward = 0;
        if (currentStage <= goldRewardPerStage.Length)
        {
            reward = goldRewardPerStage[currentStage - 1];
            playerGold += reward;
        }

        if (stageClearUI != null)
        {
            // Stage Clear UI 열기
            stageClearUI.SetActive(true);
            goldRewardText.text = $"+{reward} Gold!!";
        }
       
        Time.timeScale = 0f; // 게임 일시 정지
        SoundManager.Instance.PlayStageClear();
    }

    // 상점 열기
    public void OpenShop()
    {
        if (shopUI != null) shopUI.SetActive(true);
    }

    // 상점 닫기
    public void CloseShop()
    {
        if (shopUI != null) shopUI.SetActive(false);
    }

    // 게임 일시 정지
    public void PauseGame()
    {
        isPaused = true;
        pauseUI?.SetActive(true);
        Time.timeScale = 0f;
    }

    public bool IsPaused() // 상태창에서 일시정지 막기 위함
    {
        return isPaused;
    }

    // 일시 정지 해제
    public void ResumeGame()
    {
        isPaused = false;
        pauseUI?.SetActive(false);
        Time.timeScale = 1f;
    }

    // 타이틀로 돌아가기
    public void GoToTitle()
    {
        // 모든 상태 초기화
        currentStage = 1;
        playerGold = 0;
        isPaused = false;
        Time.timeScale = 1f;

        // 스탯 초기화
        savedMaxHP = 100;
        savedMoveSpeed = 3.0f;
        savedJumpForce = 5.0f;
        savedHasShield = false;
        savedInventory.Clear();

        SceneManager.LoadScene("Title");
    }

    public void NextStage()
    {
        currentStage++;
        Time.timeScale = 1f;

        // 마지막 스테이지(10) 클리어 후엔 GameClear 씬으로 이동
        if (currentStage > 10)
        {
            SceneManager.LoadScene("GameClear");
        }
        else
        {
            SceneManager.LoadScene($"Stage{currentStage}");
        }
    }

    public void GameOver()
    {
        if (gameOverUI != null) gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬 다시 로드
    }

    // 골드 획득
    public int GetGold() => playerGold;

    // 골드 소모
    public bool SpendGold(int amount)
    {
        if (playerGold < amount)
        {
            Debug.Log("골드 부족");
            return false;
        }
        playerGold -= amount;
        return true;
    }

    public void AddGold(int amount)
    {
        playerGold += amount;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
