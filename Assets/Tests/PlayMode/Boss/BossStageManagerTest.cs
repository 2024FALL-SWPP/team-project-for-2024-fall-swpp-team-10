using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using TMPro;
using EnumManager;
using System.Linq;
using UnityEngine.EventSystems;

public class BossStageManagerTest
{
    private GameManager gameManager;

    [SetUp]
    public void Setup()
    {
        GameObject gameManagerObject = new GameObject("GameManager");
        gameManager = gameManagerObject.AddComponent<GameManager>();
        GameManager.inst = gameManager;

        gameManager.SetMaxLife(3);
        gameManager.SetBossStageMaxLife(5);
        gameManager.SetCharacter(Character.Minji);
        gameManager.SetStage(1);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(GameManager.inst.gameObject);
        GameManager.inst = null;
        SceneManager.UnloadSceneAsync($"BossStage{gameManager.GetStage()}Scene");
    }

    [UnityTest]
    public IEnumerator TestBossLifeInitialization()
    {
        gameManager.LoadBossStage();
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == $"BossStage{gameManager.GetStage()}Scene");

        BossStageManager bossStageManager = GameObject.FindObjectOfType<BossStageManager>();
        Assert.IsNotNull(bossStageManager);

        int initialBossLife = bossStageManager.GetBossLife();
        Assert.AreEqual(bossStageManager.GetBossMaxLife(), initialBossLife);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestIncrementPhase()
    {
        gameManager.LoadBossStage();
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == $"BossStage{gameManager.GetStage()}Scene");

        BossStageManager bossStageManager = GameObject.FindObjectOfType<BossStageManager>();
        Assert.IsNotNull(bossStageManager);

        int initialPhase = bossStageManager.GetPhase();
        bossStageManager.IncrementPhase();
        yield return null;

        int newPhase = bossStageManager.GetPhase();
        Assert.AreEqual(initialPhase + 1, newPhase);
        Assert.AreEqual(bossStageManager.GetBossMaxLife() - newPhase, bossStageManager.GetBossLife());

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestPlayerLifeSetToBossStageMaxLife()
    {
        gameManager.LoadBossStage();
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == $"BossStage{gameManager.GetStage()}Scene");

        int playerLife = gameManager.GetLife();
        int bossStageMaxLife = gameManager.GetBossStageMaxLife();

        Assert.AreEqual(bossStageMaxLife, playerLife);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestPauseAndResumeGame()
    {
        gameManager.LoadBossStage();
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == $"BossStage{gameManager.GetStage()}Scene");

        BossStageManager bossStageManager = GameObject.FindObjectOfType<BossStageManager>();
        Assert.IsNotNull(bossStageManager);

        bossStageManager.SendMessage("PauseGame");
        yield return null;

        GameObject pauseMenu = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "Pause");
        Assert.IsNotNull(pauseMenu);
        Assert.IsTrue(pauseMenu.activeSelf);
        Assert.AreEqual(0f, Time.timeScale);

        GameObject resumeButtonObject = GameObject.Find("Resume Button");
        Assert.IsNotNull(resumeButtonObject);

        var pointerClick = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(resumeButtonObject, pointerClick, ExecuteEvents.pointerClickHandler);
        yield return null;

        Assert.AreEqual(1f, Time.timeScale);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestGameOver()
    {
        gameManager.LoadBossStage();
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == $"BossStage{gameManager.GetStage()}Scene");

        BossStageManager bossStageManager = GameObject.FindObjectOfType<BossStageManager>();
        Assert.IsNotNull(bossStageManager);

        while (gameManager.GetLife() > 0)
        {
            gameManager.RemoveLife();
        }
        yield return null;
        yield return null;

        GameObject gameOverScreen = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "GameOver");
        Assert.IsNotNull(gameOverScreen);
        Assert.IsTrue(gameOverScreen.activeSelf);
        Assert.AreEqual(0f, Time.timeScale);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestGameClear()
    {
        gameManager.LoadBossStage();
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == $"BossStage{gameManager.GetStage()}Scene");

        BossStageManager bossStageManager = GameObject.FindObjectOfType<BossStageManager>();
        Assert.IsNotNull(bossStageManager);

        while (bossStageManager.GetBossLife() > 0)
        {
            bossStageManager.IncrementPhase();
            yield return null;
        }

        bossStageManager.Invoke("Update", 0f);
        yield return null;

        yield return new WaitUntil(() =>
        {
            GameObject gameClear = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "GameClear");
            return gameClear != null && gameClear.activeSelf;
        });

        GameObject gameClearObj = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "GameClear");
        Assert.IsNotNull(gameClearObj);
        Assert.IsTrue(gameClearObj.activeSelf);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestObstaclesDeactivateOnStageComplete()
    {
        gameManager.LoadBossStage();
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == $"BossStage{gameManager.GetStage()}Scene");

        BossStageManager bossStageManager = GameObject.FindObjectOfType<BossStageManager>();
        Assert.IsNotNull(bossStageManager);

        while (bossStageManager.GetBossLife() > 0)
        {
            bossStageManager.IncrementPhase();
            yield return null;
        }

        bossStageManager.Invoke("Update", 0f);
        yield return null;

        yield return new WaitForSeconds(0.1f);

        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (var obstacle in obstacles)
        {
            Assert.IsFalse(obstacle.activeSelf);
        }

        yield return null;
    }
}
