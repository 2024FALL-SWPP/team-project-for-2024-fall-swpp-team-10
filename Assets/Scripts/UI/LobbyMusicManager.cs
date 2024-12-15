using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyMusicManager : MusicManager
{
    public static LobbyMusicManager inst;

    protected override void Awake()
    {
        if (LobbyMusicManager.inst == null)
        {
            LobbyMusicManager.inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        base.Awake();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Selection") || scene.name.Contains("MainMenu") || scene.name.Contains("Leader"))
            PlayMusic();
        else if (scene.name.Contains("MainStage") && audioSource.isPlaying)
            StopMusic();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (GameManager.inst.IsSelected() && audioSource.isPlaying)
        {
            StopMusic();
        }
    }
}
