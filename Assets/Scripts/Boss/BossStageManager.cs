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
    private MusicManager musicManager;
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
        if (scene.name == "BossStage1Scene") // BossStage1Scene 확인
        {
            if (GameManager.inst != null)
            {
                Debug.Log("BossStageManager: Setting maxLife to 5.");
                GameManager.inst.ResetStats();
                GameManager.inst.maxLife = 5;
                Debug.Log($"BossStageManager: maxLife is now {GameManager.inst.maxLife}.");

                // life를 maxLife로 설정
                int initialLife = GameManager.inst.GetLife();
                Debug.Log($"BossStageManager: Initial life before AddLife() - {initialLife}");
                while (GameManager.inst.GetLife() < GameManager.inst.maxLife)
                {
                    GameManager.inst.AddLife();
                    Debug.Log($"BossStageManager: Life increased to {GameManager.inst.GetLife()}.");
                }

                Debug.Log($"BossStageManager: Final life is {GameManager.inst.GetLife()}.");
            }
            else
            {
                Debug.LogWarning("BossStageManager: GameManager.inst is null.");
            }
            characters[(int)GameManager.inst.GetCharacter()].SetActive(true);
            characterUI[(int)GameManager.inst.GetCharacter()].SetActive(true);
            Time.timeScale = 1;
            isGameOver = false;

        }
    }*/

    void Awake()
    {
        scoreText = score.GetComponent<TextMeshProUGUI>();
        musicManager = FindObjectOfType<MusicManager>();
    }
    void Start()
    {

        Debug.Log("BossStageManager: Start called.");
        if (SceneManager.GetActiveScene().name == "BossStage1Scene")
        {
            if (GameManager.inst != null)
            {
                GameManager.inst.maxLife = 5;

                // life를 maxLife로 설정
                int initialLife = GameManager.inst.GetLife();
                while (GameManager.inst.GetLife() < GameManager.inst.maxLife)
                {
                    GameManager.inst.AddLife();
                }
            }
            else
            {
                Debug.LogWarning("BossStageManager: GameManager.inst is null in Start.");
            }
        }
        else
        {
            Debug.LogWarning($"BossStageManager: Current scene is {SceneManager.GetActiveScene().name}, expected BossStage1Scene.");
        }
    }
    private void Update()
    {
        //scoreText.text = "score\n" + GameManager.inst.GetScore().ToString();
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            Pause();
        }
        for (int i = 0; i < GameManager.inst.maxLife; i++)
            hearts[i].SetActive(i < GameManager.inst.GetLife());

        for (int i = 0; i < 3; i++)
            Darkhearts[i].SetActive(i < 3- bossScript.GetCurrentPhase());

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
}
