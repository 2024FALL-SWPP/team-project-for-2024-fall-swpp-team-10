using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] public AudioClip backgroundMusic;
    protected AudioClip musicToPlay;
    [SerializeField][Range(0f, 1f)] public float musicVolume = 0.5f;
    [SerializeField] public bool loop = false;
    [SerializeField] public float pitch = 1f;

    protected AudioSource audioSource;


    protected virtual void Awake()
    {
        // Setup audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        SetMusic();
        audioSource.clip = musicToPlay;
        audioSource.volume = musicVolume;
        audioSource.loop = loop;
        audioSource.pitch = pitch;
        PlayMusic();
    }

    protected virtual void SetMusic()
    {
        musicToPlay = backgroundMusic;
    }

    public void PlayMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp(volume, 0f, 1f);
        }
    }

    public void PauseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }

    }

    public void ResumeMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.UnPause();
        }

    }

    public void ChangeSpeed(float v)
    {
        if (audioSource != null)
        {
            audioSource.pitch = v;
        }
    }
}