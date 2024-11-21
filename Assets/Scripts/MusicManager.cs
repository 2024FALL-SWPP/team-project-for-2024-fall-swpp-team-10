using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] public AudioClip backgroundMusic;
    [SerializeField][Range(0f, 1f)] public float musicVolume = 0.5f;
    [SerializeField] public bool loop = false;
    [SerializeField] public float pitch = 1f;

    private AudioSource audioSource;
    private GameObject[] fires;
    private AudioSource[] fireAudioSources;

    private void Awake()
    {
        // Setup audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.volume = musicVolume;
        audioSource.loop = loop;
        audioSource.pitch = pitch;
        PlayMusic();

        fires = GameObject.FindGameObjectsWithTag("Fire");
        if (fires != null)
        {
            fireAudioSources = new AudioSource[fires.Length];
            for (int i = 0; i < fires.Length; i++)
            {
                fireAudioSources[i] = fires[i].GetComponent<AudioSource>();
            }
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
        if (fires != null)
        {
            for (int i = 0; i < fires.Length; i++)
            {
                fireAudioSources[i].Pause();
            }
        }
    }

    public void ResumeMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.UnPause();
        }
        if (fires != null)
        {
            for (int i = 0; i < fires.Length; i++)
            {
                fireAudioSources[i].UnPause();
            }
        }
    }

    public void ChangeSpeed( float v)
    {
        if (audioSource != null)
        {
            audioSource.pitch = v;
        }
    }
}