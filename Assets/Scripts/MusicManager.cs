using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] public AudioClip backgroundMusic;
    [SerializeField][Range(0f, 1f)] public float musicVolume = 0.5f;
    [SerializeField] public bool loop = false;
    [SerializeField] public float pitch = 1f;

    protected AudioSource audioSource;


    protected virtual void Awake()
    {
        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.volume = musicVolume;
        audioSource.loop = loop;
        audioSource.pitch = pitch;
        PlayMusic();
    }

    public virtual void PlayMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public virtual void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public virtual void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp(volume, 0f, 1f);
        }
    }

    public virtual void PauseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }

    }

    public virtual void ResumeMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.UnPause();
        }

    }

    public virtual void ChangeSpeed(float v)
    {
        if (audioSource != null)
        {
            audioSource.pitch = v;
        }
    }
}