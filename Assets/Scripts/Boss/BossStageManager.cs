using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using EnumManager;
using UnityEngine.EventSystems;
public class BossStageManager : StageManager
{
    [Header("Boss Stage Settings")]
    public GameObject[] darkHearts; // 보스의 하트

    public GameObject gameClear;
    public BossStageCamera cameraScript;
    private BossStagePlayer playerScript;
    public BossControl bossControlScript;
    public WeakspotsManager weakspotsManagerScript;
    public int weakspotHitCount = 0; // Number of total hits on weakspot
    public AudioClip gameOverMusic; //게임오버 효과음
    public AudioClip victoryMusic; //게임클리어 효과음
    private GameObject[] fires;
    private GameClearLight gameClearLight;
    private int bossMaxLife = 3;
    private int currentPhase;
    public float carrotSpeed = 10f;
    [SerializeField] Animator transitionAnimator;
    [SerializeField] float introAnimationDuration;
    protected BossStageTransitionManager transitionManager;

    protected override void Awake()
    {
        base.Awake();
        transitionManager = FindObjectOfType<BossStageTransitionManager>();
        GameManager.inst.CursorActive(true);
        maxLife = GameManager.inst.bossStageMaxLife;
        currentPhase = 0;
        fires = GameObject.FindGameObjectsWithTag("Fire");
        gameClearLight = GetComponent<GameClearLight>();
        if (transitionManager != null && transitionAnimator != null)
        {
            transitionAnimator.gameObject.SetActive(true);
            introAnimationDuration = transitionManager.BossStageTransition();
            StartCoroutine("WaitForIntro");
            StartCoroutine(transitionManager.Countdown());
        }
    }

    protected virtual void Start()
    {
        playerScript = activeCharacter.GetComponent<BossStagePlayer>();
        base.isPausable = false;

        while (GameManager.inst.GetLife() < GameManager.inst.bossStageMaxLife)
        {
            GameManager.inst.AddLife(GameManager.inst.bossStageMaxLife);
        }
        musicManager.ChangeSpeed(1.25f);
    }

    public override void Update()
    {
        base.Update();

        // End condition
        if (!bossControlScript.IsDead() && GetBossLife() <= 0 && !isStageComplete)
        {
            base.isPausable = false;
            playerScript.SetEnableKeys(false);
            isStageComplete = true;
            StartCoroutine(HandleBossDeath());
        }

        // "Obstacle" 태그 오브젝트 비활성화
        if (isStageComplete)
        {
            GameObject[] obstacleObjects = GameObject.FindGameObjectsWithTag("Obstacle");
            foreach (GameObject obstacle in obstacleObjects)
            {
                obstacle.SetActive(false);
            }
        }
    }

    protected override void HandleGameOver()
    {
        base.HandleGameOver();
        playerScript.SetEnableKeys(false);
        AudioSource.PlayClipAtPoint(gameOverMusic, Camera.main.transform.position, soundVolume);
    }
    public override void PauseGame()
    {
        base.PauseGame();
        playerScript.SetEnableKeys(false);
    }

    public override void ResumeGame()
    {
        base.ResumeGame();
        playerScript.SetEnableKeys(true);
    }
    private IEnumerator HandleBossDeath()
    {
        gameObject.GetComponent<CarrotAttackManager>().StopShooting();
        weakspotsManagerScript.RemoveAllWeakSpots();

        // 1. 슬로우 모션 적용
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // 2. 보스의 죽음 애니메이션 실행
        bossControlScript.BossDeath();

        // 3. 카메라가 보스 주위를 순회
        yield return StartCoroutine(cameraScript.OrbitAroundBoss());

        //(Optional)
        //gameClearLight.ActivateLight(player);
        gameClearLight.ActivateLight(activeCharacter.transform);
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
        StartCoroutine(AddScoreBasedOnLives());

        // 6. 승리 음악 재생
        if (musicManager != null)
        {
            musicManager.StopMusic();
            AudioSource.PlayClipAtPoint(victoryMusic, Camera.main.transform.position, soundVolume);
        }
        gameClear.SetActive(true);
        isStageComplete = true;
        GameManager.inst.CursorActive(true);
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
        carrotSpeed += 5f;
        currentPhase += 1;
        if (currentPhase < 3) StartCoroutine(weakspotsManagerScript.NewWeakSpots());
        for (int i = 0; i < bossMaxLife; i++)
            darkHearts[i].SetActive(i < GetBossLife());
    }

    private IEnumerator WaitForIntro()
    {
        if (introAnimationDuration < transitionManager.countdownDuration)
            Debug.LogError("Countdown finishes before animation");
        yield return new WaitForSecondsRealtime(transitionManager.countdownDuration);

        StartLevel();
    }

    private void StartLevel()
    {
        base.isPausable = true;
        gameObject.GetComponent<DropAttackManager>().enabled = true;
        playerScript.enabled = true;
        bossControlScript.enabled = true;
        gameObject.GetComponent<CarrotAttackManager>().enabled = true;
        weakspotsManagerScript.enabled = true;
    }
}
