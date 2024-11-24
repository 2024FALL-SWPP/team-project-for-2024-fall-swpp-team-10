using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using EnumManager;
using UnityEngine.EventSystems;
public class BossStageManager : MonoBehaviour
{
    public GameObject[] characters;
    public GameObject[] characterUI;
    public GameObject score;
    private TextMeshProUGUI scoreText;
    public GameObject gameOver;
    public BossAttackPattern bossScript; // BossAttackPattern 스크립트
    private bool isGameOver = false;
    public GameObject[] hearts;
    public GameObject[] Darkhearts;
    private BossStageMusicManager musicManager;
    private int bossMaxLife = 3;
    private int bossLife;
    private int currentPhase;
    public GameObject pause;


    /*void OnEnable()
    {
        // 씬 로드 완료 이벤트에 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("BossStageManager: Subscribed to sceneLoaded.");
    }

    void OnDisable()
    {
        // 씬 로드 완료 이벤트에서 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("BossStageManager: Unsubscribed from sceneLoaded.");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameManager.inst.ResetStats();
        while (GameManager.inst.GetLife() < GameManager.inst.bossStageMaxLife)
        {
            GameManager.inst.AddLife(GameManager.inst.bossStageMaxLife);
        }

        characters[(int)GameManager.inst.GetCharacter()].SetActive(true);
        characterUI[(int)GameManager.inst.GetCharacter()].SetActive(true);
        Time.timeScale = 1;
        isGameOver = false;
    }*/

    void Awake()
    {
        scoreText = score.GetComponent<TextMeshProUGUI>();
        musicManager = FindObjectOfType<BossStageMusicManager>();
        bossLife = bossMaxLife;
        currentPhase = 0;
    }

    void Start()
    {
        // life�� bossStageMaxLife�� ����
        while (GameManager.inst.GetLife() < GameManager.inst.bossStageMaxLife)
        {
            GameManager.inst.AddLife(GameManager.inst.bossStageMaxLife);
        }
        musicManager.ChangeSpeed(1.25f);
    }

    private void Update()
    {
        //scoreText.text = "score\n" + GameManager.inst.GetScore().ToString();
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            Pause();
        }

        for (int i = 0; i < GameManager.inst.bossStageMaxLife; i++)
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
        EventSystem.current.SetSelectedGameObject(null);
    }

    public int GetBossLife()
    {
        return bossLife;
    }

    public int GetBossMaxLife()
    {
        return bossMaxLife;
    }

    public void DecreaseBossLife()
    {
        bossLife -= 1;
    }

    public int GetPhase()
    {
        return currentPhase;
    }

    public void IncrementPhase()
    {
        currentPhase += 1;
        for (int i = 0; i < bossMaxLife; i++)
            Darkhearts[i].SetActive(i < bossLife);
    }
}
