using UnityEngine;

public class GameClearManager : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 1f;
    }

    public void OnClickTitle()
    {
        SoundManager.Instance.PlayButtonClick();
        GameManager.Instance.GoToTitle();
    }

    public void OnClickQuit()
    {
        SoundManager.Instance.PlayButtonClick();
        // Unity Editor
        //UnityEditor.EditorApplication.isPlaying = false;
        // Build
        Application.Quit();
    }
}
