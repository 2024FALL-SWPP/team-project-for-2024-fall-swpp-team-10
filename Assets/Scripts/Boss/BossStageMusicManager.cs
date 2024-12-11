using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStageMusicManager : MusicManager
{
    private GameObject[] fires;
    private AudioSource[] fireAudioSources;

    protected override void Awake()
    {
        // �θ� Ŭ����(MusicManager)�� Awake ����
        base.Awake();

        base.audioSource.loop = true;

        // ���� �������� ���� �ʱ�ȭ �۾�
        fires = FindGameObjectsWithTagIncludingInactive("Fire");
        if (fires != null)
        {
            fireAudioSources = new AudioSource[fires.Length];
            for (int i = 0; i < fires.Length; i++)
            {
                fireAudioSources[i] = fires[i].GetComponent<AudioSource>();
            }
        }
    }
    void Start() 
    { 
    
    }
    public override void StopMusic()
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
    public override void PauseMusic()
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

    public override void ResumeMusic()
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
    public static GameObject[] FindGameObjectsWithTagIncludingInactive(string tag)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        List<GameObject> taggedObjects = new List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.hideFlags == HideFlags.None && obj.CompareTag(tag))
            {
                taggedObjects.Add(obj);
            }
        }

        return taggedObjects.ToArray();
    }
}
