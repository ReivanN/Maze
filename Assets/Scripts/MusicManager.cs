using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] musicClips;
    public float volume = 0.7f;

    private AudioSource audioSource;
    public static MusicManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.volume = volume;

        PlayRandomTrack();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayRandomTrack();
        }
    }

    private void PlayRandomTrack()
    {
        if (musicClips.Length == 0) return;

        AudioClip clip = musicClips[Random.Range(0, musicClips.Length)];
        audioSource.clip = clip;
        audioSource.Play();
    }
}
