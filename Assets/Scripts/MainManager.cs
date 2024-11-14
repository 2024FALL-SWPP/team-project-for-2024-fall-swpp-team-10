using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EnumManager;
using TMPro;

public class MainManager : MonoBehaviour
{
    public GameObject[] characters;
    public GameObject[] characterUI;
    public GameObject[] hearts;

    public GameObject pause;
    public GameObject gameOver;
    public GameObject score;
    private TextMeshProUGUI scoreText;

    private bool isGameOver = false;
    private MusicManager musicManager;
    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(AddScoreEverySecond());
        scoreText = score.GetComponent<TextMeshProUGUI>();
        // DontDestroyOnLoad(gameObject);
        musicManager = FindObjectOfType<MusicManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            Pause();
        }

        scoreText.text = "score\n" + GameManager.inst.GetScore().ToString();

        for (int i = 0; i < GameManager.inst.maxLife; i++)
            hearts[i].SetActive(i < GameManager.inst.GetLife());

        if (GameManager.inst.GetLife() <= 0)
        {
            Time.timeScale = 0;
            gameOver.SetActive(true);
            isGameOver = true;
            if (musicManager != null)
            {
                musicManager.StopMusic();
            }
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
        if (musicManager != null)
        {
            musicManager.PauseMusic();
        }
    }

    public void Resume()
    {
        pause.SetActive(false);
        Time.timeScale = 1;
        if (musicManager != null)
        {
            musicManager.ResumeMusic();
        }
    }

    private IEnumerator AddScoreEverySecond()
    {
        while (GameManager.inst.GetLife() > 0)
        {
            yield return new WaitForSeconds(1.0f);
            GameManager.inst.AddScore(100);
        }
    }
}
