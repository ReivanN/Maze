using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] musicClips;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioSource musicSource;
    private List<AudioSource> sfxSources = new List<AudioSource>();

    public static MusicManager Instance { get; private set; }

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
        }
    }

    private void OnDestroy()
    {
        // Отписка чтобы не было ошибок при уничтожении
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = false;
        musicSource.volume = musicVolume;

        //RefreshAudioSources();
        PlayRandomTrack();
    }

    void Update()
    {
        if (!musicSource.isPlaying)
        {
            PlayRandomTrack();
        }
    }

    /// <summary>
    /// Автоматически вызывается после загрузки сцены
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //RefreshAudioSources();
        Debug.LogError("UPDATED!");
    }


    /// <summary>
    /// Находит все AudioSource в сцене, кроме главного музыкального
    /// </summary>
    /*public void RefreshAudioSources()
    {
        sfxSources.Clear();
        AudioSource[] allSources = FindAnyObjectByType<AudioSource>();

        foreach (var src in allSources)
        {
            if (src != musicSource) // чтобы не задеть музыку
                sfxSources.Add(src);
        }

        ApplyVolumes();
    }*/

    /// <summary>
    /// Установить громкость музыки
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (musicSource != null)
            musicSource.volume = volume;
    }

    /// <summary>
    /// Установить громкость эффектов (все остальные AudioSource)
    /// </summary>
    public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;
        foreach (var src in sfxSources)
        {
            if (src != null)
                src.volume = volume;
        }
    }

    private void ApplyVolumes()
    {
        SetMusicVolume(musicVolume);
        SetSfxVolume(sfxVolume);
    }

    private void PlayRandomTrack()
    {
        if (musicClips.Length == 0) return;

        AudioClip clip = musicClips[Random.Range(0, musicClips.Length)];
        musicSource.clip = clip;
        musicSource.Play();
    }
}
