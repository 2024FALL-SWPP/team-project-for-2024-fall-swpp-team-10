using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EnumManager;
using TMPro;
using UnityEngine.EventSystems;
public class MainStageManager : StageManager
{
    [Header("MainStage End Condition Settings")]
    public float stageDuration = 180.0f;
    private float currentStageTime = 0.0f;
    private bool isSpawnStopped = false;
    public GameObject boss;
    public GameObject bossLandingParticle;
    float bossDropSpeed = 10f;
    protected MainStageTransitionManager transitionManager;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        maxLife = GameManager.inst.maxLife;
        GameManager.inst.ResetStats();
        transitionManager = FindObjectOfType<MainStageTransitionManager>();
        GameManager.inst.CursorActive(false);
        StartCoroutine(AddScoreEverySecond());
    }

    public override void Update()
    {
        base.Update();

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
    }

    public IEnumerator CompleteStage()
    {
        base.isPausable = false;
        activeCharacter.GetComponent<MainStagePlayer>().SetEnableKeys(false);
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
        StartCoroutine(AddScoreBasedOnLives());
        // Freeze the game
        Time.timeScale = 0f;



        // Start the transition sequence
        if (transitionManager != null)
        {
            yield return StartCoroutine(transitionManager.StartMainStageTransition());
        }
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        base.OnSceneLoaded(scene, mode);
        transitionManager.SetCurrentCharacter(activeCharacter);
        activeCharacter.GetComponent<MainStagePlayer>().ChangeColorOriginal();
    }

    public override void PauseGame()
    {
        base.PauseGame();
        activeCharacter.GetComponent<MainStagePlayer>().SetEnableKeys(false);
        GameManager.inst.CursorActive(true);
    }

    public override void ResumeGame()
    {
        base.ResumeGame();
        activeCharacter.GetComponent<MainStagePlayer>().SetEnableKeys(true);
        GameManager.inst.CursorActive(false);
    }

    // ���� ���� ó��
    protected override void HandleGameOver()
    {
        base.HandleGameOver();
        activeCharacter.GetComponent<MainStagePlayer>().SetEnableKeys(false);
        GameManager.inst.CursorActive(true);
    }
    public IEnumerator AddScoreEverySecond()
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
