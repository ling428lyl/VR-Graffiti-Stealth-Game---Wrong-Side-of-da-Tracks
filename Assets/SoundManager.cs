using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI soundButtonText;
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private string[] singleTrackScenes;
    [SerializeField] private AudioClip singleTrack;
    private int currentTrackIndex = 0;
    private bool muted = false;
    private Coroutine musicCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevents destruction on scene load
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }
    }

    void Start()
    {
        backgroundMusicSource.ignoreListenerPause = true;

        if (!PlayerPrefs.HasKey("muted"))
        {
            muted = false;
            Save();
        }
        else
        {
            Load();
        }

        SceneManager.activeSceneChanged += OnSceneChanged;
        PlayMusicForCurrentScene();
        UpdateButtonText();
        UpdateAudio();
    }

    public void OnButtonPress()
    {
        muted = !muted;
        Save();
        UpdateButtonText();
        UpdateAudio();
    }

    private void UpdateButtonText()
    {
        soundButtonText.text = muted ? "Music On" : "Music Off";
    }

    private void UpdateAudio()
    {
        if (muted)
        {
            backgroundMusicSource.Pause();
            if (musicCoroutine != null)
            {
                StopCoroutine(musicCoroutine);
                musicCoroutine = null;
            }
        }
        else
        {
            backgroundMusicSource.UnPause();
            if (musicCoroutine == null && !backgroundMusicSource.isPlaying)
            {
                musicCoroutine = StartCoroutine(PlayNextTrackAfterCurrent());
            }
        }
    }

    private void Load()
    {
        muted = PlayerPrefs.GetInt("muted") == 1;
        UpdateButtonText();
    }

    private void Save()
    {
        PlayerPrefs.SetInt("muted", muted ? 1 : 0);
    }

    private void ShuffleMusicClips()
    {
        for (int i = musicClips.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            AudioClip temp = musicClips[i];
            musicClips[i] = musicClips[j];
            musicClips[j] = temp;
        }
    }

    private void PlayCurrentTrack()
    {
        if (musicClips.Length == 0) return;

        Debug.Log("Playing track: " + musicClips[currentTrackIndex].name);
        backgroundMusicSource.clip = musicClips[currentTrackIndex];
        backgroundMusicSource.Play();

        if (musicCoroutine != null)
        {
            StopCoroutine(musicCoroutine);
        }
        musicCoroutine = StartCoroutine(PlayNextTrackAfterCurrent());
    }

    private IEnumerator PlayNextTrackAfterCurrent()
    {
        while (backgroundMusicSource.isPlaying)
        {
            yield return null;
        }

        currentTrackIndex++;

        if (currentTrackIndex >= musicClips.Length)
        {
            currentTrackIndex = 0;
            ShuffleMusicClips();
        }

        PlayCurrentTrack();
    }

    private void PlayMusicForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (System.Array.Exists(singleTrackScenes, scene => scene == currentSceneName))
        {
            backgroundMusicSource.clip = singleTrack;
            backgroundMusicSource.loop = true;
            backgroundMusicSource.Play();
            if (musicCoroutine != null)
            {
                StopCoroutine(musicCoroutine);
                musicCoroutine = null;
            }
        }
        else
        {
            backgroundMusicSource.loop = false; // Ensure looping is disabled for the multi-track playlist
            ShuffleMusicClips();
            currentTrackIndex = 0;
            PlayCurrentTrack();
        }
    }

    private void OnSceneChanged(Scene previousScene, Scene newScene)
    {
        PlayMusicForCurrentScene();
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
}
