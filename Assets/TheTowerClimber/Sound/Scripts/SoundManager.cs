using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Slider")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("BGM")]
    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private AudioClip gameBGM;
    [SerializeField] private AudioClip clearBGM;

    [Header("SFX")]
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip hitSFX;
    [SerializeField] private AudioClip gameOverSFX;
    [SerializeField] private AudioClip stageClearSFX;
    [SerializeField] private AudioClip purchaseSFX;
    [SerializeField] private AudioClip refundSFX;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 코드로 AudioSource 생성
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
    }

    private void Start()
    {
        float bgmVolume = 0.05f;
        float sfxVolume = 0.5f;

        if (bgmSlider != null)
        {
            bgmSlider.value = bgmVolume;
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVolume;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);

        PlayTitleBGM();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Title")
        {
            PlayTitleBGM();
            SetBGMVolume(0.05f);
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1.0f));

            // Title 씬 슬라이더 다시 연결
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                Slider bgm = canvas.transform.Find("OptionUI/Bgm/BgmSlider")?.GetComponent<Slider>();
                Slider sfx = canvas.transform.Find("OptionUI/Sfx/SfxSlider")?.GetComponent<Slider>();

                if (bgm != null)
                {
                    bgm.value = PlayerPrefs.GetFloat("BGMVolume", 0.05f);
                    bgm.onValueChanged.RemoveAllListeners();
                    bgm.onValueChanged.AddListener(SetBGMVolume);
                }

                if (sfx != null)
                {
                    sfx.value = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
                    sfx.onValueChanged.RemoveAllListeners();
                    sfx.onValueChanged.AddListener(SetSFXVolume);
                }
            }
        }
        else if (scene.name == "GameClear")
        {
            PlayClearBGM();
        }
        else if (scene.name.StartsWith("Stage"))
        {
            PlayGameBGM();
            SetBGMVolume(0.8f);
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1.0f));

            // Stage 씬 슬라이더에 저장된 값 넣기
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                Slider bgm = canvas.transform.Find("PauseUI/Bgm/BgmSlider")?.GetComponent<Slider>();
                Slider sfx = canvas.transform.Find("PauseUI/Sfx/SfxSlider")?.GetComponent<Slider>();

                if (bgm != null)
                {
                    bgm.value = 0.5f;
                    bgm.onValueChanged.RemoveAllListeners();
                    bgm.onValueChanged.AddListener(SetBGMVolume);
                }

                if (sfx != null)
                {
                    sfx.value = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
                    sfx.onValueChanged.RemoveAllListeners();
                    sfx.onValueChanged.AddListener(SetSFXVolume);
                }
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // BGM
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlayTitleBGM() => PlayBGM(titleBGM);
    public void PlayGameBGM() => PlayBGM(gameBGM);
    public void PlayClearBGM() => PlayBGM(clearBGM);

    // SFX
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    public void PlayButtonClick() => PlaySFX(buttonClickSFX);
    public void PlayJump() => PlaySFX(jumpSFX);
    public void PlayHit() => PlaySFX(hitSFX);
    public void PlayGameOver() => PlaySFX(gameOverSFX);
    public void PlayStageClear() => PlaySFX(stageClearSFX);
    public void PlayPurchase() => PlaySFX(purchaseSFX);
    public void PlayRefund() => PlaySFX(refundSFX);

    // 볼륨 조절 (슬라이더)
    public void SetBGMVolume(float value)
    {
        bgmSource.volume = value;
        PlayerPrefs.SetFloat("BGMVolume", value);
    }
    
    public void SetSFXVolume(float value)
    {
        sfxSource.volume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
}
