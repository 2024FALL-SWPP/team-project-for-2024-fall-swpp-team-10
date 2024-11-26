using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStageMusicManager : MusicManager
{
    private GameObject[] fires;
    private AudioSource[] fireAudioSources;

    protected override void Awake()
    {
        // 부모 클래스(MusicManager)의 Awake 실행
        base.Awake();

        base.audioSource.loop = true;

        // 보스 스테이지 전용 초기화 작업
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
}
