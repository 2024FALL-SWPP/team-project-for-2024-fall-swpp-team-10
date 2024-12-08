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

    [Header("Intro Settings")]
    public Transform stadiumTransform; // 경기장 위치
    //private Transform bossTransform; // 보스 위치
    public float cameraOrbitDuration = 3f; // 카메라 회전 시간
    //private GameObject bossObject; // 보스 객체
    private Transform gameplayCameraTransform; // 게임 플레이 시점의 카메라 Transform
    private AudioSource[] fireAudioSources;
    private Animator fadeImageAnimator;
    protected override void Awake()
    {
        base.Awake();
        //bossObject = GameObject.Find("Boss");
        cameraScript.SetCameraFixed(true); // 카메라 움직임 중지
        fadeImageAnimator = GameObject.Find("FadeImage").GetComponent<Animator>();
        //bossTransform = bossObject.transform;
        gameplayCameraTransform = cameraScript.transform;
        transitionManager = FindObjectOfType<BossStageTransitionManager>();
        GameManager.inst.CursorActive(true);
        maxLife = GameManager.inst.bossStageMaxLife;
        currentPhase = 0;
        fires = GameObject.FindGameObjectsWithTag("Fire");
        if (fires != null)
        {
            fireAudioSources = new AudioSource[fires.Length];
            for (int i = 0; i < fires.Length; i++)
            {
                fireAudioSources[i] = fires[i].GetComponent<AudioSource>();
                fireAudioSources[i]?.Stop();
                fires[i].SetActive(false);
            }
        }
        gameClearLight = GetComponent<GameClearLight>();
        if (transitionManager != null && transitionAnimator != null)
        {
            transitionAnimator.gameObject.SetActive(true);
            introAnimationDuration = transitionManager.BossStageTransition();
            //StartCoroutine("WaitForIntro");
            //StartCoroutine(transitionManager.Countdown());
            StartCoroutine(IntroSequence());

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

    // 인트로 시퀀스 Coroutine 수정
    private IEnumerator IntroSequence()
    {
        // 카메라를 경기장 위치로 즉시 이동
        cameraScript.transform.position = stadiumTransform.position + new Vector3(-8, 5, -10); // 적절한 오프셋 적용
        cameraScript.transform.LookAt(stadiumTransform);
        // 1. 페이드 인 시작 (경기장 보여주기)
        //fadeImageAnimator.SetTrigger("FadeIn");
        yield return new WaitForSecondsRealtime(1f); // 페이드 인 시간



        // 불 기둥 활성화 및 소리 재생
        StartCoroutine(ActivateFiresInPhases());

        // 불 기둥 연출 시간 대기
        yield return new WaitForSecondsRealtime(1f);

        // 2. 페이드 아웃
        fadeImageAnimator?.SetTrigger("FadeOut");
        yield return new WaitForSecondsRealtime(1f);

        // 카메라를 보스 위치로 즉시 이동
        //cameraScript.transform.position = bossTransform.position;
        //cameraScript.transform.LookAt(bossTransform);

        // 3. 페이드 인 (보스 보여주기)
        fadeImageAnimator?.SetTrigger("FadeIn");
        //yield return new WaitForSecondsRealtime(1f);

        // 보스 주위를 도는 카메라 연출 (동적 움직임)
        yield return StartCoroutine(DynamicCameraMovement(bossControlScript.transform, 10,8,new Vector3(0,0,0)));

        // 보스 연출 시간 대기
        //yield return new WaitForSecondsRealtime(1f);

        // 4. 페이드 아웃
        fadeImageAnimator?.SetTrigger("FadeOut");
        yield return new WaitForSecondsRealtime(1f);

        // 카메라를 플레이어 캐릭터 위치로 즉시 이동
        //cameraScript.transform.position = activeCharacter.transform.position;
        //cameraScript.transform.LookAt(activeCharacter.transform);

        // 5. 페이드 인 (캐릭터 보여주기)
        fadeImageAnimator?.SetTrigger("FadeIn");
        //yield return new WaitForSecondsRealtime(1f);

        // 캐릭터 주위를 도는 카메라 연출 (동적 움직임)
        yield return StartCoroutine(DynamicCameraMovement(activeCharacter.transform, 1,2,new Vector3(0, 0,-5)));

        // 캐릭터 연출 시간 대기
        //yield return new WaitForSecondsRealtime(0.5f);

        // 6. 페이드 아웃
        fadeImageAnimator?.SetTrigger("FadeOut");
        yield return new WaitForSecondsRealtime(1f);

        // 카메라를 게임 플레이 시점으로 즉시 이동
        cameraScript.transform.position = gameplayCameraTransform.position;
        cameraScript.transform.rotation = gameplayCameraTransform.rotation;

        // 7. 페이드 인 (게임 플레이 시작)
        fadeImageAnimator?.SetTrigger("FadeIn");
        cameraScript.SetCameraFixed(false); // 카메라 움직임 중지
        yield return new WaitForSecondsRealtime(0.5f);


        // 8. 카운트다운 시작
        StartCoroutine("WaitForIntro");
        StartCoroutine(transitionManager.Countdown());
    }

    private IEnumerator DynamicCameraMovement(Transform target, int h, int radius, Vector3 offset)
    {
        float duration = cameraOrbitDuration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // ���� ���
            float angle = (elapsed / duration) * 30f; 
            Vector3 orbitPosition = target.position + new Vector3(
                Mathf.Cos(Mathf.Deg2Rad * angle) * (-radius),
                h,
                Mathf.Sin(Mathf.Deg2Rad * angle) * (-radius)
            );

            // ī�޶� ��ġ �� ���� �ٶ󺸱�
            cameraScript.transform.position = orbitPosition + offset;
            transform.LookAt(target); // �׻� ������ �ٶ�

            elapsed += Time.unscaledDeltaTime; // ���ο� ��ǿ����� ���������� ����ǵ��� unscaledDeltaTime ���
            yield return null;
        }
    }

    private IEnumerator ActivateFiresInPhases()
    {
        // 첫 번째 절반 활성화
        for (int i = 0; i < fires.Length; i+=2)
        {
            fires[i].SetActive(true);
            fireAudioSources[i]?.Pause();
        }
        yield return new WaitForSecondsRealtime(1f); // 첫 번째 단계 대기

        // 두 번째 절반 활성화
        for (int i = 1; i < fires.Length; i+=2)
        {
            fires[i].SetActive(true);
            fireAudioSources[i]?.Pause();
        }
        yield return new WaitForSecondsRealtime(1f); // 첫 번째 단계 대기

    }
}
