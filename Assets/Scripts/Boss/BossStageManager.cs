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
    public GameObject gameClear;
    public BossStageCamera cameraScript; // BossStageCamera ��ũ��Ʈ ����
    public BossStagePlayer playerScript; // BossStagePlayer ��ũ��Ʈ ����
    public BossControl bossControlScript; // BossControl ��ũ��Ʈ
    private bool isGameOver = false;
    private bool isGameClear = false;
    public GameObject[] hearts;
    public GameObject[] Darkhearts;
    private BossStageMusicManager musicManager;
    private int bossMaxLife = 3;
    private int currentPhase;
    public GameObject pause;
    public Camera mainCamera;
    public AudioClip heartDeactivateSound; // 하트->스코어 전환 효과음
    public AudioClip gameOverMusic; //게임오버 효과음
    public AudioClip victoryMusic; //게임클리어 효과음
    public float soundVolume = 0.7f; // 효과음 볼륨
    private GameObject[] fires;
    private GameClearLight gameClearLight; // GameClearLight 컴포넌트 참조
    public Transform player; //추후 삭제 예정

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
        currentPhase = 0;
        fires = GameObject.FindGameObjectsWithTag("Fire");
        gameClearLight = GetComponent<GameClearLight>();
        GameManager.inst.CursorActive(true);
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
        scoreText.text = "score\n" + GameManager.inst.GetScore().ToString();
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver && !isGameClear)
        {
            Pause();
        }

        if (!isGameClear)
        {
            for (int i = 0; i < GameManager.inst.bossStageMaxLife; i++)
                hearts[i].SetActive(i < GameManager.inst.GetLife());
        }

        if (GameManager.inst.GetLife() <= 0 && !isGameOver)
        {
            Time.timeScale = 0;
            gameOver.SetActive(true);
            isGameOver = true;
            if (musicManager != null)
            {
                musicManager.StopMusic();
                AudioSource.PlayClipAtPoint(gameOverMusic, Camera.main.transform.position, soundVolume);

            }
        }

        // End condition
        if (!bossControlScript.IsDead() && GetBossLife() <= 0 && !isGameClear)
        {
            isGameClear = true;
            StartCoroutine(HandleBossDeath());

        }
        //"Obstacle" 태그 오브젝트 비활성화
        if (isGameClear) 
        {
            GameObject[] obstacleObjects = GameObject.FindGameObjectsWithTag("Obstacle");
            foreach (GameObject obstacle in obstacleObjects)
            {
                obstacle.SetActive(false);
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

    private IEnumerator HandleBossDeath()
    {
        // 1. 슬로우 모션 적용
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // 2. 보스의 죽음 애니메이션 실행
        bossControlScript.BossDeath();

        // 3. 카메라가 보스 주위를 순회
        yield return StartCoroutine(cameraScript.OrbitAroundBoss());

        //(Optional)
        gameClearLight.ActivateLight(player);
        //gameClearLight.ActivateLight(characters[(int)GameManager.inst.GetCharacter()].transform);
        //(Optional)
        for (int i = 0; i < fires.Length; i++)
        {
            fires[i].SetActive(false);

        }
        // 4. 플레이어가 제자리에서 한 바퀴 회전
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        yield return StartCoroutine(playerScript.SpinInPlace());


        // 5. 연출 후 카메라 원상복구+점수 추가
        cameraScript.ResetCamera(); // 카메라를 원래 상태로 복구
        AddScoreBasedOnLives();

        // 6. 승리 음악 재생
        if (musicManager != null)
        {
            musicManager.StopMusic();
            AudioSource.PlayClipAtPoint(victoryMusic, Camera.main.transform.position, soundVolume);
        }
        gameClear.SetActive(true);
        isGameClear = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void AddScoreBasedOnLives()
    {
        StartCoroutine(GameManager.inst.DeactivateLivesAndAddScore(hearts, heartDeactivateSound, soundVolume));
    }

    public int GetBossLife()
    {
        return bossMaxLife - currentPhase;
    }

    public int GetBossMaxLife()
    {
        return bossMaxLife;
    }

    public int GetPhase()
    {
        return currentPhase;
    }

    public void IncrementPhase()
    {
        currentPhase += 1;
        for (int i = 0; i < bossMaxLife; i++)
            Darkhearts[i].SetActive(i < GetBossLife());
    }
}
