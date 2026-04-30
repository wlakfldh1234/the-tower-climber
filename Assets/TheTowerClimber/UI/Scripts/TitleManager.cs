using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class TitleManager : MonoBehaviour
{
    [Header("Option UI")]
    [SerializeField] private GameObject optionUI;

    [Header("Tip UI")]
    [SerializeField] private GameObject tipUI;

    // Start 버튼
    public void OnClickStart()
    {
        SoundManager.Instance.PlayButtonClick();
        SoundManager.Instance.PlayGameBGM();
        SceneManager.LoadScene("Stage1");
    }

    // Option 버튼
    public void OnClickOption()
    {
        SoundManager.Instance.PlayButtonClick();
        optionUI.SetActive(true);
    }

    public void OnClickTip()
    {
        SoundManager.Instance.PlayButtonClick();
        tipUI.SetActive(true);
    }

    public void OnClinkClose()
    {
        SoundManager.Instance.PlayButtonClick();
        optionUI.SetActive(false);
        tipUI.SetActive(false);
    }

    // Quit 버튼
    public void OnClickQuit()
    {
        SoundManager.Instance.PlayButtonClick();
        // 에디터
        // UnityEditor.EditorApplication.isPlaying = false;

        // 빌드
        Application.Quit();
    }
}
