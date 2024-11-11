using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EnumManager;

public class MainManager : MonoBehaviour
{
    public GameObject[] characters;
    public GameObject[] characterUI;
    public GameObject[] hearts;

    public GameObject pause;
    public GameObject gameOver;
    private bool isGameOver = false;
    // Start is called before the first frame update
    void Start()
    {
        // DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            Pause();
        }

        for (int i = 0; i < GameManager.inst.maxLife; i++)
            hearts[i].SetActive(i < GameManager.inst.GetLife());

        if (GameManager.inst.GetLife() <= 0)
        {
            Time.timeScale = 0;
            gameOver.SetActive(true);
            isGameOver = true;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        characters[(int)GameManager.inst.GetCharacter()].SetActive(true);
        characterUI[(int)GameManager.inst.GetCharacter()].SetActive(true);
        Time.timeScale = 1;
        isGameOver = false;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Pause()
    {
        pause.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        pause.SetActive(false);
        Time.timeScale = 1;
    }
}
