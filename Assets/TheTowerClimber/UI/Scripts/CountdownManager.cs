using System.Collections;
using TMPro;
using UnityEngine;

public class CountdownManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI countdownText;

    [Header("Settings")]
    [SerializeField] private float fadeOutTime = 0.3f; // 페이드 아웃 시간
    [SerializeField] private float holdTime = 0.5f; // 텍스트 유지 시간

    private void Start()
    {
        Time.timeScale = 0f;
        GameManager.Instance.isCountingDown = true;
        StartCoroutine(StartCountDown());
    }

    private IEnumerator StartCountDown()
    {
        string[] texts = { "3", "2", "1", "Start!" };

        foreach (string t in texts)
        {
            // 바로 표시
            countdownText.text = t;
            countdownText.alpha = 1f;
            countdownText.gameObject.SetActive(true);

            // 유지
            yield return new WaitForSecondsRealtime(holdTime);

            // 페이드 아웃
            yield return StartCoroutine(FadeOut());
        }

        countdownText.gameObject.SetActive(false);
        Time.timeScale = 1f;
        GameManager.Instance.isCountingDown = false;
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;
        while (timer < fadeOutTime)
        {
            timer += Time.unscaledDeltaTime;
            countdownText.alpha = Mathf.Lerp(1f, 0f, timer / fadeOutTime);
            yield return null;
        }
        countdownText.alpha = 0f;
    }
}
