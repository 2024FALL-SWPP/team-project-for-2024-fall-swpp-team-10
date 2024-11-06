using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    public GameObject[] characters;
    public GameObject[] characterUI;

    public GameObject pause;
    public GameObject game;
    // Start is called before the first frame update
    void Start()
    {
        // DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause.SetActive(true);
            game.SetActive(false);
            Time.timeScale = 0;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        characters[GameManager.inst.GetCharacterInt()].SetActive(true);
        characterUI[GameManager.inst.GetCharacterInt()].SetActive(true);
        Time.timeScale = 1;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Resume()
    {
        pause.SetActive(false);
        game.SetActive(true);
        Time.timeScale = 1;
    }
}
