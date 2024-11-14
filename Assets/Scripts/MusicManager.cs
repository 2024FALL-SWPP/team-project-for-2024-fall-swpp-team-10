using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.5f;
    [SerializeField] private bool playOnAwake = true;
    [SerializeField] private bool loop = true;
    
    private AudioSource audioSource;
    private static MusicManager instance;

    // Singleton pattern to ensure only one music manager exists
    public static MusicManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        // Implement singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Setup audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.volume = musicVolume;
        audioSource.loop = loop;

        if (playOnAwake)
        {
            PlayMusic();
        }
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
}
