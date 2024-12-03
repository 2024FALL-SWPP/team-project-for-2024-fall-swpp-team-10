using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

// with GPT
public class MainStageManagerTest
{
    private MainStageManager mainStageManager;
    private GameObject gameManagerObject;
    private GameManager gameManager;

    [SetUp]
    public void Setup()
    {
        // Create a GameManager instance
        gameManagerObject = new GameObject("GameManager");
        gameManager = gameManagerObject.AddComponent<GameManager>();
        GameManager.inst = gameManager;

        gameManager.LoadMainStage();
        mainStageManager = GameObject.FindObjectOfType<MainStageManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(gameManagerObject);
        Object.Destroy(mainStageManager);
    }

    [UnityTest]
    public IEnumerator TestStageComplete()
    {
        gameManager.LoadMainStage();
        mainStageManager = GameObject.FindObjectOfType<MainStageManager>();
        Assert.IsFalse(mainStageManager.IsStageComplete());

        yield return null;
        mainStageManager = GameObject.FindObjectOfType<MainStageManager>(); //안하면 Destroy됐다고 뜸
        mainStageManager.StartCoroutine(mainStageManager.CompleteStage());

        yield return null;

        // Assert
        Assert.IsTrue(mainStageManager.IsStageComplete());
    }

    [UnityTest]
    public IEnumerator TestScoreAddsEverySecond()
    {
        gameManager.LoadMainStage();
        mainStageManager = GameObject.FindObjectOfType<MainStageManager>();
        Assert.AreEqual(0, GameManager.inst.GetScore());
        // Arrange
        int initialScore = GameManager.inst.GetScore();

        // Act
        yield return new WaitForSeconds(1.1f);

        // Assert
        Assert.Greater(GameManager.inst.GetScore(), initialScore);
        Assert.AreEqual(initialScore + 100, GameManager.inst.GetScore());
    }

    [UnityTest]
    public IEnumerator TestIsSpawnStopped()
    {
        gameManager.LoadMainStage();
        mainStageManager = GameObject.FindObjectOfType<MainStageManager>();
        Assert.IsFalse(mainStageManager.IsSpawnStopped());
        // Stage duration을 4.9초로 설정
        mainStageManager.stageDuration = 4.9f;

        // 시간이 거의 끝나갈 때 isSpawnStopped가 true로 설정되는지 확인
        mainStageManager.Update();
        yield return null;
        Assert.IsTrue(mainStageManager.IsSpawnStopped());
    }

    [UnityTest]
    public IEnumerator TestPauseGame()
    {
        gameManager.LoadMainStage();
        mainStageManager = GameObject.FindObjectOfType<MainStageManager>();
        // Act
        Cursor.visible = false; // 테스트 시작 전 초기화
        mainStageManager.PauseGame();

        yield return new WaitForEndOfFrame();

        // Assert
        Assert.IsTrue(Cursor.visible);
    }

    [UnityTest]
    public IEnumerator TestResumeGame()
    {
        gameManager.LoadMainStage();
        mainStageManager = GameObject.FindObjectOfType<MainStageManager>();
        // Act
        Cursor.visible = false; // 테스트 시작 전 초기화
        mainStageManager.PauseGame();

        yield return new WaitForEndOfFrame();

        mainStageManager = GameObject.FindObjectOfType<MainStageManager>(); //안하면 Destroy됐다고 뜸
        // Act
        mainStageManager.ResumeGame();
        yield return null;

        // Assert
        Assert.IsFalse(Cursor.visible);
    }
}
