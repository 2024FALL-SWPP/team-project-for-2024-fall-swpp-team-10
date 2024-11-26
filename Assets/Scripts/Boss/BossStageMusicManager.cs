using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStageMusicManager : MusicManager
{
    private GameObject[] fires;
    private AudioSource[] fireAudioSources;
    [SerializeField] public AudioClip GameOverMusic;
    [SerializeField] public AudioClip VictoryMusic;

    protected override void Awake()
    {
        // �θ� Ŭ����(MusicManager)�� Awake ����
        base.Awake();

        base.audioSource.loop = true;

        // ���� �������� ���� �ʱ�ȭ �۾�
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
    public new void StopMusic()
    {
        base.StopMusic();
        if (fires != null)
        {
            for (int i = 0; i < fires.Length; i++)
            {
                fireAudioSources[i]?.Stop();
            }
        }
    }
    public new void PauseMusic()
    {
        base.PauseMusic();
        if (fires != null)
        {
            for (int i = 0; i < fires.Length; i++)
            {
                fireAudioSources[i]?.Pause();
            }
        }
    }

    public new void ResumeMusic()
    {
        base.ResumeMusic();
        if (fires != null)
        {
            for (int i = 0; i < fires.Length; i++)
            {
                fireAudioSources[i]?.UnPause();
            }
        }
    }
    public void PlayGameOverMusic()
    {
        base.audioSource.clip = GameOverMusic;
        base.audioSource.loop = false;
        base.PlayMusic();
    }
    public void PlayVictoryMusic()
    {
        base.audioSource.clip = VictoryMusic;
        base.audioSource.loop = false;
        base.PlayMusic();
    }

}
