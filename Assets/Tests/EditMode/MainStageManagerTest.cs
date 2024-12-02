using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainStageTest
{
    private GameObject gameManagerObject;
    private GameManager gameManager;
    private MainStageManager mainStageManager;

    [SetUp]
    public void Setup()
    {
        // Unity 엔진의 객체 초기화
        gameManagerObject = new GameObject();
        gameManager = gameManagerObject.AddComponent<GameManager>();

        // GameManager 초기화
        gameManager.maxLife = 3;
        gameManager.ResetStats();
        GameManager.inst = gameManager;

        // MainStageManager 초기화
        GameObject mainStageObject = new GameObject();
        mainStageManager = mainStageObject.AddComponent<MainStageManager>();

        // 스테이지 완료를 체크할 수 있도록 변수들 설정
        mainStageManager.stageDuration = 10f;
    }

    [Test]
    public void TestStageComplete()
    {
        Assert.IsFalse(mainStageManager.IsStageComplete(), "Stage should not be completed at first");

        // Act
        mainStageManager.StartCoroutine(mainStageManager.CompleteStage());

        // Assert
        Assert.IsTrue(mainStageManager.IsStageComplete(), "Stage should be completed after the coroutine.");
    }

    [Test]
    public void TestIsSpawnStopped()
    {
        Assert.IsFalse(mainStageManager.IsSpawnStopped(), "Spawn should start at first.");
        // Stage duration을 4.9초로 설정
        mainStageManager.stageDuration = 4.9f;

        // 시간이 거의 끝나갈 때 isSpawnStopped가 true로 설정되는지 확인
        mainStageManager.Update();

        Assert.IsTrue(mainStageManager.IsSpawnStopped(), "Spawn should be stopped when the stage time is near its end.");
    }

    [Test]
    public void TestGamePauseAndResume()
    {
        // Act: 게임 일시 정지
        mainStageManager.PauseGame();

        // Assert: 일시 정지 상태에서 Cursor가 활성화되어야 함
        Assert.IsTrue(Cursor.visible, "Cursor should be visible when the game is paused.");

        // Act: 게임 재개
        mainStageManager.ResumeGame();

        // Assert: 재개 후 Cursor가 비활성화되어야 함
        Assert.IsFalse(Cursor.visible, "Cursor should be hidden when the game is resumed.");
    }
}
