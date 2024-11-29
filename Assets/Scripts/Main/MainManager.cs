using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EnumManager;
using TMPro;
using UnityEngine.EventSystems;
public class MainManager : StageManager
{
    /*[Header("UI Setting")]
    public GameObject[] characters;
    public GameObject[] characterUI;
    public GameObject[] hearts;
    public GameObject score;
    private TextMeshProUGUI scoreText;

    [Header("other UI")]
    public GameObject pause;
    public GameObject gameOver;
    public GameObject boss;

    private bool isGameOver = false;
    private bool isSpawnStopped = false;
    private MusicManager musicManager;

    // Stage End Condition Variables
    [Header("Stage End Condition Settings")]
    public float stageDuration = 3.0f; // Length of the main stage
    private float currentStageTime = 0f;
    private bool isStageComplete = false;
    private StageTransitionManager transitionManager;*/
    [Header("MainStage End Condition Settings")]
    [SerializeField] float stageDuration = 3.0f;
    private float currentStageTime = 180.0f;
    private StageTransitionManager transitionManager;
    public GameObject bossLandingParticle;
    float bossDropSpeed = 10f;

    GameObject activeCharacter;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        maxLife = GameManager.inst.maxLife;
        GameManager.inst.ResetStats();
        transitionManager = FindObjectOfType<StageTransitionManager>();
        GameManager.inst.CursorActive(false);
        StartCoroutine(AddScoreEverySecond());
    }

    // Update is called once per frame
    /*void Update()
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
            GameManager.inst.CursorActive(true);
            gameOver.SetActive(true);
            isGameOver = true;
            if (musicManager != null)
            {
                musicManager.StopMusic();
            }
        }

        if (!isSpawnStopped && stageDuration - currentStageTime < 5.0f)
        {
            isSpawnStopped = true;
        }

        if (!isStageComplete)
        {
            currentStageTime += Time.deltaTime;
            // Check if stage duration is complete
            if (currentStageTime >= stageDuration)
            {
                StartCoroutine(CompleteStage());
            }
        }
    }*/
    protected override void Update()
    {
        base.Update();

        // �������� �Ϸ� üũ
        if (!isStageComplete)
        {
            currentStageTime += Time.deltaTime;
            // Check if stage duration is complete
            if (currentStageTime >= stageDuration)
            {
                StartCoroutine(CompleteStage());
            }
        }
    }
    private IEnumerator CompleteStage()
    {
        isStageComplete = true;

        boss = Instantiate(boss, new Vector3(0, 13, activeCharacter.transform.position.z + 3), Quaternion.Euler(0, 180, 0));
        while (boss.transform.position.y >= 2)
        {
            boss.transform.Translate(Vector3.down * bossDropSpeed * Time.deltaTime, Space.World);
            yield return null;
        }
        Instantiate(bossLandingParticle, boss.transform.position - new Vector3(0, 0, 0.6f), boss.transform.rotation);
        yield return new WaitForSecondsRealtime(2.5f);
        if (musicManager != null)
        {
            musicManager.PauseMusic();
        }
        yield return new WaitForSeconds(0.5f);
        AddScoreBasedOnLives();
        // Freeze the game
        Time.timeScale = 0f;



        // Start the transition sequence
        if (transitionManager != null)
        {
            yield return StartCoroutine(transitionManager.StartStageTransition());
        }
    }

    /*private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }*/

    /*void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int characterIndex = (int)GameManager.inst.GetCharacter();
        characters[characterIndex].SetActive(true);
        characterUI[characterIndex].SetActive(true);
        activeCharacter = characters[characterIndex];
        transitionManager.SetCurrentCharacter(activeCharacter);
        Time.timeScale = 1;
        GameManager.inst.ResetStats();
        isGameOver = false;
        isSpawnStopped = false;

        characters[(int)GameManager.inst.GetCharacter()].GetComponent<PlayerControl>().ChangeColorOriginal();
    }*/
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        base.OnSceneLoaded(scene, mode);
        transitionManager.SetCurrentCharacter(activeCharacter);
        activeCharacter.GetComponent<PlayerControl>().ChangeColorOriginal();
    }

    /*private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Pause()
    {
        pause.SetActive(true);
        GameManager.inst.CursorActive(true);
        Time.timeScale = 0;
        if (musicManager != null)
        {
            musicManager.PauseMusic();
        }
    }

    public void Resume()
    {
        pause.SetActive(false);
        GameManager.inst.CursorActive(false);
        Time.timeScale = 1;
        if (musicManager != null)
        {
            musicManager.ResumeMusic();
        }
        EventSystem.current.SetSelectedGameObject(null);
    }*/
    protected override void PauseGame()
    {
        base .PauseGame();
        GameManager.inst.CursorActive(true);
    }

    public override void ResumeGame()
    {
        base.ResumeGame();
        GameManager.inst.CursorActive(false);
    }

    // ���� ���� ó��
    protected override void HandleGameOver()
    {
        base.HandleGameOver();
        GameManager.inst.CursorActive(true);
    }
    private IEnumerator AddScoreEverySecond()
    {
        while (GameManager.inst.GetLife() > 0 && !isStageComplete)
        {
            yield return new WaitForSeconds(1.0f);
            GameManager.inst.AddScore(100);
        }
    }

    public bool IsSpawnStopped()
    {
        return isSpawnStopped;
    }

    public bool IsStageComplete()
    {
        return isStageComplete;
    }
}
