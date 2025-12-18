using System.Collections;
using UnityEngine;

public class Map_AudioManager : MonoBehaviour
{
    public static Map_AudioManager Instance;

    [Header("BGM Clips")]
    [SerializeField] private AudioClip battleBGM;
    [SerializeField] private AudioClip bossBGM;

    [Header("Settings")]
    [SerializeField] private float defaultVolume = 0.8f;
    [SerializeField] private float fadeTime = 0.5f;

    [SerializeField] private bool autoPlayBattleOnSceneStart = true;

    private AudioSource bgmSource;
    private Coroutine fadeRoutine;

    private void Start()
    {
        if (autoPlayBattleOnSceneStart)
        {
            PlayBattleBGM(forceRestart: false);
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSource = GetComponent<AudioSource>();
        if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = defaultVolume;
    }

    public void PlayBattleBGM(bool forceRestart = false)
    {
        PlayBGM(battleBGM, forceRestart);
    }

    public void PlayBossBGM(bool forceRestart = false)
    {
        PlayBGM(bossBGM, forceRestart);
    }

    private void PlayBGM(AudioClip clip, bool forceRestart = false)
    {
        if (clip == null) return;

        if (!forceRestart && bgmSource.isPlaying && bgmSource.clip == clip)
            return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeToClip(clip));
    }


    public void StopBGM(bool fadeOut = true)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);

        if (!fadeOut)
        {
            bgmSource.Stop();
            bgmSource.clip = null;
            return;
        }

        fadeRoutine = StartCoroutine(FadeOutAndStop());
    }

    private IEnumerator FadeToClip(AudioClip newClip)
    {
        // fade out
        float startV = bgmSource.volume;
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startV, 0f, t / fadeTime);
            yield return null;
        }
        bgmSource.volume = 0f;

        // switch clip
        bgmSource.clip = newClip;
        bgmSource.Play();

        // fade in
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0f, defaultVolume, t / fadeTime);
            yield return null;
        }
        bgmSource.volume = defaultVolume;
    }

    private IEnumerator FadeOutAndStop()
    {
        float startV = bgmSource.volume;
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startV, 0f, t / fadeTime);
            yield return null;
        }
        bgmSource.volume = 0f;

        bgmSource.Stop();
        bgmSource.clip = null;

        // 恢复默认音量（下次播放时正常）
        bgmSource.volume = defaultVolume;
    }
}
